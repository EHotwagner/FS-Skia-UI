# Phase 1 Data Model: Live Host Pacing, Surface Honesty & Viewer Ergonomics

This feature is loop/config/doc work, not a domain-entity feature. The "entities" are the
public config shape, the two pure decisions under test, and the published surface.

## 1. `ViewerOptions` (public, `src/SkiaViewer/SkiaViewer.fsi`)

| Field | Type | Notes |
|-------|------|-------|
| `Title` | `string` | unchanged |
| `InitialSize` | `Size` | unchanged; validated `> 0` |
| `PresentMode` | `ViewerPresentMode` | unchanged |
| `FrameRateCap` | `int option` | **new, additive.** `None` ⇒ default 60 (today's behavior). `Some n` ⇒ target/cap of `n` FPS. Mirrors `ViewerConfiguration.TargetFrameRate : int option`. |

- **Validation rule** (extends `validateOptions`): `FrameRateCap = Some n` with `n <= 0`
  ⇒ `Result.Error` startup diagnostic (`Window`/`App` `ProductDefect` `Startup`),
  consistent with the existing positive-size check. `None` and `Some n` (n > 0) pass.
- **Default/back-compat path**: existing record construction that omits `FrameRateCap`
  must keep compiling. Resolution: provide a defaulting construction helper
  (`ViewerOptions.create` / `withFrameRateCap`) and a `defaults`-style record so call
  sites and FSI preludes do not need editing; samples that build the record positionally
  add `FrameRateCap = None`. (Every construction site is caught by `RefreshSurfaceBaselines`
  Build + `FsiTranscripts`, per the 100/108 record-field-addition gotchas.)
- **Flow**: `FrameRateCap |> Option.orElse (Some 60)` (or `Option.defaultValue 60`) into
  `ViewerConfiguration.TargetFrameRate` at `SkiaViewer.fs:1232–1236`, replacing the
  literal `TargetFrameRate = Some 60`.

## 2. Pure pacing decision (testable; native loop calls it)

```
shouldAdvanceFrame (lastFrameTime: float) (now: float) (frameInterval: float) : bool
    = now - lastFrameTime >= frameInterval
```

- Extracted from `Host/OpenGl.fs` `runEventLoop` so the **render** gate (FR-002) and the
  existing **update** gate share one tested decision. `frameInterval = 1.0 / max 1 cap`.
- **Invariants under test**:
  - cap `n` ⇒ at most ~`n` advances per simulated second (cadence bounded by the cap);
  - a larger interval yields strictly fewer advances over the same window;
  - the first frame (`lastFrameTime` sentinel) always advances (cold frame renders).
- **Wiring**: `runEventLoop` calls `shouldAdvanceFrame` to gate **both** `DoUpdate()` and
  `DoRender()` (today only update is gated; render runs every poll). `Thread.Sleep(1)` and
  feature-120 paint-skip are unchanged.

## 3. Clock-advance no-alloc invariant (internal; `wrappedTick`)

```
advanceClocks (delta) (state: Map<Identity, RetainedUiState>) : Map<Identity, RetainedUiState>
    = if state |> Map.exists (fun _ s -> s.Animation |> Option.exists clockActive)
      then state |> Map.map (fun _ s -> { s with Animation = s.Animation |> Option.map (advance delta) })
      else state            // reference-equal passthrough — no allocation
```

- **Invariants under test**:
  - no active clock ⇒ `obj.ReferenceEquals(result, input)` (no allocation, no behavior
    change);
  - ≥1 active clock ⇒ every active clock advanced by `delta` exactly as the current
    `Map.map` path (features 099/103 unchanged);
  - a settled clock present but inactive does not force a rebuild.
- `clockActive` already exists (`RetainedRender.fs` ~551). `RetainedRender.advance`
  already no-ops on settled/zero-delta, so the guard is cost-only.

## 4. Published surface (docs/api-surface/) — no code change, documentation of existing public types

| Type | Source `.fsi` | Cases |
|------|---------------|-------|
| `PointerButton` | `src/Controls/Pointer.fsi` ~9 | `Primary` \| `Secondary` \| `Middle` (`[<RequireQualifiedAccess>]`) |
| `PointerInteraction` | `src/Controls/Pointer.fsi` ~72 | `HoverEnter` \| `HoverLeave` \| `PressedDown` \| `ReleasedUp` \| `Click` \| `DragBegin` \| `DragMove` \| `DragEnd` \| `DragCancelled` \| `Scroll` \| `FocusMovedByPointer` \| `Diagnostic` |
| `ViewerPointerPhaseKind` | `src/SkiaViewer/SkiaViewer.fsi` ~539 | `Moved` \| `Pressed` \| `Released` \| `Wheel` \| `Exited` (`[<RequireQualifiedAccess>]`) |

Plus the `InteractiveAppHost` folding contract note: `MapPointer : PointerInteraction -> 'msg option`
and `MapKeyChord : ViewerKey -> KeyModifiers -> 'msg option` are the fallback seams an
authored binding defers to. A drift check fails if these published entries diverge from
the `.fsi`.
</content>
