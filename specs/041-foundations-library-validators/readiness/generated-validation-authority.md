# Generated-validation authority

Focused per-gate results are authoritative; aggregate FAKE results are non-authoritative.

For this feature the authoritative evidence is: the focused Governance.Tests executable
(parity + typed-finding suites, 295/295), the focused `CapabilityCheck` / `TargetMetadata` /
`TargetMetadataDrift` byte-diff parity (`report-parity.md`), and the per-gate FAKE runs in
the serialized order. No generated product or template content changes (FR-012), so the
generated-product gates validate the unchanged baseline.

Authoritative command: the focused per-target/per-gate command named in each readiness note.
Failure class: `governance / authority-resolution`. Next action: prefer the focused per-gate
rerun over any aggregate result when they disagree.
