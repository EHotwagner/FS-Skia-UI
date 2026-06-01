// Interpret.fsi — the I/O edge (feature 045). The only module that executes effects
// against the real filesystem / git / process. runTarget = init -> update (StartTarget t)
// -> interpret over the emitted effect list (the function the exe's Target.create bodies call).
module FS.Skia.UI.Build.Engine.Interpret

open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model

val interpret: root: string -> effect: BuildEffect -> unit
val runTarget: target: Targets.Target -> unit
