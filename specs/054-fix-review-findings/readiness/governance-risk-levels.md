# Governance Risk Levels — Feature 054 (Fix Review Findings)

This feature's change set and the validation each risk level requires.

## Risk levels

- **small** — a single framework-internal `src/**`/`build/**` edit with no
  consumer-contract surface; focused validation is `Dev` only.
- **medium** — touches `template/**`, governance `build/Governance/**`, a
  governance test, and `.gitignore`. **This feature is medium.** `Route`
  escalates to the maintainer-verify path.
- **broad** — public `.fsi`/surface-baseline change, new dependency, or a
  runtime/graphics behaviour change. Not applicable here (Tier 2, internal).

## Required evidence for the selected level (medium)

The **required evidence** for this medium change is the focused set:

- the strengthened pin-parity assertion run under
  `TemplateCheck` / `GeneratedProductCheck`
  (`GeneratedProjectValidationTests` — exact `#r` vs props version equality),
- a clean `--no-incremental` FS3261 count (before **88** raw emissions across
  33 distinct sites in 8 files → after **0**) with the escape hatch removed so
  the compiler enforces it,
- a `git status --porcelain` clean-tree check after the stray scratch removal.

## When broad validation is required

**Broad validation** (the full serialized six-target order: `Dev` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`) is required at integration (Phase 6) because
the change set touches template/governance/contract paths and `Route`
escalates. It is also required whenever a change reaches `broad` risk (public
surface, new dependency, runtime behaviour).

## Non-authoritative aggregate FAKE results

FAKE-backed commands share `.fake` state and are never run concurrently. When an
aggregate (`Dev`) run is used, its result is recorded as **non-authoritative**;
any race-like or environment failure is rerun in focused isolation as the
authoritative result.
