# Phase 1 Data Model: Deterministic Escalated Validation Path

This feature is build-tooling behavior, not a domain data feature. The only
"entities" are the small value shapes the normalization function operates over.

## Entity: GraphicsDisplayState (derived, transient)

A classification of the ambient environment, computed from two variables.

| Field | Source | Meaning |
|-------|--------|---------|
| `hasWayland` | `WAYLAND_DISPLAY` present & non-empty | A Wayland display is advertised |
| `hasX11` | `DISPLAY` present & non-empty | An X11 (incl. Xvfb) display is advertised |

Derived classification:

- **DualDisplay** — `hasWayland && hasX11` → the failure condition; normalization applies.
- **WaylandOnly** — `hasWayland && not hasX11` → real Wayland host; **no-op**.
- **X11Only** — `not hasWayland && hasX11` → already on the working path; **no-op**.
- **Neither** — `not hasWayland && not hasX11` → non-Linux / headless-no-display; **no-op**.

## Transformation: normalizeGraphicsEnv

Pure function over an environment map (no side effects; the ambient read happens at
the call sites in `Program`/`BuildProcess`).

```
normalizeGraphicsEnv : Map<string,string> -> Map<string,string>
```

| Input classification | Output mutation |
|----------------------|-----------------|
| DualDisplay | remove key `WAYLAND_DISPLAY`; set `GDK_BACKEND=x11`; set `SDL_VIDEODRIVER=x11` |
| WaylandOnly | identity (unchanged) |
| X11Only | identity (unchanged) |
| Neither | identity (unchanged) |

**Validation rules**
- Idempotent: `normalize (normalize m) = normalize m` (after one pass the state is X11Only → no-op).
- Total: defined for every map, including empty.
- Non-destructive beyond the three named keys: no other entries are added or removed.
- Pure: identical output for identical input; no environment read inside the function.

## Application points (interpreter edge — not part of the pure model)

| Site | Action |
|------|--------|
| `build/Program.fs` startup | read ambient env → classify → if DualDisplay, apply removals/sets to the **current process** environment (so all descendants inherit) and log the decision once |
| `BuildProcess.runProcessWithAllowedExitCodes` | build child `startInfo.Environment` from current env + caller map, then apply `normalizeGraphicsEnv` so the child is guaranteed normalized |
| `BuildProcessHealth.runShortCommand` | same normalization applied to the short-command child |
| timeout branch (`BuildProcess`) | on kill, append a diagnostic naming probable graphics-backend init failure + pointer to `runtime-limitations.md` |

No persistence, no schema, no migration.
