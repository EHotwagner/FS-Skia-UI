# Template `--feedback false` Byte-Identity Evidence (T032, SC-006)

## Design-level guarantee

`--feedback false` (the default) is byte-identical to today **by construction**:

- The `feedback` symbol is a `bool` parameter with `defaultValue: "false"`. A parameter
  symbol emits no output of its own.
- Every piece of feedback content lives behind a `"condition": "(feedback == true)"`
  source in `.template.config/template.json`:
  - `template/feedback/skill/` → `.agents/skills/fs-skia-feedback-capture/` and
    `.claude/skills/fs-skia-feedback-capture/`
  - `template/feedback/extensions/` → `.specify/extensions/feedback/`
- The `fs-skia-feedback-capture` skill is authored under `template/feedback/`, **not** under
  the repo `.agents/skills/` tree (which ships unconditionally to every generated project).
  This is the key choice that preserves byte-identity: nothing about feedback enters the
  default generation.

With `feedback == false` none of those three conditional sources fire → no
`fs-skia-feedback-capture` skill, no `.specify/extensions/feedback/`, no `feedback/`
destination, and no stray markers/whitespace. Generated output equals today's.

## Empirical verification — DONE (2026-06-03)

Verified empirically this session. Two projects were generated with an **identical name**
(`-n SameApp`, so `sourceName` substitution is held constant) and `--allow-scripts yes`,
one per `--feedback` value, then compared:

```
dotnet new fs-skia-ui -o bid-false -n SameApp --feedback false --allow-scripts yes   # exit 0
dotnet new fs-skia-ui -o bid-true  -n SameApp --feedback true  --allow-scripts yes   # exit 0
diff -rq --exclude=.git bid-false bid-true
  Only in bid-true/.agents/skills: fs-skia-feedback-capture
  Only in bid-true/.claude/skills: fs-skia-feedback-capture
  Only in bid-true/.specify/extensions: feedback
```

The `feedback == false` (default) output differs from `feedback == true` by **exactly the
three conditional feedback sources and nothing else** — every other generated file is
byte-identical. SC-006 is met empirically: the `feedback` symbol induces **zero diff** in
the default branch. (The first comparison attempt used different output names and so showed
spurious differences from `sourceName` substitution; holding the name constant isolates the
flag's true effect.)

`TemplateCheck` and `GeneratedProductCheck` (which generate with the default `feedback=false`)
both ran **green** this session, confirming the default generated project builds and validates.
