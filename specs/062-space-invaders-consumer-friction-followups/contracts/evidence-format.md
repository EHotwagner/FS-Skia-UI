# Contract: Evidence-Format Recoverability (FR-004/005/007)

**Surface type:** `FS.Skia.UI.Build` governance output (diagnostics + a generated
reference doc + DAG render). No public `.fsi` change. Single-sourced from the
enforcing constants so diagnostics, reference, and validator cannot diverge.

## C1 — `Dev` self-describing output (FR-004)

`build/Governance/Engine/Update.fs` (`StartTarget Targets.Dev`): the
`dev-verdict.txt` content **and** a console line MUST state that `Dev` writes
logs/markers and does **not** compile, and that `Test`/`Verify` (`dotnet test`)
is the authoritative compile/test path.

**Acceptance (SC-004):** `./fake.sh build -t Dev` output and `dev-verdict.txt`
both contain the caveat.

## C2 — Per-class schema-printing diagnostics (FR-005)

For each failing format class, the audit/graph diagnostic MUST print the complete
required shape (extends the 061 `Required`-token pattern on the readiness-contract
scan):

| Class | Source file | Must print |
|---|---|---|
| readiness-contract | `Scans.fs` (`readinessContract`) | full `Required` token list per file (already) |
| skill-loading-evidence | `Audit.fs` (`validateSkillLoadingEvidence`) | 8-column row schema; `loaded_at < work_started_at`; `.agents/skills/<id>/SKILL.md` path |
| window-visibility / `diagnostic-class` | `Scans.fs` (`windowVisibility`) | required `key=value` keys per file; `diagnostic-class` ∈ {environment-session, window-visibility, app-lifecycle, product-defect} |
| seh-acceptance | `TaskParser.fs` | tokens `accepted-seh`, `synthetic-error-handling-approved` (no backticks) |

## C3 — Generated reference doc (FR-005)

`template/base/docs/evidence-formats.md` MUST be **generated** from the same
schema constants (ApiSurfaceGen-style) and **currency-checked** (D12) — listing,
per file, its name, required tokens/columns, ordering rules, and resolved-path
pattern. Authors recover the shape **before** triggering a failure.

## C4 — Effective-DAG render (FR-007)

`Render.taskGraphMd` / `renderMermaid` MUST render explicit deps **and** the
auto-injected Phase N+1 → Phase N checkpoint edges (from `Graph.allDeps` /
`TaskParser` `PhaseDeps`), with **injected edges distinctly labeled**, and print
the **resolved `skillist`-id set**, alongside the existing `graphVerdictLine`.

## Acceptance (SC-002)

In a freshly generated project with no passing sibling, every format class reaches
a passing `EvidenceAudit` using only the audit/graph output and/or
`evidence-formats.md` — **no `strings -el FS.Skia.UI.Build.dll`, no sibling copy.**
Logged in `readiness/readiness-recoverability.md`.
