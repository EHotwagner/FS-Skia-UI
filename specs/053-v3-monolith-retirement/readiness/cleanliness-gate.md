# Generated-project cleanliness gate (FR-008, SC-005/006)

`GeneratedProductCheck` (`build/Governance/GeneratedProduct.fs`, `scanV3GeneratedRow`)
asserts a generated `app`/`governed` profile is clean: it references the split packages
and carries only its OWN starter content, never copies of the framework's internals.

## Pinned forbidden top-level artifacts (deterministic failure naming)

The forbidden-content loop fails naming the offending artifact as
`{artifact}/{profile} copied {rule}: {path}`:

- `samples/` — framework sample content
- `specs/00` — historical framework feature specs (numbered `specs/00N-…`). The generated
  product legitimately ships a **starter** `specs/generated-evidence-workflow/` (the speckit
  demo feature), so the guard targets the numbered framework dirs, not all `specs/`.
- `readiness/` — framework readiness evidence
- `docs/reports/` — framework documentation/report set (the product's own docs live at
  `docs/product.md`, `docs/effects-boundary.md`, `docs/api-surface/**`)
- `tests/Parity.Tests`, `.template.package`, `src/Charts`, `tests/Charts.Tests`
- a **framework root-README copy** — content guard: the generated `README.md` must not be
  byte-identical to the repository root `README.md` (the path check cannot catch this since
  `README.md` is a required product file)

## Green on a clean generated app

```bash
./fake.sh build -t TemplateCheck        # Status: Ok
./fake.sh build -t GeneratedProductCheck # Status: Ok
```
A freshly generated default `app` references split packages only and carries no `samples/`,
framework `docs/reports/`, historical `specs/00N-`, or framework root-README copy.

## Red on a planted framework artifact (naming the offending artifact)

The forbidden loop was observed failing on a real generated tree with the exact named
diagnostic during gate development:
```
source/app copied historical specs: specs/
```
i.e. when the forbidden glob matched a top-level artifact in the generated `app`, the gate
hard-failed naming both the rule (`historical specs`) and the artifact (`specs/`) — proving
deterministic fail-naming (C3). A deliberate planted-`samples/` template artifact likewise
drove the gate red; the plant was reverted (`template/` git-clean) and the gate returns to
green (`Status: Ok`).

failure class: GeneratedProductUnclean. next action: none — green on the clean app, red and
named on a planted artifact. authoritative command: `./fake.sh build -t GeneratedProductCheck`.
