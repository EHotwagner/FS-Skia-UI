# Phase 0 Research: Controls Authoring API Discoverability

All Technical-Context unknowns are resolved below. No `NEEDS CLARIFICATION` remains.

## D1 — What counts as "the Controls authoring surface" the gate covers

**Decision**: The `ControlsDocCoverageCheck` gate covers **every public member of
every `src/Controls/**/*.fsi` file** — the whole Controls package public surface
(186 placeholder summaries across 13 files today) — not a hand-picked authoring
subset.

**Rationale**: A fuzzy "authoring vs. runtime" boundary would be arguable and would
let boilerplate persist on members a consumer still sees in IntelliSense (e.g.
`Diagnostics`, `ControlRuntime`). A single flat rule — *no placeholder/empty summary
anywhere on the Controls public surface* — is enforceable, unambiguous, and satisfies
FR-005's "at minimum" set by superset. The typed `Widgets/*.fsi` are already
well-documented, so they pass for free and act as the positive exemplar.

**Alternatives considered**: (a) Only `Control.fsi` + `Attributes.fsi` + `Catalog.fsi`
— rejected: leaves Charts/DataGrid/RichText authoring boilerplate. (b) All `src/**`
packages repo-wide (390 occurrences) — rejected as this feature's scope; deferred as a
bounded follow-up (plan "Deferred scope").

## D2 — What the gate flags (the violation predicate)

**Decision**: A public `.fsi` member is a **violation** when its associated `///`
summary is any of:
1. **Placeholder** — contains the known boilerplate sentence
   `Public contract function exposed by this FS.Skia.UI package.` (substring match,
   whitespace-normalized).
2. **Empty** — the member has no `///` summary at all, or a summary that is only
   whitespace.
3. **Duplicate-only** — the *exact same* summary text is shared by ≥ N members within
   a file in a way that indicates a mechanically-reworded placeholder. Practical rule:
   flag a summary string that is identical across ≥ 8 members **and** carries no
   member-specific token (no backticked identifier, no parameter/value description) —
   this catches "rename the boilerplate" evasions (edge case in spec) without
   penalizing legitimately terse repeated summaries that name their member.

**Rationale**: Matches the three failure modes in the spec's US2 + edge cases. The
duplicate-only rule is the anti-evasion guard; it is deliberately conservative (high
threshold + "no specific token") so it does not fire on honest short docs.

**Alternatives considered**: AST/XML-based coverage via the compiled
`FS.Skia.UI.Controls.xml` — rejected for the gate's *primary* path: gates in this repo
read `.fsi` text directly (`PerPackageSurface`, `CatalogDocsGen` precedent) and `.fsi`
is the single source of the `///` text, avoiding a build-order dependency on the
compiled `.xml`. The `.xml` is still the *shipping* vehicle (it's what reaches
IntelliSense); the gate asserts on the source the `.xml` is generated from.

## D3 — How the gate reads the surface

**Decision**: New module `build/Governance/ControlsDocCoverage.fs` enumerates
`src/Controls/**/*.fsi` via `FS.Skia.UI.SkillSupport.Globbing`, parses each file
line-by-line associating a leading `///` summary block with the next `val`/`type`/
`member` declaration (regex line grammar, reusing the `Parsing` helper style), and
returns `DocFinding list`. Pure function; I/O (file reads, report write) happens at the
`Engine/Interpret.fs` edge, exactly like `PerPackageSurface.diff`.

**Rationale**: Reuses shipped SkillSupport helpers (no new dependency, Constitution
III simplicity), follows the established gate pattern, keeps the analysis pure and
unit-testable with a planted-placeholder fixture.

**Alternatives considered**: Reuse `PerPackageSurface.normalize` (it *strips* comments,
the opposite of what we need) — rejected; we need to *read* the comments, so a small
purpose-built line scanner is correct.

## D4 — Wiring a new gate (precedent: DesignTokenDrift)

**Decision**: Follow the `DesignTokenDrift` precedent exactly — edit `Targets.fs`
(add `ControlsDocCoverageCheck` to the `Target` DU, `allTargets`, `name` map,
`directPrerequisites` = `[]`, `routableGates`), add the gate to the
controls-public-surface rule in `Routing.fs`, implement `ControlsDocCoverage.fs`, wire
its effect in `Engine/Update.fs` + `Engine/Interpret.fs`, and **regenerate**
`validation.contract.yml` via `./fake.sh build -t RefreshSurfaceBaselines` (never hand
-edit it; `TargetMetadataDrift` enforces currency). A mistyped gate name is a compile
error (closed DU).

**Rationale**: The governance home is the single source of rules and is generated, not
hand-synced; the precedent is proven and recent.

## D5 — Surface-baseline stability under doc-only `.fsi` edits

**Decision**: Adding/replacing `///` comments on the Controls `.fsi` does **not** churn
the per-package surface baseline (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`),
because `PerPackageSurface.normalize` strips `//`-prefixed lines (which includes `///`)
before diffing. The **api-surface bundle** (`template/base/docs/api-surface/Controls/*.fsi`)
copies `.fsi` verbatim *with* comments, so it WILL change and MUST be regenerated by
`RefreshSurfaceBaselines`; `ApiSurfaceGen.currency` then passes.

**Rationale**: Confirmed by reading `PerPackageSurface.normalize` and `ApiSurfaceGen`.
This is a load-bearing fact for the evidence plan: the doc rewrite produces a clean
per-package baseline (no signature drift) while still refreshing the consumer-visible
bundle. It also means the surface-baseline gate cannot be used to *prove* the docs
changed — the gate in D2 is what proves coverage.

## D6 — Typed front door is already bundled (scope correction)

**Decision**: Treat FR-004/SC-006 as a **verify-and-keep-current** obligation, not new
enrollment. `template/capabilities.yml` (feature 089) already lists all 14
`src/Controls/Widgets/*.fsi` contracts, and they are already present in
`template/base/docs/api-surface/Controls/`. The work is to (a) confirm currency after
this feature's regeneration and (b) make the **demonstrated** starter use the surface
that is already discoverable.

**Rationale**: Investigation confirmed the bundle already contains the typed surface.
The reflection incident therefore was *not* caused by a missing typed bundle — it was
caused by the starter demonstrating only the legacy stringly API and that legacy
surface being boilerplate-documented. This sharpens US1: the fix is the starter +
README, not new bundling.

## D7 — Making catalog facts consumer-visible (US3)

**Decision**: Surface the per-control catalog to consumers two ways, both reachable
from the generated README:
1. **Programmatic** — document `Catalog.requiredAttributes` / `supportedAttributes` /
   `supportedEvents` / `knownControlKinds` / `markdownSummary` so a consumer enumerates
   a control's contract from IntelliSense and can even render a catalog summary at
   runtime (`Catalog.markdownSummary()` already exists).
2. **Static** — bundle a consumer-visible catalog reference into the generated project
   under `template/base/docs/` (the generated repo already produces per-control
   markdown under `docs/controls/*.md` via `CatalogDocsGen`; the reference bundles or
   points at the same catalog data the package ships in `contentFiles/catalog.yml`).

**Rationale**: FR-010/FR-011 require the README pointer to resolve to *something a
consumer can open*. `catalog.yml` packed only under `contentFiles/` is not obviously
openable; a `docs/`-level reference + the documented `Catalog.*` API closes the gap two
ways (offline file + runtime API), satisfying SC-004.

**Alternatives considered**: Generate a full fsdocs HTML site — deferred (plan
Unsupported scope); heavier than needed and the `.fsi` bundle + catalog reference
already give the consumer everything.

## D8 — Starter migration is behavior-preserving

**Decision**: Rewrite `View.fs` to `Typed` `Props`/`view`, relying on the existing
`tests/Controls.Tests/TypedLoweringTests.fs` parity guarantee (typed `view` lowers
structurally equal to the legacy builder). The starter's rendered output is unchanged;
`GeneratedProductCheck` confirms it compiles + renders.

**Rationale**: The parity tests already prove per-control structural equality for the
keystone controls; the starter uses those controls. No new parity test is required
beyond ensuring the starter's specific controls are covered by the existing suite (add
a case only if the starter uses a control not yet in the parity set).
