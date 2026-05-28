# Data Model: Generated Evidence Workflow Authority

## Generated Evidence Command

**Purpose**: Represents a generated project command that claims evidence graph, audit, or guidance validation.

**Fields**:

- `name`: command or target name, such as `EvidenceGraph` or `EvidenceAudit`.
- `scope`: generated project, feature directory, or readiness package being validated.
- `authority`: whether validation was authoritative, delegated, skipped, or placeholder-only.
- `exitCode`: process result.
- `status`: pass, fail, unsupported, or skipped.
- `reportPath`: readiness artifact written by the command.
- `diagnostics`: validation area, message, and failed artifact references.

**Validation Rules**:

- A pass status is valid only when `authority` is authoritative or delegated to authoritative validation.
- Placeholder-only or skipped validation must not produce a pass claim.
- Failure diagnostics must include the command identity and failed validation area.

## Evidence Graph Result

**Purpose**: Captures task graph validation, dependency topology, synthetic propagation, and task skill metadata validation.

**Fields**:

- `featureDirectory`: feature directory under validation.
- `tasks`: task ids, declared status, effective status, phase, story, and title.
- `dependencies`: explicit and phase-injected dependency edges.
- `skillRequirements`: structured `skillist` metadata per task.
- `skillEvidenceCoverage`: coverage result for required skill-loading rows.
- `errors`: cycles, dangling references, missing ids, omitted skill metadata, or invalid rows.
- `warnings`: non-blocking diagnostics.

**Validation Rules**:

- Every task in `tasks.md` must appear in `tasks.deps.yml` and vice versa.
- The graph must be acyclic and have no dangling references.
- Every task must include structured and visible `skillist` metadata.
- Skill-loading evidence must cover every required `(task id, skill id)` pairing when such evidence is required.

## Skill-Loading Evidence Row

**Purpose**: Proves a declared skill was loaded before implementation work began for one task.

**Fields**:

- `taskId`: exact task id.
- `skillId`: exact skill id from the task's `skillist`.
- `skillPath`: resolved readable `SKILL.md` path.
- `loadedAt`: timestamp when the skill was loaded.
- `workStartedAt`: timestamp when task work began.
- `source`: automatic record, generated helper output, or manually entered row.
- `status`: valid, missing, duplicate, late, collapsed, or invalid.

**Validation Rules**:

- One row is required for each `(taskId, skillId)` pairing.
- Rows that cover task ranges or multiple skills as prose are invalid.
- `loadedAt` must be earlier than `workStartedAt`; equality is non-compliant.
- Duplicate rows for the same pairing are reported and must not mask missing rows.

## Audit Readiness Diagnostic

**Purpose**: Describes why an audit readiness contract failed.

**Fields**:

- `readinessPath`: expected or actual readiness file path.
- `status`: missing, incomplete, invalid, or pass.
- `missingTerms`: required terms absent from the file.
- `missingSections`: required sections absent from the file.
- `reason`: human-readable failure reason.
- `blocking`: whether the diagnostic blocks readiness.

**Validation Rules**:

- Missing readiness files must be named exactly.
- Incomplete readiness files must list missing terms or sections.
- Blocking diagnostics must be present in command output and persisted readiness artifacts.

## Readiness Contract

**Purpose**: Defines the readiness files and evidence terms required before implementation or merge readiness.

**Fields**:

- `featureDirectory`: feature scope.
- `requiredFiles`: readiness files enforced by the audit.
- `requiredTerms`: required words or phrases by file.
- `discoverySurface`: task, placeholder, checklist, docs, or generated guidance location where the requirement is visible.
- `owner`: target, script, or template area that enforces the contract.

**Validation Rules**:

- Each audit-enforced file must have at least one discovery surface before implementation starts.
- Required terms must match the audit implementation or generated contract artifact.

## Generated Framework Guidance

**Purpose**: Generated documentation or examples that guide app authors through evidence-safe FS.Skia.UI patterns.

**Fields**:

- `topic`: message qualification, vector conversion, semantic evidence, screenshot vocabulary, or fallback reporting.
- `location`: generated docs, README fragment, tests, or source comments.
- `requiredPhrases`: phrases or examples that must appear.
- `validation`: governance or generated product check that verifies the guidance.

**Validation Rules**:

- Guidance must appear in generated-consumer-facing locations.
- Screenshot wording must not conflate deterministic scene evidence, pixel-readback fallback, and live screenshot proof.
- Message qualification and vector conversion guidance must include concrete generated app examples.

## Screenshot Proof Claim

**Purpose**: Represents a generated evidence statement about screenshot, pixel-readback, or deterministic scene proof.

**Fields**:

- `evidenceKind`: live screenshot, pixel readback fallback, deterministic scene evidence, unsupported host, or failure.
- `provesScreenshot`: true or false.
- `captureSource`: live viewer window, render target, deterministic scene, or none.
- `fallbackReason`: reason screenshot proof was unavailable.
- `deterministicFallbackKind`: fallback category when not screenshot proof.
- `artifactPath`: produced artifact when present.

**Validation Rules**:

- `provesScreenshot=true` is valid only for live screenshot proof accepted by the screenshot contract.
- Pixel-readback fallback and deterministic scene evidence must use `provesScreenshot=false`.
- Unsupported or failed screenshot attempts must include explicit reason and must not imply desktop visibility.
