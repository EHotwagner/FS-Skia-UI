# Phase 0 Research: Deterministic Escalated Validation Path

## R1 — Root cause and the corrective backend selection

**Decision**: Force the X11/Xvfb path for graphics-touching validation processes by
**removing `WAYLAND_DISPLAY`** and **setting `GDK_BACKEND=x11` and
`SDL_VIDEODRIVER=x11`**, applied only when the dual-display condition holds
(`WAYLAND_DISPLAY` present *and* `DISPLAY` present).

**Rationale**: Root cause confirmed 2026-05-31 — the headless host sets
`WAYLAND_DISPLAY=wayland-0` *and* `DISPLAY=:1` (Xvfb). GLFW/SDL prefer Wayland and
try to load `libdecor-gtk.so`, which fails to init in the container. The tests
themselves PASS (e.g. SkiaViewer.Tests 39/39); the host aborts only on teardown, and
the same Wayland path makes a generated product's `Verify` FSI startup hang ~20 min.
Xvfb (`DISPLAY=:1`) already provides a working X11 surface, so forcing X11 is
behavior-preserving on this host. The condition guard keeps the change a no-op where
Wayland is the *only* display (real Wayland desktop) or where neither is set
(non-Linux / headed-X11-only), satisfying FR-007 / SC-004.

**Alternatives considered**:
- *Fix the libdecor plugin / install GTK deps in the container* — addresses the
  symptom host-side, not in-repo; not reproducible for every operator; rejected.
- *Add a software-renderer fallback* — large scope, explicitly out of scope, and
  would change rendering behavior.
- *Set `GDK_BACKEND` unconditionally* — risks altering behavior on real Wayland
  desktops; rejected in favor of the dual-display guard.

## R2 — Where to apply the normalization (single source vs launcher)

**Decision**: Apply in the **compiled front-end**, at two layers: (1) normalize the
ambient process environment once in `build/Program.fs` at startup; (2) re-apply at
the process-spawn edge (`BuildProcess`, `BuildProcessHealth`) on each child's
`startInfo.Environment`. Do **not** edit `fake.sh` / `fake.cmd`.

**Rationale**: `runProcessWithAllowedExitCodes` uses `UseShellExecute=false`, so
children inherit the parent front-end's environment and only the explicitly-passed
map is overlaid. Nested generated-product validation is launched as
`bash ./fake.sh build -t <target>` (`GeneratedProduct.fs:328,933`), which re-enters
the front-end — so a single startup normalization in the parent propagates to
**every** descendant: `dotnet test`, FSI scripts, and nested `fake.sh`. Layer (1)
gives propagation; layer (2) gives a **directly unit-testable guarantee** at the
edge and defends against a child that resets env. Keeping it in compiled F# (not the
shell launchers) is cross-platform, single-source (consistent with the 045 "whole
front-end is compiled" philosophy), and testable — whereas a `fake.sh` `unset` is
Linux-only, duplicated in `fake.cmd`, and untested.

**Alternatives considered**:
- *Launcher-only `unset WAYLAND_DISPLAY`* — Linux-only, duplicated across two
  launchers, bypassed when the front-end is run via `dotnet run --project build`
  directly, and not unit-testable; rejected as the primary fix (acceptable only as
  redundant hardening, omitted to keep the diff single-source).
- *Per-call env maps at every `runProcess` call site* — error-prone (many call
  sites, easy to miss one) and noisy; the edge + startup approach is centralized.

## R3 — Hang vs timeout behavior

**Decision**: Rely on removing the Wayland path to eliminate the hang; additionally
enrich the existing `BuildProcess` timeout branch so that a kill-on-timeout emits a
diagnostic naming the likely graphics-backend cause and pointing at
`runtime-limitations.md`.

**Rationale**: `BuildProcess.runProcessWithAllowedExitCodes` already kills at a
30-minute `WaitForExit`. The observed ~20-min hang sits *inside* that window, so the
timeout never fired authoritatively — the fix is to remove the stall, not lengthen
the timeout. The enriched diagnostic covers the residual edge case (FR-005 / SC-005:
no usable backend) so a genuine init failure fails fast-and-legible instead of
looking like a product regression. The 30-minute bound is left unchanged (reducing
it is a separate tuning concern, out of scope).

**Alternatives considered**:
- *Shorten the global timeout* — risks false failures on legitimately slow targets;
  out of scope.
- *Add a separate short graphics-init probe/timeout* — more machinery than the flake
  warrants once the Wayland selection is removed.

## R4 — Not masking real failures (FR-004 vs FR-008)

**Decision**: Do not suppress or rewrite child exit codes. FR-004 (teardown crash
after a green run) is addressed by *removing the crash cause* (the Wayland teardown),
not by ignoring nonzero exits. The focused-control corroboration
(`dotnet test tests/SkiaViewer.Tests -m:1`) remains available as evidence.

**Rationale**: Swallowing nonzero exit codes would violate Principle VI/VII and risk
masking genuine regressions (FR-008). Because the crash was a *teardown* artifact of
the Wayland path, eliminating that path lets the real (passing) result stand without
any exit-code manipulation.

**Alternatives considered**:
- *Treat teardown-only crashes as pass via output heuristics* — fragile, could mask
  real crashes; rejected.

## Resolved unknowns

No `NEEDS CLARIFICATION` items remained from the spec or Technical Context. The root
cause is confirmed, the affected hosts are identified, the propagation mechanism is
verified by reading the spawn code, and the safety boundary (dual-display guard) is
defined.
