// Update.fsi — the PURE decision boundary (feature 045, Principle IV / FR-007).
//
// The signature deliberately exposes ONLY update : BuildMsg -> BuildModel ->
// BuildModel * BuildEffect list. No filesystem / git / process / write symbol is
// surfaced, so the compiler enforces that update's body performs no I/O — all
// interpretation happens at the Interpret edge.
module FS.Skia.UI.Build.Engine.Update

open FS.Skia.UI.Build.Engine.Model

val update: msg: BuildMsg -> model: BuildModel -> BuildModel * BuildEffect list
