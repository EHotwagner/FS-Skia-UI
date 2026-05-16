# US4 Selected Skills Validation

## Verdict

PASS: generated products receive the project skill plus selected and
prerequisite capability skills only.

## Evidence

| Evidence | Path |
|----------|------|
| Skill catalog report | `specs/009-v3-modular-framework/readiness/selected-skills.md` |
| Default app file list | `specs/009-v3-modular-framework/readiness/generated-file-lists/app-source.txt` |
| Headless scene file list | `specs/009-v3-modular-framework/readiness/generated-file-lists/headless-scene-source.txt` |
| Sample pack file list | `specs/009-v3-modular-framework/readiness/generated-file-lists/sample-pack-source.txt` |
| Skill target log | `specs/009-v3-modular-framework/readiness/logs/us4-skillcheck.txt` |

## Omitted Skills

Repository maintenance skills under `.agents/skills/speckit-*` are not copied
into generated products by selected capability generation. The sample skill is
copied only for the sample-pack row. Charts is absent from the headless-scene
row because charts is not selected there.
