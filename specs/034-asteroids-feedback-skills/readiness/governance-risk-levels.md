# Governance Risk Levels

command: `./fake.sh build -t Dev`, `./fake.sh build -t GeneratedGuidanceCheck`, `./fake.sh build -t TemplateCheck`, `./fake.sh build -t GeneratedProductCheck`, `./fake.sh build -t PackLocal`, `./fake.sh build -t EvidenceGraph`, `./fake.sh build -t EvidenceAudit`
scanned files: `specs/034-asteroids-feedback-skills/plan.md`, `specs/034-asteroids-feedback-skills/tasks.md`, generated guidance files, public `.fsi` files, packed NuGet artifacts.
observed: medium governance risk for shared task guidance, generated product guidance, and XML documentation surfaces.
missing: none.
failure class: none.
next action: use focused checks for edited surfaces, then broad validation only when shared templates or package artifacts change.

Risk levels:

| Level | Required evidence | Broad validation |
|-------|-------------------|------------------|
| small | Focused test or scan for one localized documentation or readiness file. | Not required unless shared templates change. |
| medium | Required evidence includes focused governance tests, generated guidance scans, template checks, generated product scans, package XML inspection, and graph refresh. | Broad validation is required when shared templates, generated product guidance, command surfaces, or packable package documentation output change. |
| broad | Full governed workflow, package inspection, graph, and audit. | Broad validation is required for cross-package API shape changes, runtime behavior changes, or template/package release changes. |

This feature remains medium risk because runtime API shape, package versions,
renderer behavior, and new Asteroids demo implementation are deferred.
