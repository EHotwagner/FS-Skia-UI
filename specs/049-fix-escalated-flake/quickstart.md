# Quickstart: Verifying the Deterministic Escalated Validation Path

## What changed

The compiled build front-end now forces the working X11 graphics path whenever the
host advertises both a Wayland and an X11 display (the headless/Xvfb container
case), and re-applies that to every child process it spawns. This removes the
`libdecor-gtk` teardown crash and the ~20-minute generated-product FSI startup hang
without any manual environment setup.

## Fast inner-loop check (pure logic + spawn contract)

```bash
# Unit + contract tests for the normalization (fast, no graphics):
dotnet test tests/Governance.Tests/Governance.Tests.fsproj \
  --filter "FullyQualifiedName~GraphicsEnvironment"
```

Expect: dual-display input → `WAYLAND_DISPLAY` removed, `GDK_BACKEND=x11` /
`SDL_VIDEODRIVER=x11` set; single-display and no-display inputs → unchanged; a real
spawned child observes no `WAYLAND_DISPLAY`.

## Authoritative escalated check (single run, no manual env, no rerun)

Run the serialized escalated order **once**, with no `env -u WAYLAND_DISPLAY ...`
prefix:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

(Run FAKE-backed targets **sequentially** — they share `.fake` state.)

Expect, on the headless host:
- No `Failed to load plugin 'libdecor-gtk.so'` / test-host teardown crash.
- `GeneratedProductCheck` completes within its normal envelope (no ~20-min stall).
- A pass verdict from a **single** run — no focused rerun required to be authoritative.

## Confirm the safety boundary (headed / native host)

On a real desktop (single display) or non-Linux host, behavior — including a sample
viewer's visual output — is unchanged:

```bash
./fake.sh build -t Dev
# and, on a headed host, a sample viewer renders exactly as before:
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
```

## Confirm fail-fast on a broken backend (edge case)

If no usable backend exists, the affected step is killed at its timeout and the log
names a probable graphics-backend init failure and points to
`readiness/runtime-limitations.md` — it does **not** hang indefinitely.

## Evidence produced

- `readiness/aggregate-hang-diagnostics.md` — the single-run authoritative verdict.
- `readiness/logs/` — Dev / GeneratedProductCheck logs free of `libdecor-gtk` crashes.
- `readiness/runtime-limitations.md`, `readiness/graphics-env-contract.md`.
