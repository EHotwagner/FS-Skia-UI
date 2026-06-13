# Phase 1 Data Model: Backend Paint Replay & Performance Honesty

## Entity: `CacheBoundary` / `SceneNode.CachedSubtree` (Scene IR, public, additive)

A transparent wrapper marking a reuse-stable subtree as a replay-cache boundary.

| Field | Type | Meaning |
|---|---|---|
| `CacheId` | `uint64` | Stable identity of the subtree (derived from `RetainedId`); the cache slot. Stable across frames for the same logical subtree. |
| `Fingerprint` | `uint64` | Collision-resistant structural hash of the wrapped subtree's render-affecting inputs (R1). Decides replay validity. |
| `Scene` | `Scene` | The wrapped subtree — both the record source and the transparent fallback. |

**Validity rules**
- When the backend replay cache is **disabled** (oracle), or any IR consumer other than the
  backend painter visits it, the node is **transparent**: behavior == recursing into `Scene`.
- `CacheId` MUST be stable frame-to-frame for the same subtree; `Fingerprint` MUST change iff
  any render-affecting input of `Scene` changes (R1).
- Emitted only for subtrees reuse-stable on the prior frame (FR-012); never for a subtree first
  seen this frame.

## Entity: Subtree fingerprint (Controls, internal)

- `Fingerprint: uint64` field added to the retained **fragment** record.
- Computed in `paintFresh`/`buildFresh` (repaint paths) via `hashScene`; carried unchanged on
  `Keep` reuse. Cost ∝ damage set, not tree size.
- Replaces the `sprintf "%A"`-based `PictureCacheKey.Picture` digest; `PictureCacheKey` becomes
  `{ Box: Box; Fingerprint: uint64 }` (still `internal`).

## Entity: `PictureReplayCache` (SkiaViewer, internal) + entry

A bounded LRU mapping cache identity → recorded native picture.

| Field | Type | Meaning |
|---|---|---|
| `Entries` | `Dictionary<uint64, Entry>` | keyed by `CacheId`. `mutable` (native handles). |
| `Cap` | `int` | capacity bound (default mirrors `PictureCacheCap = 256`). |
| `enabled` | `bool` | oracle: `false` ⇒ never record/replay (parity proof). |

`Entry`:

| Field | Type | Meaning |
|---|---|---|
| `Picture` | `SKPicture` | recorded draw commands (native; owns GPU/native memory). |
| `Fingerprint` | `uint64` | the fingerprint the picture was recorded for. |
| `Stamp` | `int64` (mutable) | LRU recency (deterministic monotonic counter, not wall clock). |

**Lifecycle / validity rules**
- **Hit**: entry exists and `entry.Fingerprint = boundary.Fingerprint` → `DrawPicture` (replay).
- **Miss / stale**: absent or fingerprint differs → record (`SKPictureRecorder.BeginRecording`
  over the boundary box → paint `Scene` → `EndRecording`), disposing any replaced picture,
  then draw.
- **Eviction**: when `Entries.Count > Cap`, evict the min-`Stamp` entry and `Dispose` its
  `Picture`. A later request for an evicted `CacheId` is a miss (re-record), never a stale hit.
- **Disposal**: `Dispose` every `SKPicture` on eviction, on replacement, and on cache teardown.
  Native bytes are observable (`ReplayCacheNativeBytes`).
- **Coordinate space**: the picture is recorded at the box coordinates it is replayed at (R2);
  a relayout that moves a subtree changes its box → different fingerprint → re-record.

## Entity: Frame paint record (Controls.Elmish `FrameMetrics`, public, additive)

New fields (additive; golden counters except the two `TimeSpan` durations which are non-golden):

| Field | Type | Golden? | Meaning |
|---|---|---|---|
| `PaintDuration` | `TimeSpan` | no | scene→canvas walk time (live only; `Zero` on `Perf.runScript`). |
| `ComposeDuration` | `TimeSpan` | no | flush + buffer-swap present time (live only). |
| `ReplayHitCount` | `int` | yes | boundaries replayed from a valid recorded picture. |
| `ReplayMissCount` | `int` | yes | boundaries recorded (cold, changed fingerprint, or evicted). |
| `ReplayRecordCount` | `int` | yes | pictures recorded this frame. |
| `ReplaySkippedNodeCount` | `int` | yes | subtree paint-nodes skipped by replay (the win signal). |
| `ReplayCacheNativeBytes` | `int` | yes (bounded) | native memory held by the replay cache after this frame. |

Existing field **corrected**: `DirtyArea` now = union area (R5), `≤` frame area.

## Entity: Frame-dirty signal (SkiaViewer model, internal)

- A flag (or small cause set reusing feature 111's `FrameCause`) on the viewer model: set by
  product message / resize / theme / active animation clock; cleared after present.
- `update` emits `RenderFrame` only when set (or when an animation clock is live); otherwise
  no scene work (FR-004/FR-006).

## Removed

- `lastRuntimeStateTouched` `ref` in the interactive host (`ControlsElmish.fs`, written-never-read;
  FR-018). Removal is behavior-neutral.
