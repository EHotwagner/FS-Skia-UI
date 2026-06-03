namespace FS.Skia.UI.SkillSupport

open System.Diagnostics

module ShellProcess =

    type ProcResult =
        { ExitCode: int
          StdOut: string
          StdErr: string }

    let run (exe: string) (args: string list) (workingDir: string) : ProcResult =
        let psi = ProcessStartInfo(exe)
        for a in args do
            psi.ArgumentList.Add a
        psi.WorkingDirectory <- workingDir
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.UseShellExecute <- false
        use p = new Process()
        p.StartInfo <- psi
        p.Start() |> ignore
        let stdout = p.StandardOutput.ReadToEnd()
        let stderr = p.StandardError.ReadToEnd()
        p.WaitForExit()
        { ExitCode = p.ExitCode
          StdOut = stdout
          StdErr = stderr }

    let git (args: string list) (workingDir: string) : ProcResult = run "git" args workingDir
