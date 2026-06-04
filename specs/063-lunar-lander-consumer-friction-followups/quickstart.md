# Quickstart & Verification: Feature 063

End-to-end verification for the LunarLander consumer-friction follow-ups. FAKE-backed
commands share `.fake` state — **run them sequentially**, never concurrently.

## 0. Route first

```
./fake.sh build -t Route            # authoritative tier + minimal gate list for the diff
./fake.sh build -t Route --enforce  # also fails on missing escalated-tier evidence
```

Expect **Tier-1 escalation** once the FR-010 `Wrap` helper and the `SymbolCrossCheck`
target land (new `FS.Skia.UI.SkillSupport` surface baseline + new governance target).
Run only the gates Route prints.

## 1. Renderer parity (FR-001/002 — the headline)

Before the fix, capture image evidence of a scene containing `Line` + `Path` +
`Text`; confirm the PNG shows only a placeholder/pad. After the shared
`SceneRenderer.paintNode` lands:

- terrain `Line` nodes and the filled-ground `Path` render as real pixels;
- `Text` renders as real glyphs, not a box;
- the build **fails to compile** if any `SceneNode` case is unhandled (no wildcard);
- interactive (`Vulkan`) and evidence (`drawScreenshotScene`) renders of the same
  scene agree on which primitives appear.

Run the SkiaViewer/Scene tests and the before/after capture (golden image diff).
SkiaViewer per-package surface baseline is **unchanged** (the shared module is
non-public).

## 2. `SymbolCrossCheck` target (FR-003)

```
./fake.sh build -t SymbolCrossCheck     # reads plan/data-model/tasks from the feature dir
```

Seed a `Msg` case present in `data-model.md` + `tasks.md` but absent from `plan.md`;
confirm the printed `## Symbol consistency (analyze pass G)` reports the
proper-subset finding and `readiness/symbol-cross-check.md` is written — no throwaway
harness. Then:

```
./fake.sh build -t TargetMetadataDrift   # green: validation.contract.yml regenerated
./fake.sh build -t Test                  # Governance.Tests: no unknown-gate diagnostic
```

## 3. Evidence-format discoverability + diagnostics (FR-004/005)

- Trigger a readiness-contract failure with exactly **one** absent token; confirm the
  output prints `full-required-set:` and `absent-from-file:` as distinct labels.
- Confirm `.claude/skills/speckit-implement/SKILL.md` (regenerated) names
  `docs/evidence-formats.md` as a before-authoring reference and documents the
  `skill-loading-evidence.md` feature-dir location + `[X]`-gated timing.

## 4. Authoring references (FR-006/007/008)

- `speckit-plan` skill references `docs/scaffold-map.md`; the map carries the
  `.fsi`-authoritative note.
- `speckit-specify` skill snapshots an external-URL source into
  `specs/<feature>/source-spec.md`; local input is a no-op.
- FR-008: re-run the template scan and confirm no template seeds `evidence/`; record
  the result. No code change.

## 5. Shipped helper (FR-010)

```
./fake.sh build -t Test                 # Wrap.wrapDeltaX determinism/range/shortest-path
./fake.sh build -t PackageSurfaceCheck  # or PerPackageSurfaceDiff per Route
```

Confirm `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` includes the
`Wrap` module and the FSI transcript exercises the packed surface. Confirm
`fs-skia-layout-readability` references both `reserveHudBand` and `wrapDeltaX`, and
documents the deferred camera projection + `--evidence-run` summary discipline
(FR-009).

## 6. Governance regen + serialized validation

After any `.agents/skills/**` edit:

```
./fake.sh build -t RefreshSurfaceBaselines   # regenerate .claude tree + validation.contract.yml
```

Then the escalated serialized order (sequential):

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## 7. Merge gate

`./fake.sh build -t EvidenceAudit` returns `verdict=PASS` for
`specs/063-lunar-lander-consumer-friction-followups` with no `[S]`/`[S*]` and no
diff-scan hits (SC-007). All Route-printed gates pass, including
`SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck` and the SkillSupport
surface baseline.

## Evidence artifacts produced

`specs/063-lunar-lander-consumer-friction-followups/readiness/`:
`target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
`aggregate-hang-diagnostics.md` (Route-required escalated-tier); the before/after
renderer image capture (FR-001); `symbol-cross-check.md` (FR-003); the Wrap unit-test
output + updated surface baseline (FR-010); the template-scan record for FR-008.
