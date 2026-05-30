# Contract: ControlEventOrigin Qualified Access

Covers FR-007, FR-010. The single public-contract change in this feature.

## Surface delta

`src/Controls/Types.fsi` (and matching `Types.fs`):

```fsharp
[<RequireQualifiedAccess>]
type ControlEventOrigin =
    | Pointer
    | Keyboard
    | Text
    | Focus
    | Selection
    | Clipboard
```

The only change is the added `[<RequireQualifiedAccess>]` attribute. Cases are
unchanged. After this change, the cases must be referenced qualified
(`ControlEventOrigin.Text`), so the `Text` case no longer leaks into an opened
`FS.Skia.UI.Controls` namespace and no longer shadows the Scene `Text`
constructor.

This aligns `ControlEventOrigin` with its sibling DUs in the same module
(`KnownControl`, `KnownEvent`, `KnownAttribute`, `StandardControlKind`,
`StandardEventKind`, `StandardAttributeName`), which already carry the
attribute.

## Behavior guarantee (US3)

In a file that opens both `FS.Skia.UI.Scene` and `FS.Skia.UI.Controls` with
Controls last, constructing a scene text node unqualified resolves to the scene
construct (or, for any remaining collision, fails with a diagnostic naming the
colliding symbols) — never the opaque "value is not a function / has type
ControlEventOrigin" error. Verified by a fixture compiling the previously-failing
open order under `readiness/fsi/`.

Scene DU constructors and the shared `LayoutBounds` record remain
**guidance-governed** (FR-008); only `ControlEventOrigin` receives the attribute.

## Baseline impact

Refresh via `scripts/refresh-surface-baselines.fsx`:

- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`
- `readiness/surface-baselines/FS.Skia.UI.txt` (merged)

`FS.Skia.UI.Controls.ControlEventOrigin` and `...ControlEventOrigin+Tags` entries
remain; the baseline reflects the qualified-access surface. Validated by
`PackageSurfaceCheck` / `Dev`.

## Recorded reversal (FR-010)

`specs/035-api-discovery-names/readiness/name-collision-safety.md` previously
recorded `decision: consumer-guidance` / `compatibility: no-contract-change` for
the `Text` collision. This feature reverses that **for `ControlEventOrigin`
only**, with rationale: guidance proved insufficient (it cost real debugging
time and surfaced an opaque diagnostic). The reversal note names spec 037 as the
source and confirms no other collision decisions from spec 035 change.

## Package identity

No package identity, content, or version change. This is a public-contract
(`.fsi` surface) change to an existing type, not a new/changed package.
