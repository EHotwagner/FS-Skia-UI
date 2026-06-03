# Governance Risk Levels — Feature 055

Risk classification for the decoupling of author-guidance prose from
generation-currency anchors (Tier 2, governance-internal).

## Levels

- **small** — a single framework-internal edit (e.g. one `build/Scene/**` file)
  that routes to the inner-loop tier (`Dev` only). Not this feature.
- **medium** — `build/Governance/**` + `tests/**` logic changes plus governed
  guidance prose under `.specify/**`. The **required evidence** for the medium
  level is the focused validation set below.
- **broad** — consumer-contract or build-target changes (`build.fsx`,
  `scripts/build/**`, `validation.contract.yml`) that escalate to the
  maintainer-verify tier and require **broad validation** (the full serialized
  six-target order).

## This feature

Selected level: **medium → broad at integration.** The diff spans
`build/Governance/**` + `tests/**`, governed guidance under `.specify/**`, and
`docs/**` baseline records, so `Route` escalates.

- **required evidence** (focused): the new pure-core unit tests — US1 rewording
  PASS, US2 drift FAIL, the twin-coverage drift case, SC-004 token-removal FAIL,
  and the FR-006 stale-term FAIL — plus the real-repository
  `GeneratedGuidanceCheck` regression (SC-006).
- **broad validation** is required at integration (Phase 6): the serialized
  order `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.

## Aggregate FAKE results

Aggregate FAKE runs are recorded as **non-authoritative**; any race-like failure
is rerun in focused isolation as the authoritative result (FAKE shares `.fake`
state and is never run concurrently).
