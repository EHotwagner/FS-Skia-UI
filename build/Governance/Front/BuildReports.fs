module BuildReports

open System
open System.IO
open BuildPaths

let writeReport outputPath lines =
    ensureParent outputPath
    File.WriteAllText(outputPath, String.concat Environment.NewLine lines + Environment.NewLine)

let appendOrCreateSection outputPath header lines =
    ensureParent outputPath
    let content = String.concat Environment.NewLine lines + Environment.NewLine

    if File.Exists outputPath && File.ReadAllText(outputPath).StartsWith(header, StringComparison.Ordinal) then
        File.AppendAllText(outputPath, content + Environment.NewLine)
    else
        File.WriteAllText(outputPath, header + Environment.NewLine + Environment.NewLine + content + Environment.NewLine)

let writeJoinedReport outputPath rows =
    writeReport outputPath rows
