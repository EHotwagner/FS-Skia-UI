# Compatibility Impact

status=ok

This feature is behavior-preserving cleanup. It does not remove or rename
public APIs, package IDs, generated commands, generated profile names, FAKE
targets, readiness paths, report fields, or exit-code meanings.

Compatibility notes:

- Existing generated evidence and layout evidence commands keep their schemas
  and unsupported-host classifications.
- Build target names remain stable, including `Dev`, `Verify`, `Ci`,
  `PackLocal`, `DependencyReport`, `TemplateCheck`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and
  `EvidenceAudit`.
- Viewer host capability diagnostics remain explicit. Unsupported screenshot or
  window hosts are reported as unsupported evidence, not successful screenshot
  or persistent-window proof.
- Compatibility package restructuring is out of scope and deferred.

Validation path:

- Phase checks record behavior preservation in the required readiness files.
- Final readiness runs `EvidenceGraph` and `EvidenceAudit`.
