---
phase: implement
date: 2026-06-08
severity: major
---

## Process friction
Implementation itself was smooth: the scaffold-swap contract
(`docs/scaffold-map.md` + `data-model.md` + the three `contracts/*.md`) was precise
enough that `Model.fs`/`View.fs`/`BehaviorTests.fs` could be written wholesale and the
three durable files (`LayoutEvidence.fs`, `EvidenceCommands.fs`, `Program.fs`) re-pointed
in one pass — the product compiled after two small, predictable collisions (the
`CellContent.Text` ⇄ `Scene.Text` constructor shadow, solved by renaming to `Textv`
to match `Numberv`/`Formulav`; and `ViewerKeyDirection.KeyDown` ⇄ `KeyboardMsg.KeyDown`,
solved by qualifying). All 32 tests passed and SC-007 was provably byte-for-byte.

The **major** friction was the `EvidenceAudit` merge gate, which cost far more time than
the feature code:

1. **`base_ref` never resolved** (stayed `null`) even after committing the feature so
   `main` was a strict ancestor of `HEAD`. The diff-scan therefore reported 0 hits and
   the real blockers came entirely from the window-visibility validator — but the only
   summary surfaced was `total-blockers=5` with `unaccepted-synthetic-tasks=0`, which
   actively misled toward the (empty) diff-scan. The actionable detail lived only in the
   per-scan JSON sidecars (`window-visibility-hits.json` etc.), which are not mentioned
   in the audit command doc.
2. **The engine (0.1.82-preview.1) requires more window-visibility readiness files than
   `docs/evidence-formats.md` documents.** The doc lists `interactive-visible-window.md`
   and `window-state-diagnostics.md`; the engine additionally hard-requires
   `close-reason-separation.md`, `window-options.md`, `generated-validation.md`, and a
   feature-local `evidence-audit.md`, plus token sub-checks (`window-state-diagnostics.md`
   needs the raw `native-handle/visible/focusable/renderable-surface/input-devices` facts;
   `generated-validation.md` needs `exact-package-match/generated-tests-ran/authoritative/
   failure-class`). These were discoverable only by reading the per-file `reason`/`missing`
   arrays in the hit JSON and by extracting UTF-16 string literals from
   `FS.Skia.UI.Build.dll`. `docs/evidence-formats.md` is stamped "do not edit by hand;
   regenerate with RefreshSurfaceBaselines" — it has drifted behind the shipped engine.

What would have helped: (a) the audit summary echoing the non-empty hit-file paths and a
one-line per-blocker reason to stdout; (b) `docs/evidence-formats.md` regenerated against
0.1.82 so the window-visibility contract (6 files + their token sets) is recoverable
before triggering a failure, per that doc's own stated purpose (FR-005).

## Generalizable code
- **Pure recursive-descent formula parser + Kahn topological recompute with cycle→#ERR**
  in `Model.fs` is a clean, dependency-free pattern (matches `fsharp-parsing` "regex/parse
  port" and `fsharp-graph-algorithms` "hand-roll, stable-sorted Kahn" verdicts). It is
  product-specific here, but the *shape* (parse→AST→topo-evaluate→propagate-error) is a
  reusable recipe worth a short cookbook entry in `fsharp-graph-algorithms` (it already
  owns topo sort; adding "evaluate-in-topo-order with error propagation" would round it out).
- The `KeyCommand` intermediate type that unifies the raw-`RawKey` path and the normalized
  `ViewerKey` host path into one pure `applyCommand` is a tidy way to honor research R1
  (printable from `RawKey`) while keeping the live `MapKey: ViewerKey -> bool -> Msg`
  boundary — a candidate note for `fs-skia-keyboard-input`.

## Skill gaps
none — every declared capability skill resolved and applied. The gap was in the
**evidence-gate documentation** (`docs/evidence-formats.md` window-visibility section),
not in a capability skill. `speckit-evidence-audit`'s command doc could also note that
window-visibility/readiness-contract validators run in addition to graph+diff-scan, and
that detail lives in the `*-hits.json` sidecars.

## Research links
research blocked — offline; the engine contract was recovered by reading the shipped
`.fsi`/`docs/api-surface` (authoritative) and the per-scan hit JSON, not external docs.
