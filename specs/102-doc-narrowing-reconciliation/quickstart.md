# Quickstart: Verifying Documented-Narrowing Reconciliation (R8)

R8 ships **no behavior change**. Verification is *inspection that prose now matches code*
plus *the existing gate suite staying green and unchanged*. A reviewer can confirm the whole
feature by walking the six reconciliations and the zero-behavior invariant.

## 1. Route first, run only what it prints

```sh
./fake.sh build -t Route          # authoritative tier + minimal gate list for THIS diff
```

Expect escalation to the **controls-public-surface** set (any `src/Controls/**/*.fs` edit
escalates, even a comment — feature 101 precedent). Run the printed gates **sequentially**
(FAKE-backed commands share `.fake` state — never concurrent), e.g.:

```sh
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## 2. Confirm each of the six reconciliations

| # | Where to look | What to confirm |
|---|---|---|
| FR-001 | roadmap §10.3 | Says `deriveVisualState` realizes only the 5-level runtime tail; names `applyRuntimeVisualState` for head states + consumer-wins arbitration (matches `ControlRuntime.fsi`). |
| FR-002 | `src/Controls/ControlRuntime.fs:206-207` | Dead `Selected`-from-`Selection` branch carries a forward-looking note: live host never populates `Selection`; only consumer-set `Selected` fires. (Annotated, **not** removed — recorded decision.) |
| FR-003 | roadmap §10.4 | Names a computed-`Bounds` cache keyed by `LayoutNodeId`; **no** "intrinsic-size memo keyed by retained identity"; cross-refs feature 101's deferral. |
| FR-004 | `src/Layout/Layout.fs:7-12` | Comment now states **both** the INV-1 correctness motive **and** the maintainer's blast-radius approval ("blast-radius nil, Controls integer geometry unaffected"). |
| FR-005 / FR-006 | `src/Controls/Focus.fs:127-129`; roadmap §11.5 / parity row | `Chart`/`Graph`/`Progress` branch annotated classed-but-not-routed-by-default (no `NavRange` in `defaultFor`); every "segmented" mention corrected (no `Segmented` `AccessibilityRole`). |
| FR-007 | `src/Controls/Control.fs:1131` | `Key ?? Kind` id annotated as the legacy 080 single-control preview/layout path, distinct from R3's `Key ?? path` dispatch/recovery id. |

## 3. Confirm the zero-behavior invariant (FR-008/SC-003/SC-004/SC-005)

- Parity/golden evidence **unchanged** — no row moves (explicitly not the R6 case).
- R1/R2/R4/R5 property and unit suites (Controls / Elmish / Layout) **green and unmodified**
  — a *moved* test would signal a comment was parsed as a behavior token (FR-010 violation).
- Arrow-key routing for `Chart`/`Graph`/`Progress` **unchanged** (still not routed) — the
  navigation suite passes without modification.
- **No** public `.fsi` / surface-baseline delta (annotate/document defaults) → no recapture.
- `EvidenceAudit` verdict present with **0 synthetic** tasks.

## 4. Decisions to confirm are recorded (SC-006)

- FR-002 remove-vs-annotate → **annotate** (rationale in `research.md`).
- FR-005 document-vs-drop → **document/annotate** (rationale in `research.md`).
