// GeneratedRunner.fsi — the generated-product evidence façade (feature 064, FR-004 / R1).
//
// A single, reflection-friendly entry point the generated `build.fsx` invokes WITHOUT a
// typed `open`, so the script can resolve the engine assembly from `<FsSkiaUiVersion>` at
// runtime (Assembly.LoadFrom) and bind exactly one literal version in the whole project.
// This is the impure I/O edge for a generated product: it resolves the feature directory,
// reads the readiness inputs, runs the pure Evidence.Engine graph/audit, and writes the
// artifacts + the command report. Behaviour is the verbatim relocation of the evidence
// orchestration that previously lived inline in `template/base/build.fsx`.
namespace FS.Skia.UI.Build.Evidence

module GeneratedRunner =

    /// Run a generated-product evidence target ("EvidenceGraph" | "EvidenceAudit") from the
    /// generated project rooted at `repoRoot` (its current working directory). Resolves the
    /// feature directory (SPECKIT_FEATURE_DIR override, else .specify/feature.json; never a
    /// bundled sample), writes the artifacts + report under the feature readiness dir, and
    /// returns the process exit code (0 pass, 2 needs-evidence/graph-error).
    val run: target: string -> repoRoot: string -> int
