// SkillistReference.fsi — single-source generator + currency for the generated
// `template/base/docs/skillist-reference.md` (feature 062, FR-006, D6; Principle II).
//
// Derived from the live `SkillRegistry` (the same discovery the gates use) and the
// closed `owns:` vocabulary in `Audit.ownsVocabulary`, so the reference an author
// reads in a generated project — the valid `skillist` ids (directory-name vs
// `name:` resolved) and the `owns:`→implied-skill table — cannot drift from the
// enforcing code. The render is pure; the registry build + file reads stay at the
// build interpreter edge.
module FS.Skia.UI.Build.SkillistReference

open FS.Skia.UI.Build.Evidence

/// Repo-relative emitted path of the generated reference doc.
val referenceDocPath: string

/// Render the full `docs/skillist-reference.md` (FR-006) from the registry + the
/// closed owns vocabulary. Deterministic, byte-stable, no clock/env.
val render: registry: SkillRegistry -> ownsVocabulary: (string * string) list -> string
