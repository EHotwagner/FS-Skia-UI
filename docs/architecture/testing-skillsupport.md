---
title: Testing & SkillSupport
category: Architecture
categoryindex: 3
---

# Testing & SkillSupport

`FS.Skia.UI.Testing` and `FS.Skia.UI.SkillSupport` are the two FS.Skia.UI
distribution packages that hold *helper* code rather than runtime rendering code.
`Testing` is the validation and evidence layer: it turns a free-form check ("did
the generated product launch?", "is this screenshot a real proof?") into a typed
request/result pair you can assert on. `SkillSupport` is a small, dependency-light
toolbox of generic algorithms — DAG ordering, governance-input parsing, globbing,
and deterministic document generation — that backs the repository's `fsharp-*`
authoring skills and is reusable in any consumer project. They share this page
because both are framework *support* libraries: neither draws to the screen, both
are pure-leaning, and both are pinned and versioned with the rest of the
`FS.Skia.UI.*` set. For the precise signatures, see the
[API reference index](../reference/index.html).

## FS.Skia.UI.Testing — what it does

`Testing` answers one question repeatedly: *given some captured facts, is a claim
about a generated FS.Skia.UI product acceptable?* The package is deliberately
shaped as **pure decision functions** over plain records. Each helper takes a
`*Check` (or `*Request`/`*Expectation`) record describing the observed facts and
returns a `*Result` record carrying an accept/reject verdict plus diagnostics — so
the caller (a test, or a governance gate) does the I/O and `Testing` does the
judging.

### Generated-product and package expectations

The starting point is describing what a scaffolded product should look like.
`GeneratedProductExpectation` lists the required files, forbidden path prefixes,
and expected package references for a profile;
`GeneratedProductAssertions.summarize` renders it for an evidence log, while
`validateDefaultInteractiveLaunch` and `validateWindowDiagnostics` check launch
and window behaviour against captured output.

Pinned-package drift is its own concern. `LocalConsumerPackage` /
`LocalConsumerPackageDrift` and the `LocalConsumerPackages` module
(`report`, `classifyDrift`) compare an expected local-feed package set against the
actual one and emit a per-package remediation command when a version mismatches —
this is how a generated project's single `<FsSkiaUiVersion>` pin is held honest.

See the namespace overview at
[`../reference/fs-skia-ui-testing.html`](../reference/fs-skia-ui-testing.html),
and the type pages
[`../reference/fs-skia-ui-testing-generatedproductexpectation.html`](../reference/fs-skia-ui-testing-generatedproductexpectation.html)
and
[`../reference/fs-skia-ui-testing-localconsumerpackagedrift.html`](../reference/fs-skia-ui-testing-localconsumerpackagedrift.html).

### Consumer validation and the validation contract

`GeneratedConsumerValidation` assembles the broad generated-product contract.
`verifyPackageResolution`, `verifyGeneratedTests`, `selectVisualEvidence`, and
`validateVisualEvidenceCommandOutput` check the individual stages;
`buildValidationContractOutput` folds the package-resolution, generated-test,
default-launch, bounded-evidence, close-reason, window-diagnostic, window-options,
and image-evidence sub-results into one `GeneratedValidationContractResult` with a
single `FailureClass` and an `Authoritative` flag. The `Authoritative` /
`NonAuthoritativeReason` distinction matters: a check that could not run in the
current host (for example, no display) reports a non-authoritative result rather
than a false pass.

### Evidence reports and screenshot proof

The `EvidenceReports` module is the heart of FS.Skia.UI's "visual-proof honesty"
discipline (see [docs/reports/evidence.md](../reports/evidence.html)). It can
`build` and `write` a structured `EvidenceReport` (status, command, output path,
named `Fields`, lines, exit code), `validate` it, and — importantly — distinguish
*kinds* of evidence:

- `parseScreenshotEvidenceRecord` reads a screenshot evidence record back from
  its emitted lines.
- `validateScreenshotArtifact` decodes the PNG and rejects missing, unreadable,
  out-of-readiness, dimension-mismatched, or **blank** images.
- `validateScreenshotEvidence` enforces the full record contract: a record only
  counts as screenshot proof when it records live viewer-window capture after
  first-frame presentation, `proves-screenshot=true`, a readiness-local PNG,
  positive decoded dimensions, and non-blank pixel validation.

Layered on top, `DefaultTextGlyphEvidence.validate` checks rendered-text coverage
(glyph-coverage / solid-block / placeholder metrics) so that "the text rendered"
is not satisfied by a solid block or tofu placeholders. `HostWarningClassification`
(`HostWarningClass`, `classify`) sorts a raw host warning into benign vs.
launch / render / layout / package failure, which is what keeps an unsupported-host
environment warning from being mistaken for a product defect.

The remaining modules round out readiness checking:
`GeneratedLayoutValidation.validate` checks HUD/gameplay layout bounds from a
`LayoutEvidenceReport` (a `Scene` type — note this package depends on
`FS.Skia.UI.Scene`); `PersistentLaunchArtifactValidation.validate` checks a
persisted graphical-launch artifact for missing fields and contradictions; and
`ReadinessFileDiscovery.validate` confirms the required readiness files exist.

For the full set, see
[`../reference/fs-skia-ui-testing-evidencereport.html`](../reference/fs-skia-ui-testing-evidencereport.html)
and
[`../reference/fs-skia-ui-testing-hostwarningclassificationresult.html`](../reference/fs-skia-ui-testing-hostwarningclassificationresult.html).

### How the helpers are used

The intended pattern is: a FAKE governance gate (or an Expecto test) does the
real work — restore, launch, capture a screenshot — collects the facts into the
appropriate `*Check` record, calls the matching `validate`/`classify`/`build`
function, and asserts on the returned verdict. Because the decision is a pure
function over a record, the same logic is exercised directly in
`tests/Governance.Tests` without needing a display or a live process. The repo's
own screenshot evidence is validated through exactly these functions; see the
"Screenshot Evidence Validation" section of
[docs/reports/testing.md](../reports/testing.html).

## FS.Skia.UI.SkillSupport — what it does

`SkillSupport` is the shipped backing library for the `fsharp-*` authoring skills
(graph algorithms, parsing, globbing, code generation, shell process). Its design
rule is **dependency-light and pure where possible**: the geometry/RNG helpers
take plain `float`/`uint64` rather than `Scene` types so the package adds no heavy
dependencies, and each module's visibility lives in its `.fsi` (Principle II). The
governance engine in `build/Governance/**` is one consumer; a generated game
product threading a deterministic RNG through its Elmish `update` is another.

### Graph — DAG ordering and cycle detection

`Graph` is the generic DAG core. `topoSort` is a Kahn topological sort with a
deterministic ascending-`NodeId` tie-break, returning `Ok order` or
`Error remaining` (the nodes that could not be ordered because they sit in or
depend on a cycle). `detectCycle` is a 3-colour DFS returning a single cycle
witness (`[a; b; c; a]`) or `None`. The governance synthetic-propagation rule
that powers `EvidenceGraph` is a downstream consumer of this same core.

### Parsing — typed governance-input reads

`Parsing` provides `readYaml<'T>` (YamlDotNet) and `readJson<'T>`
(System.Text.Json + FSharp.SystemTextJson, so F# records and unions deserialize),
both `Result`-returning, plus `matchLines`, which compiles a regex once and
applies it as a line grammar yielding `(lineIndex, Match)`. Consumers keep their
own exact `tasks.md` / `tasks.deps.yml` grammars and call these utilities.

### Globbing — discovery and currency diff

`Globbing.isMatch` is fnmatch-style matching where `**` crosses `/` and `*`/`?`
stay within a path segment; `discover` enumerates files under a root matching any
glob and returns sorted relative paths; `currencyDiff` is a DiffPlex-based
generation-currency check — an empty result means the on-disk artifact is current.
This is the basis of the repo's "generated, not hand-synced" currency gates.

### CodeGen — deterministic document builders

`CodeGen.mermaidGraph`, `markdownTable`, and `asciiTree` build Markdown / Mermaid /
ASCII output via plain `StringBuilder` assembly — no code quotations, no reflection
(Principle III) — so generated docs are byte-deterministic. Consumers render their
specific layouts on top of these primitives.

### ShellProcess, Hud, Wrap, Random

`ShellProcess.run` is a captured external-process runner returning a `ProcResult`
(`ExitCode`/`StdOut`/`StdErr`) with arguments passed as a quoted list (no shell
interpolation); `git` is a thin wrapper. The remaining three are the recurring
arcade/game helpers: `Hud.reserveHudBand` partitions an axis into a fixed HUD band
and a clamped gameplay remainder; `Wrap.wrapDeltaX` is the shortest wrap-aware
signed delta on a toroidal axis; and `Random` is a deterministic, replayable
seeded RNG (`seedRng`/`nextRng`/`nextBelow`) with a `private`-representation
`RngState` you thread through a pure `update`.

See
[`../reference/fs-skia-ui-skillsupport-graph.html`](../reference/fs-skia-ui-skillsupport-graph.html),
[`../reference/fs-skia-ui-skillsupport-parsing.html`](../reference/fs-skia-ui-skillsupport-parsing.html),
[`../reference/fs-skia-ui-skillsupport-globbing.html`](../reference/fs-skia-ui-skillsupport-globbing.html),
[`../reference/fs-skia-ui-skillsupport-codegen.html`](../reference/fs-skia-ui-skillsupport-codegen.html),
and the full
[API reference index](../reference/index.html).

## Analysis

### Implementation strengths

- `Testing` helpers are pure decision functions over plain records (`*Check` in,
  `*Result` out), so they are trivially unit-testable in `tests/Governance.Tests`
  without a display, a process, or any I/O — the caller does the side effects and
  the package only judges.
- The evidence layer encodes real honesty rules in code rather than prose:
  `validateScreenshotArtifact` actually decodes the PNG and rejects blank or
  dimension-mismatched images, and `validateScreenshotEvidence` requires
  `proves-screenshot=true` with a non-blank readiness-local artifact, so a metadata
  or fallback record cannot masquerade as screenshot proof.
- `SkillSupport` is genuinely dependency-light: `Hud`/`Wrap`/`Random` take plain
  `float`/`uint64` instead of `Scene` geometry, and `Random.RngState` has a
  `private` representation, so a consumer's Elmish `update` stays pure and
  replayable with no `Scene` dependency dragged in.
- Single algorithm homes: the same `Graph.topoSort`/`detectCycle` that ships in
  `SkillSupport` backs the governance evidence-graph propagation, so DAG behaviour
  is implemented and tested once rather than duplicated per consumer.

### Implementation weaknesses

- `ScreenshotEvidenceReportCheck` carries roughly two dozen mostly-`option`
  fields, and several `Testing` results overlap (`*FailureClass` is sometimes a
  typed union, sometimes a `string option`), so the surface is broad and a caller
  must read the `.fsi` carefully to know which fields a given verdict actually
  consults.
- Acceptance contracts (e.g. the exact field set a screenshot record must contain)
  live partly in the validators and partly in the prose of
  [docs/reports/evidence.md](../reports/evidence.html); the two can drift because
  nothing mechanically ties the documented field list to the validated one.
- `Testing` depends on `FS.Skia.UI.Scene` (for `LayoutEvidenceReport`/`Rect`),
  so a consumer wanting only package-drift or evidence-report helpers still pulls
  in the scene package.
- `SkillSupport.ShellProcess` and `Parsing` perform real I/O and deserialization;
  their failure paths (process spawn errors, malformed YAML/JSON) are surfaced as
  `Result`/captured output but are inherently harder to exercise than the pure
  modules, leaving thinner natural test coverage there.

### Design pros

- Splitting *judging* (`Testing`, pure) from *doing* (the gate/test that captures
  facts) makes verdicts deterministic and reproducible, and lets the same contract
  run identically in CI and locally regardless of host capabilities via the
  `Authoritative`/`NonAuthoritativeReason` channel.
- Shipping `SkillSupport` as a real, versioned package — rather than burying the
  helpers inside the build scripts — means generated consumer products and the
  authoring skills draw from the same audited implementations.
- Both packages move on the single shared `<FsSkiaUiVersion>` pin, so a consumer
  upgrades the support libraries in the same one-line edit as the rest of the
  framework, with `LocalConsumerPackages.classifyDrift` available to catch a stray
  mismatch.

### Design cons

- Pairing a validation/evidence library with a generic algorithm/util library
  under one "support" banner is a coherence compromise: they share no domain, and
  a consumer who wants the toroidal-wrap helper has no need of screenshot
  validation (and vice versa), so the grouping is organizational, not conceptual.
- The "support" framing understates how much *policy* `Testing` actually encodes
  (what counts as a valid proof); that policy is consequential and could justify
  its own clearly-named governance/evidence package rather than living beside
  arcade math helpers.
- Pure-decision-function design pushes all the hard, flaky work (launching,
  capturing, decoding host warnings) onto callers, so the packages can look
  reassuringly green while the genuinely fragile integration lives — and must be
  tested — elsewhere.
