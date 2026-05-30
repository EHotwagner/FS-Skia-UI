# Phase 0 Research: Fail-Loud Authoring & Audit Robustness

All NEEDS CLARIFICATION items from Technical Context are resolved below. Each
decision records what was chosen, why, and the alternatives evaluated.

## R1 — Authoritative active-feature source (US1, FR-001/FR-002/FR-003)

**Decision**: `.specify/feature.json` `feature_directory` is the single
authoritative source for the feature under audit. Remove the hardcoded
`"007-v2-template-packaging"` fallback in `activeFeatureId` (`build.fsx`
~line 284). On missing file, missing key, unparseable value, or empty value,
the resolver **hard-fails** with a prominent message naming the expected source
and the failure cause, rather than returning a placeholder feature id.

**Rationale**: The false green came from `activeFeatureId` returning a *valid but
wrong* feature id when `.specify/feature.json` was absent/unreadable. Every
fallback branch (`markerIndex < 0`, `colonIndex < 0`, missing quotes,
whitespace) returns the same hardcoded id, so the audit silently ran against a
different feature. An unresolved feature must never be a passable state
(Clarification: hard-fail). The current code already proves `feature.json` is
the intended source (it parses it first), so making it authoritative is a
tightening, not a new mechanism.

**Implementation notes**:
- `run-audit.sh` already `die`s (exit 4) when the feature-dir arg is missing or
  not a directory — that layer is correct. The defect is upstream in `build.fsx`
  choosing *which* dir to pass.
- `compute-task-graph.py main()` already errors when `len(argv) != 2`; it will
  receive whatever dir `build.fsx` resolves. Add an echo of the resolved feature
  id and parsed task count so a wrong-feature mismatch is visible in the log
  (FR-003), and surface a recorded-vs-scanned mismatch (US1 scenario 3).
- Keep `common.sh get_feature_paths` resolution order (`SPECIFY_FEATURE_DIRECTORY`
  env → `feature.json` → branch-prefix) but ensure the terminal "nothing
  resolved" state is a failure, not a silent stub. The two resolvers
  (`build.fsx` and `common.sh`) must agree on the authoritative source.

**Alternatives considered**:
- *Warn-and-continue against the stub* — rejected in Clarifications; a green
  pass against a placeholder is the most dangerous failure in the set.
- *Branch-name as authoritative source* — rejected: branches can drift from the
  recorded feature; `feature.json` is the explicit recorded state and the spec's
  stated assumption.

## R2 — Structured status region format and resolution rule (US2, FR-004/FR-005)

**Decision**: Machine-readable status values are read **only** from a designated
fenced code block whose info string declares the audit-status language (e.g.
```` ```audit-status ````). `key=value` lines are parsed only inside such a
region. Resolution rule (deterministic, documented for authors):

1. The authoritative value for a key is taken from the first declared
   `audit-status` region in the file, scanned top-to-bottom.
2. A duplicate key **within** the authoritative region is a parse error
   (surfaced, not silently last-wins).
3. Occurrences of the same key in prose, markdown bullets, or unlabeled/other
   fenced blocks are never read as status — they cannot override the region.
4. A present-but-malformed structured key is surfaced as a parse error, never
   silently treated as passing or failing (Edge Case).

The bare substring blockers are removed: `"taskbar-only" in text`,
`"mismatch" in text`, `"nu1603" in text`, and `exact not in {true,yes}` over raw
prose (`run-audit.sh` lines ~513, ~672–731, ~874). Blocking is driven by
explicit structured fields with violating values (e.g.
`taskbar-only=true`, `exact-package-match=false`, `package-resolution=nu1603`).

**Rationale**: The three observed false blocks all came from substring matching
honest prose — a sentence saying a claim is *not* taskbar-only, and a markdown
bullet mentioning a key name. `parse_key_values` today scans the whole file, so
even a `key=value` inside a prose bullet is read. Restricting reads to a labeled
fenced region gives authors a stable place to declare status and frees prose to
discuss governance concepts without tripping the scanner (FR-004/FR-005). FR-006
is preserved because the genuine-violation fixture declares its violating value
*inside* the region and still blocks.

**Implementation notes**:
- Add a region extractor: collect lines between a ```` ```audit-status ````
  fence and its closing fence; feed only those to `parse_key_values`.
- Replace each substring blocker with a structured-field check on the value
  parsed from the region.
- Document the rule in both `speckit-evidence-audit` SKILL.md peers and in the
  `audit-status-region-contract.md`.

**Alternatives considered**:
- *YAML front-matter block* — rejected: heavier, and the audit's parsers
  deliberately avoid a full YAML dependency; a fenced `key=value` block reuses
  the existing `parse_key_values` shape.
- *Last-textual-occurrence wins* — rejected by FR-005 scenario 4: must be
  deterministic and not "last wins"; first-region-wins + duplicate-is-error is
  unambiguous.
- *Keep substring scan as a secondary signal* — rejected: it is the exact source
  of the false blocks; a secondary substring pass would reintroduce them.

## R3 — RequireQualifiedAccess blast radius (US3, FR-007)

**Decision**: Add `[<RequireQualifiedAccess>]` to `ControlEventOrigin` only
(`src/Controls/Types.fs` line ~181, `.fsi` line ~200). Before landing, scan the
repo, tests, generated template content, and FSI fixtures for unqualified uses
of its cases (`Pointer`, `Keyboard`, `Text`, `Focus`, `Selection`, `Clipboard`)
and qualify them as `ControlEventOrigin.X`.

**Rationale**: The collision is the `Text` case leaking into the open namespace
and shadowing the Scene `Text` constructor, producing the opaque "value is not a
function / has type ControlEventOrigin" diagnostic. Qualified access stops the
leak at the source. The type's sibling DUs (`KnownControl`, `KnownEvent`,
`StandardControlKind`, etc.) already carry the attribute, so this aligns
`ControlEventOrigin` with the established Controls convention rather than
inventing a new pattern. Scene DU constructors and `LayoutBounds` stay
guidance-governed because they did not produce a false error and a documented
workaround exists (Clarification + FR-007/FR-008).

**Implementation notes**:
- Refresh `readiness/surface-baselines/FS.Skia.UI.Controls.txt` and the merged
  `FS.Skia.UI.txt` via `scripts/refresh-surface-baselines.fsx`; `Tags` nested
  type entry remains.
- Record the reversal of spec 035's `decision: consumer-guidance` /
  `compatibility: no-contract-change` for this one type in
  `specs/035-api-discovery-names/readiness/name-collision-safety.md` (FR-010).
- Add a fixture compiling the previously-failing open order (Controls opened
  after Scene) that constructs a scene text node unqualified.

**Alternatives considered**:
- *Guidance-only (spec 035 choice)* — already tried; insufficient (it cost real
  debugging time and produced an opaque error). FR-010 documents the reversal.
- *Attribute on Scene DU + bounds too* — out of scope per Clarification; only
  `ControlEventOrigin` is the confirmed collision source. Over-applying the
  attribute would be a broader contract change without evidence of need.

## R4 — FSI load script generation strategy (US4, FR-009)

**Decision**: Emit a static generated `.fsx` load script alongside the generated
`Product` app at generation time (`GenerateV3Products` in `build.fsx`, sourced
from `template/base/`). The script `#load`s/`#r`s the generated `Product` output
assembly plus its transitive `FS.Skia.UI.*` references, derived from the pinned
set in the generated `Directory.Packages.props` (and, after restore,
`project.assets.json`). Because it is generated from that set rather than
hand-written, it stays in sync with the app's assembly set.

**Rationale**: The author hit seven transitive references with no guidance. A
generated script removes the hand-enumeration entirely and, being derived from
the same pinned source the build uses, cannot drift the way a hand-maintained
reference list would (FR-009). Static emission keeps the script copy-pasteable
and inspectable, and lets `GeneratedGuidanceCheck`/`GeneratedProductCheck`
validate its `#r` set against the resolved packages.

**Implementation notes**:
- Filename/shape fixed in `fsi-load-script-contract.md`.
- Generation pulls versions from `Directory.Packages.props`; transitive set is
  confirmed against `tests/Product.Tests/obj/project.assets.json` after restore.
- Benign host-warning classification (spec 021 host-warning contract; GTK module
  load warnings on Linux) stays intact — the load path must not suppress real
  LaunchFailure/RenderingFailure/PackageFailure diagnostics, only the known
  benign environment warnings, and only when launch/first-frame succeed.

**Alternatives considered**:
- *Docs snippet the author copies* — rejected in Clarifications: a hand-followed
  snippet drifts when the assembly set changes; the generated script auto-stays
  in sync.
- *Runtime reflection to discover assemblies* — rejected: the generated guidance
  scan explicitly discourages assembly-reflection guidance, and a static set
  derived from the pinned manifest is simpler and inspectable.
