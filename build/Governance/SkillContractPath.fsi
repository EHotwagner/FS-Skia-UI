// SkillContractPath.fsi — SkillContractPathCheck (feature 060, FR-004; Principle II).
//
// Fails the build when a capability/product skill names a `docs/api-surface/...fsi`
// contract source that the generated api-surface tree does not emit, or claims "no DLL
// reflection needed" against an absent path — so skill claims and generated output
// cannot drift. Parsing/checking are pure; the skill-file reads stay at the build edge.
module FS.Skia.UI.Build.SkillContractPath

open FS.Skia.UI.Build.ApiSurfaceGen

/// A `docs/api-surface/<Pkg>/<file>.fsi` contract source a skill names.
type SkillContractClaim =
    { /// repo-relative skill path (e.g. `template/product-skills/fs-skia-scene/SKILL.md`).
      SkillPath: string
      /// generated-project-relative path the skill names (e.g. `docs/api-surface/Scene/Scene.fsi`).
      ClaimedPath: string
      /// whether this skill asserts "no DLL reflection needed".
      ClaimsNoReflection: bool }

/// Extract every `docs/api-surface/...fsi` claim from one skill file. Pure.
val parseClaims: skillPath: string -> skillText: string -> SkillContractClaim list

/// Check all claims against the emitted entries. Returns build-failure diagnostics
/// (empty == clean): a claimed path not in the emitted tree, or a "no DLL reflection
/// needed" assertion against an absent path. Pure.
val check: entries: ApiSurfaceEntry list -> claims: SkillContractClaim list -> string list

/// Advisory orphan report: emitted api-surface files no skill claims. Pure.
val orphans: entries: ApiSurfaceEntry list -> claims: SkillContractClaim list -> string list
