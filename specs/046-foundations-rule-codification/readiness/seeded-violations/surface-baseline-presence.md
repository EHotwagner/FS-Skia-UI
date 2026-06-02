# Seeded-violation proof — surface-baseline presence (Stage 6.1, shipped)

**Rule**: every runtime capability with a `surfaceBaseline` must have the baseline file
present (`build/Governance/Capabilities.fs:124-128`, `validateRows`).
**Gate**: `CapabilityCheck` (`runCapabilityCatalogCheck`).
**Authoritative command**: `./fake.sh build -t CapabilityCheck`

Real seeded failure (the baseline file is genuinely removed, then restored):

## FAIL — `scene` surface baseline removed

```
$ mv readiness/surface-baselines/FS.Skia.UI.Scene.txt /tmp/   # seed
$ ./fake.sh build -t CapabilityCheck
- `scene` [surfaceBaseline]: Missing surface baseline readiness/surface-baselines/FS.Skia.UI.Scene.txt
Status:           Failure
(exit 134)
```

## PASS — baseline restored

```
$ mv /tmp/FS.Skia.UI.Scene.txt readiness/surface-baselines/   # restore
$ ./fake.sh build -t CapabilityCheck
Status:           Ok
```

`git status --porcelain readiness/surface-baselines/FS.Skia.UI.Scene.txt` → empty
(restored byte-for-byte). This Stage-6.1 gate is **still blocking**; its prose may be
trimmed under FR-008.
