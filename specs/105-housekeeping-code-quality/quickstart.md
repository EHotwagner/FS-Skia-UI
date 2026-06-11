# Quickstart: Validate the housekeeping change

This is a behavior-preserving internal refactor. Validation proves **no observable
change** plus the duplication/qualifier/typing wins.

## 1. Route first — run only the gates it prints

```bash
./fake.sh build -t Route
```

The audit predicts the inner-loop `Dev` tier (the edits are `src/Controls/**` +
`Scene.fs` + `SkiaViewer.fs` `.fs` bodies, no `.fsi`). **Be prepared** for `Route`
to escalate to the `controls-public-surface` maintainer-verify set —
features 101/102 observed that *any* `src/Controls/**/*.fs` edit can escalate even
with zero `.fsi` delta. Run exactly the gates `Route` prints, FAKE-backed targets
**sequentially**. If escalated, the serialized order is:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck   # if printed
./fake.sh build -t TemplateCheck            # if printed
./fake.sh build -t GeneratedProductCheck    # if printed
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## 2. Prove zero public-surface delta (SC-007)

```bash
git diff --stat origin/main...HEAD -- 'src/**/*.fsi'   # expect: empty
```

If non-empty, an internal-only choice leaked to the surface — revert to the
default (D3–D6) before proceeding. (The optional FR-012 public-DU expansion is
**not** elected by this plan.)

## 3. Prove the de-duplication (SC-001, SC-002)

```bash
grep -rn "let withKeyOpt"  src/Controls/Widgets/   # expect: 1 (WidgetLowering.fs)
grep -rn "let onString\b"  src/Controls/Widgets/   # expect: 1
grep -rn "let onStringList" src/Controls/Widgets/  # expect: 1
grep -n  "let onChanged "  src/Controls/Control.fs # expect: 0 inline copies
grep -n  "Double.TryParse" src/Controls/Control.fs # expect: 1 (inside tryParseFloat)
```

## 4. Prove the qualifier cleanup (SC-003)

```bash
grep -rn "module private" src/Controls/Widgets/    # expect: 0
grep -n  "let private" src/Controls/Reconcile.fs   # expect: only applyAttrChanges remains
grep -n  "let private" src/Controls/RetainedRender.fs # expect: only fadeOutAnimation/firstFrameCollisions remain
grep -n  "module internal SceneRenderer" src/SkiaViewer/SceneRenderer.fs # expect: 1 (KEEP, untouched)
```
Confirm each former `private` site still carries its "hidden by `<X>.fsi`" comment.

## 5. Prove parity (SC-006) and green suites (SC-005)

- The parity assertion over the consolidated helpers passes (lowered
  `Control<'msg>` `%A`-identical to baseline).
- Controls + Controls.Elmish Expecto suites pass, no test edits forced by behavior.
- `EvidenceAudit` verdict = PASS, 0 synthetic, no diff-scan blockers.

## 6. Confirm the deferred items are untouched (SC-008)

```bash
git diff origin/main...HEAD -- src/Controls/Types.fs src/Controls/Types.fsi \
  src/SkiaViewer/SkiaViewer.fsi
# expect: no ControlId-wrapper, no ControlKind change, no public diagnostic/mode
#         field conversion, no AttrValue custom-equality change, no file splits
```
