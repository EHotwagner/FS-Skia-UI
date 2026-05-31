# Quickstart: Authoring Guidance Consistency

How to validate this feature once implemented. FAKE-backed targets run
**sequentially** — never concurrently (shared `.fake` state).

## 1. Skill-id resolution guard (US1, P1)

```bash
./fake.sh build -t GeneratedGuidanceCheck
```

- Confirms every advertised skill id resolves to a declared `name:`, every
  skill's directory/`name:`/advertised-id agree, and `.agents`↔`.claude` peers
  match. With `speckit-debug-loop` removed it PASSES.
- Failing-first: temporarily reintroduce `speckit-debug-loop` (or rename a skill
  directory away from its `name:`) and confirm the check FAILS, naming the
  offending id and file:line. Evidence → `readiness/skill-resolution.md`,
  fixtures → `readiness/skill-resolution-fixtures/`.

## 2. Local API reference, no reflection (US2, P1)

```bash
./fake.sh build -t TemplateCheck
```

- In a freshly generated project, confirm `docs/api-surface/` contains the real
  `.fsi` signatures for every package the profile references, and read a union
  case's exact field order (e.g. `SceneNode.Rectangle`) from it without DLL
  reflection. Evidence → `readiness/generated-api-reference.md`.

## 3. No name collisions on `open` (US3, P2)

```bash
./fake.sh build -t Dev
./fake.sh build -t PackageSurfaceCheck
```

- Compile a consumer fixture that `open`s the viewer namespace and defines its
  own `Normal`, `update`, and `init`. FAIL before the RQA hardening, PASS after.
- Confirm refreshed surface baselines and the recorded migration note +
  version bump. Evidence → `readiness/fsi/`, `readiness/name-collision-migration.md`.

## 4. Domain-agnostic, consumer-facing generated guidance (US4, P2)

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
```

- Confirm the generated starter app + tests contain zero demo-specific
  identifiers (`tetris`, `score`, `level`, `next piece`, `board`, `piece`),
  generated skills carry ≥1 consumer-runnable snippet, and no generated guidance
  points at framework-only paths/targets. Evidence → `readiness/generated-guidance.md`.

## 5. Canonical effects page (US5, P3)

- In a generated project, open `docs/effects-boundary.md`; confirm it names both
  effect categories, the boundary, and the `update`→host wiring, with no need to
  read `docs/reports/*` or source. Evidence → `readiness/effects-boundary.md`.

## 6. Consistent scene constructors (US6, P3)

- Compile both the existing positional `Rectangle`/`Text` constructors and the
  new self-describing forms in one FSI fixture; both succeed. Evidence →
  `readiness/fsi/`.

## 7. Evidence-gate targeting regression (FR-011, P3)

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

- Confirm the gates echo the feature resolved from `.specify/feature.json` and
  that a bare filename mention in a `tasks.md` fixture does not trigger required
  evidence. Evidence → `readiness/feature-targeting-regression.md`.

## Full sequential validation

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Governing check (SC-001)

A freshly generated consumer project builds, runs its tests, and produces its
evidence using **only local references** — zero DLL reflection, zero dependence
on framework-repo-only paths/targets/skills. No framework-repo-process item
(FR-001, FR-011) may be considered done if it has degraded this outcome.
