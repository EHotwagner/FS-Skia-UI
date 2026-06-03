# Quickstart: Skills Quality Uplift & Per-Phase Feedback Loop

How a maintainer/agent exercises and verifies this feature.

## 1. Extract the support library (full extraction)

```bash
# New packable library; bodies moved from build/Governance behind stable .fsi
ls src/SkillSupport/                       # Graph/Parsing/Globbing/CodeGen/ShellProcess .fsi+.fs
dotnet build src/SkillSupport/SkillSupport.fsproj
# FS.Skia.UI.Build consumes it by ProjectReference (no nupkg circularity)
./fake.sh build -t Dev                      # governance + tests green via moved modules
```

Parity guard: existing `build/Governance` tests are **re-pointed** at the moved
modules; new `tests/SkillSupport.Tests` add FsCheck coverage on the `.fsi`.

## 2. Run the skill-quality gate

```bash
./fake.sh build -t Route                    # confirm escalation + gate list for this diff
./fake.sh build -t SkillQualityCheck        # PASS over all in-scope skills
cat readiness/skill-quality-check.md        # per-skill PASS; demonstrated FAIL names skill+section
```

Edits land in `.agents/skills/**` and `template/product-skills/**`; regenerate the
`.claude` tree (no hand-sync):

```bash
./fake.sh build -t RefreshSurfaceBaselines  # .agents → .claude + validation.contract.yml
./fake.sh build -t Dev                       # SkillSyncCheck: no drift
```

## 3. Govern the new package surface

```bash
./fake.sh build -t PerPackageSurfaceDiff     # diffs readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt
```

## 4. Verify the template feedback parameter

```bash
./fake.sh build -t PackLocal                 # pack libs incl. FS.Skia.UI.SkillSupport
# Default: byte-identical to today
dotnet new fs-skia-ui -o /tmp/fb-off
# diff /tmp/fb-off against a baseline generation → zero changes (SC-006)

# Opt-in: per-phase capture
dotnet new fs-skia-ui --feedback true -o /tmp/fb-on
grep -R "speckit.feedback.capture" /tmp/fb-on/.specify/extensions.yml   # after_* entries present
ls /tmp/fb-on/.agents/skills/fs-skia-feedback-capture/                  # command skill present
```

Run a Spec Kit phase in `/tmp/fb-on`; on completion the agent surfaces the three
prompts and writes `specs/<feature>/feedback/<phase>-<date>.md` (process friction,
generalizable-code candidate, severity, research links). In `/tmp/fb-off` nothing
fires.

## 5. Full maintainer-verify pipeline (escalated path)

FAKE shares `.fake` state — run sequentially in order:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Acceptance mapping

| Success criterion | Verified by |
| --- | --- |
| SC-001/SC-002 quality bar | step 2 (`SkillQualityCheck` PASS + FAIL demo) |
| SC-003 driven-library API + example | step 2 (fsharp-* → SkillSupport; fs-skia-* → product pkg) |
| SC-004 library ships + references resolve | steps 1, 4 (generated project carries `.fsi`) |
| SC-005 feedback prompts + record | step 4 (`--feedback true`) |
| SC-006 default byte-identical | step 4 (`--feedback false` diff) |
| SC-007 research mandate + worked example | step 2 + `readiness/feedback-record-example.md` |
