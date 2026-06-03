// ShellProcess.fsi — fsharp-shell-process family.
//
// A generic external-process runner with captured stdout/stderr/exit-code, plus a
// thin `git` convenience wrapper, for the SkillSupport shipped library. Consumers
// in build/Governance drive their target-specific invocations through these.
// Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module ShellProcess =

    /// The captured result of running an external process.
    type ProcResult =
        { ExitCode: int
          StdOut: string
          StdErr: string }

    /// Run `exe` with `args` in `workingDir`, capturing stdout, stderr, and the
    /// exit code. Arguments are passed as a list (each is quoted as a single
    /// argument); no shell interpolation is performed.
    val run: exe: string -> args: string list -> workingDir: string -> ProcResult

    /// Convenience wrapper: `run "git" args workingDir`.
    val git: args: string list -> workingDir: string -> ProcResult
