# Scaffold-map + source-spec discoverability (FR-006/007/008, SC-005)

Verification record for US4 (T029).

## FR-006 — scaffold-map discoverability + `.fsi`-authoritative note

- `.agents/skills/speckit-plan/SKILL.md` (regenerated to `.claude/**` via
  `RefreshSurfaceBaselines`) step 2 now points an author planning a generated product at
  `docs/scaffold-map.md` **before** reconstructing the durable-vs-replaceable map by hand.
- `template/base/docs/scaffold-map.md` carries a new **"API surface authority"** section:
  the shipped `.fsi` / `docs/api-surface/` tree is the **authoritative** API reference;
  an agent-generated API summary (Explore digest, hand note) is **supporting reference
  only, never ground truth**, and must be reconciled against the `.fsi` (which wins on
  disagreement).

## FR-007 — external-URL source-spec snapshot

- `.agents/skills/speckit-specify/SKILL.md` step 3 now snapshots an **external-URL**
  source into `specs/<feature>/source-spec.md` (URL recorded in a header) and has `spec.md`
  reference the in-repo snapshot — reproducible offline, provenance captured in-repo. For
  **local-file / inline** input the step is an explicit **no-op** (no redundant copy).

## FR-008 — evidence-path token

- Resolved: **consumer-authoring-only, no code change.** No template seeds a divergent
  `evidence/` token. See [evidence-path-token-scan.md](./evidence-path-token-scan.md).

These are skill/doc/process pointers — no gate, no new artifact. Their generated-project
form is exercised by `TemplateCheck`/`GeneratedProductCheck` after `RefreshSurfaceBaselines`
regenerates `.claude/**` (Phase 8).
