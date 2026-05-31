// Spike.fsi — the entire public surface of the FS.Skia.UI.Build governance
// library skeleton for feature 039 (Principle II: visibility lives in the .fsi).
//
// This is a NEW build-tooling surface. It is NOT part of the tracked runtime
// surface baselines (PackageSurfaceCheck / FsiTranscripts), which must show no
// diff (SC-006). The library lives under build/, not src/, so runtime
// surface-baseline tooling does not sweep it.
module FS.Skia.UI.Build.Spike

/// Trivial demonstration body invoked by the build front-end's spike target.
/// Returns a stable, identifiable success message proving the body ran from the
/// library (not inlined in the front-end). This is the one-target slice that
/// de-risks D2 (FAKE Target API driven from a compiled exe).
val run : unit -> string
