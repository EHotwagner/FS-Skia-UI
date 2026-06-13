# generated-guidance-validation — applicability (feature 115, T003)

status=not-applicable (no hand-edited generated guidance)

Feature 115 edits no generated guidance content by hand. The `speckit_version` bump in
`.specify/init-options.json` is a **recorded-version** edit: the repository's skill/command tree is
canonical under `.agents/**` and the `.claude/**` tree is generated **from it** (not vendored from spec-kit
upstream), so the version bump pulls no upstream assets and regenerates no guidance. The authoritative
generated-guidance signal for this change is the `GeneratedGuidanceCheck` FAKE gate, captured live in
`generated-guidance.md` after the routed gate run — not a hand-authored claim here. If a `.agents/**`
source asset were ever touched, the `.claude` tree would be regenerated with
`./fake.sh build -t RefreshSurfaceBaselines` and `SkillSyncCheck` would confirm currency; no such source
asset is touched by this feature.
