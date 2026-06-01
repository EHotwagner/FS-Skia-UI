# SC-005 — evidence-path language reduction

Stage-0 baseline evidence path: { F#, Bash, Python } —
  build.fsx (F#) -> run-audit.sh (1,284 lines Bash) -> compute-task-graph.py (1,310) + audit-status-scan.py (150) -> JSON re-parsed in F#.

Stage-4 steady-state evidence path: { F# } + thin OS-glue git —
  build.fsx (F#) -> FS.Skia.UI.Build.Evidence.Engine (compiled F#, in-process); the only retained external process is the 'git diff' read at the build.fsx edge.

The in-process F# engine is proven byte-identical to the Python engine across 036/037/038 (see parity/). The legacy Python + Bash files are now **removed** (T029): `.specify/extensions/evidence/scripts/{python,bash}/` is deleted; `extension.yml` `requires.tools` lists only `git` (no `bash`, no `python3`). The retained `audit-patterns.yml` (data read by the diff-scan) is the only file kept under `extensions/evidence/`. Generated `dotnet new fs-skia-ui` consumers run the same in-process engine via the published `FS.Skia.UI.Build` package — no copied scripts (see ../package/sc-006-generated-consumer.md). Steady-state evidence-path languages: **{ F# } + thin OS-glue git**, down from { F#, Bash, Python } (SC-005).
