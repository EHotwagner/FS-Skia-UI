# Phase 0 Research: Asteroids Feedback Skill Guidance

## Decision: Improve generated task guidance instead of adding new hard skill validators

**Rationale**: The feature asks for better task metadata and discoverable guidance, while FR-014 explicitly preserves existing validation behavior for correctly authored task lists and keeps advisory suggestions non-blocking. Generated guidance can require `skillist` metadata where existing governance already requires it, while FS.Skia.UI capability hints remain recommendations unless an already-mandatory local skill applies.

**Alternatives considered**:
- Add hard validator rules for every FS.Skia.UI visual-demo keyword. Rejected because this would create new false-positive risk and contradict the advisory scope.
- Leave skill choice to implementers. Rejected because the Asteroids feedback shows implementers discover relevant skills too late.

## Decision: Treat multiple skill assignments as first-class task metadata

**Rationale**: Visual demo work often spans implementation, layout evidence, generated product validation, debug-until-green loops, and audit validation. The task generator should show that a single task can list multiple skills in `tasks.deps.yml` and mirror them visibly in `tasks.md`, with the minimal applicable set in dependency order.

**Alternatives considered**:
- Split every multi-skill task into separate tasks. Rejected because some work units genuinely require one implementation step plus evidence validation.
- Pick only the most specific skill. Rejected because it hides audit or debug guidance for tasks that need it.

## Decision: Scaffold the readiness contract as task-authoring guidance

**Rationale**: The Asteroids implementation report shows required audit files and key terms were discovered by failing `EvidenceAudit` and reading audit scripts. Generated tasks should enumerate required readiness files and expected fields for generated visual demos before `/speckit-implement` begins.

**Alternatives considered**:
- Keep the audit script as the only source of truth. Rejected because implementers should not reverse-engineer the contract from failures.
- Generate empty placeholder files only. Rejected because file names alone do not expose required fields such as `failure-class`, `authoritative`, or artifact-path checks.

## Decision: Make visual evidence honesty explicit in acceptance cues

**Rationale**: The feedback identifies three distinct proof classes that must not be conflated: decodable screenshot artifacts, rasterized scene pixels, and layout bounds/readability reports. Guidance must reject metadata-only screenshot reports, 1x1 fallback images, and layout-only reports as complete visual proof.

**Alternatives considered**:
- Accept renderer reports when they claim `proves-screenshot=True`. Rejected because the reported Asteroids screenshot path contained text, not a decodable PNG.
- Treat layout evidence as a substitute for image proof. Rejected because layout bounds cannot prove visible stroke or glyph rasterization.

## Decision: Classify Asteroids feedback by owner before creating follow-up work

**Rationale**: The report contains framework rendering gaps, generated evidence workflow gaps, documentation/discoverability friction, and consumer authoring choices. Owner classification prevents runtime bugs, task guidance changes, and author mistakes from being mixed in one backlog.

**Alternatives considered**:
- File all findings as framework bugs. Rejected because readiness under-scaffolding and author misjudgment are not runtime renderer defects.
- Treat everything as consumer workaround guidance. Rejected because stroke rasterization, text rasterization, screenshot artifact honesty, and host-size delivery expose framework or contract gaps.

## Decision: Validate guidance with real scans plus bounded fixtures

**Rationale**: Real scans of repository templates and generated guidance prove the guidance is visible where implementers need it. Bounded fixtures remain useful for edge cases such as metadata-only screenshots or fallback PNG claims, as long as they are disclosed as guidance fixtures and do not replace real scans.

**Alternatives considered**:
- Use only synthetic task lists. Rejected because SC-007 requires generated guidance validation before implementation begins.
- Require a full generated Asteroids implementation. Rejected because implementing a new demo is explicitly out of scope.

## Decision: Treat public `.fsi` XML comments as a package contract, not a runtime API change

**Rationale**: The clarified requirements add consumer-facing documentation without changing runtime API shapes. The `.fsi` files remain the source of public surface truth, and XML comments on those signatures are the right place to document modules, types, union cases, records, fields, values, parameters, returns, and non-obvious workflows. This satisfies the API discoverability feedback while preserving the constitution's `.fsi` ownership model.

**Alternatives considered**:
- Document only generated task guidance. Rejected because the clarification explicitly requires comprehensive XML docs for every public `.fsi` in packable framework packages.
- Add separate Markdown API guides instead of XML comments. Rejected because package consumers need shipped XML documentation in IDEs and NuGet artifacts.
- Change public signatures to improve discoverability. Rejected for this feature because runtime API shape changes are deferred and would require a separate `.fsi` design pass.

## Decision: Validate XML documentation at both build output and NuGet package boundaries

**Rationale**: `Directory.Build.props` already enables XML documentation generation, but that only proves the compiler can emit a file. The feature requires hard validation that generated XML docs are non-empty, map back to public `.fsi` members, and are included in packed NuGet artifacts for every packable framework package. Both boundaries are needed: build output protects authoring quality, while package inspection proves consumers receive the docs.

**Alternatives considered**:
- Rely on compiler warnings for missing XML comments. Rejected because warnings are easy to miss and do not prove package inclusion.
- Validate only `.nupkg` contents. Rejected because package presence alone does not prove public members are documented.
- Introduce a new XML documentation package dependency. Rejected unless implementation proves existing F#/MSBuild/XML parsing tools are insufficient.
