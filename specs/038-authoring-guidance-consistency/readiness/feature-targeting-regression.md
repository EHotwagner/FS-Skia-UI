# Feature-Targeting Regression Guard (FR-011, SC-008)

The evidence gates resolve the audited feature from `.specify/feature.json`
`feature_directory` and refuse a placeholder fallback; a bare filename mention in
`tasks.md` prose does not fire required evidence (behavior established by feature
037; this is a regression guard).

## Regression guard (T034)

`tests/Governance.Tests/FeatureResolutionRobustnessTests.fs` — new test
"compute-task-graph does not fire required evidence from a bare filename
mention": a fixture feature with two real tasks whose prose names `report.md`,
`evidence.txt`, and `screenshot.png` in passing must (a) resolve from
`feature.json` (echo `052-filename-mention`) and (b) report `real-task-count: 2`
— the prose filename mentions do not become tasks or required evidence. The
suite passes (Governance.Tests: 250/250 in the GeneratedProductCheck dep chain).

The same file already guards (037) that `build.fsx` resolution drops the
hardcoded placeholder and hard-fails (`Cannot resolve the active feature`),
naming `feature_directory` as the authoritative source.

## Gate run (T035)

`./fake.sh build -t EvidenceGraph` then `./fake.sh build -t EvidenceAudit`
(sequential) — both green:

```
EvidenceGraph: feature: .../specs/038-authoring-guidance-consistency
               resolved-feature: 038-authoring-guidance-consistency
               real-task-count: 38
EvidenceAudit: Status: Ok   exit-code=0
```

The resolved feature id is echoed from `.specify/feature.json`; the audit
produced its synthetic-propagation + diff-scan outputs and blocked on nothing
(only advisory `[adv]` items remain). See `logs/evidence-graph.txt` and
`logs/evidence-audit.txt`.

## Note (non-triggering wording)

The audit's separate conditional runtime/GUI scan keys on literal marker phrases
in spec/plan/tasks. The tasks.md validator-pitfall guidance was reworded so its
*meta*-mention of GUI/window trigger wording no longer contains a literal marker
(`persistent gui runtime`), correctly classifying this non-runtime feature and
keeping the GUI/window scans dormant — the same "a bare mention must not fire
required evidence" principle this guard protects.
