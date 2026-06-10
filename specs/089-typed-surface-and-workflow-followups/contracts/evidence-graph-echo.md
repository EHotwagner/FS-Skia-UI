# Contract: EvidenceGraph Skillist Resolution Echo (EVGRAPH-ECHO-1)

The interface is the **`EvidenceGraph` gate output** (`readiness/task-graph.md`).

## C1 — per-token resolution section (FR-008)

After running `EvidenceGraph` on a feature whose `tasks.md`/`tasks.deps.yml` carries
`[skillist: <id>]` tokens, the rendered output MUST contain a section listing, for **each distinct**
skillist id, its resolution using the **same** logic as the registry validator (`Audit.fs:150-162`):

```
## Skillist id → SKILL.md path
fs-skia-ui-widgets → src/Controls/skill/SKILL.md
speckit-tasks → .agents/skills/speckit-tasks/SKILL.md
```

## C2 — flagged tokens, distinctly (FR-009)

Tokens that do not resolve to exactly one installed skill MUST appear **separately** from the resolved
lines, each labeled by its failure class (matching the validator's semantics):

```
## Skillist id → unresolved / flagged
controlsshowcase1-widgets → directory name for src/Controls/skill/SKILL.md (accepted id: fs-skia-ui-widgets)
made-up-skill → UNRESOLVED (not registered/readable)
some-dup → ambiguous: a/SKILL.md, b/SKILL.md
```

## C3 — reuse, no new resolution logic (FR-008)

The echo MUST be produced by threading the existing `SkillRegistry` (built at
`Front/Governance.fs:804`) into `Render.taskGraphMd`; it MUST NOT introduce a parallel resolver. A
pure helper `skillistResolution: SkillRegistry -> string list -> string` is unit-tested directly.

## Acceptance (maps to SC-004)

- Running `EvidenceGraph` on a feature with skillist tokens shows each token's `id → SKILL.md path`,
  and any unresolved/alias/ambiguous token is flagged in the same output.
- A `name:`-vs-directory token (the `controlsshowcase1-widgets`/`fs-skia-ui-widgets` case) is visible
  as the alias line — **no** manual `grep '^name:'` cross-check needed.
- The echoed resolution agrees with the `Audit` validator's pass/fail for the same tokens.
</content>
