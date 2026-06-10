# Generated guidance validation (feature 095, T024)

The Controls capability fragment skill (`template/fragments/controls/skill/SKILL.md`) carries the
expanded E1–E5 consumer guidance, so a `dotnet new fs-skia-ui` project that selects the Controls
capability receives it.

- **command:** `./fake.sh build -t GeneratedGuidanceCheck` (and `GeneratedProductCheck`).
- **artifact:** the generated project's selected Controls skill destination contains the
  `## Capability surface — E1–E5` section with runnable examples.
- **failure class:** product-defect (a generated consumer missing the shipped capabilities).
- **next action:** none — the guidance ships through the existing capability-fragment channel; no
  template-manifest change.

See [us4-skill-e1-e5.md](./us4-skill-e1-e5.md) for the per-rung inspection and
[generated-validation.md](./generated-validation.md) for the package-match / tests-ran record.
