# Constitution Currency: principle edit flags stale, regenerate reflects it (SC-005 / SC-008)

`.specify/memory/constitution.md` is the single source; the two templates carry generated
principle-summary fragments between `BEGIN/END GENERATED` markers. The currency check is
folded into `TargetMetadataDrift` next to the existing `validation.contract.yml` check.

## Baseline — fragments current → PASS

```
$ ./fake.sh build -t TargetMetadataDrift    # exit 0
PASS: runnable target registry, target metadata, validation contract target references, and docs are aligned.
```

## Edit a principle without regenerating → FAIL (stale region named)

```
$ # append a clause to the first sentence of Principle II in constitution.md
$ ./fake.sh build -t TargetMetadataDrift    # exit 1
.specify/templates/plan-template.md constitution fragment(s) are stale —
stale generated regions: fsi-visibility. Regenerate via ./fake.sh build -t RefreshSurfaceBaselines.
```

The diagnostic names the **template**, the **stale fragment id** (`fsi-visibility`), and
the **actionable regeneration command** (FR-012).

## Regenerate → templates reflect the change → PASS

```
$ ./fake.sh build -t RefreshSurfaceBaselines    # exit 0
$ grep "BEGIN GENERATED: constitution/fsi-visibility" -A1 .specify/templates/plan-template.md
**II. Visibility Lives in `.fsi`, Not in `.fs`** — Every public F# module MUST have a
corresponding `.fsi` signature file (edited for the 044 currency demo).
$ ./fake.sh build -t TargetMetadataDrift        # exit 0 → PASS
```

## Hand-written prose outside the markers is preserved byte-for-byte (FR-010)

The full-file diff after the principle edit + regenerate shows **only** the marked region
changed — no `-`/`+` lines outside the `BEGIN/END GENERATED` pair:

```
$ git diff .specify/templates/plan-template.md   # (filtered to content lines)
+<!-- BEGIN GENERATED: constitution/fsi-visibility -->
+**II. Visibility Lives in `.fsi`, Not in `.fs`** — … (edited for the 044 currency demo).
+<!-- END GENERATED: constitution/fsi-visibility -->
```

(No out-of-marker line appears in the diff. The `ConstitutionFragments.splice` byte-equality
unit test asserts every out-of-marker byte is preserved over a fixture template — see
`readiness/unit-tests.md`.)

The constitution edit was then reverted and `RefreshSurfaceBaselines` re-run; the
`fsi-visibility` region is back to `Every public F# module MUST have a corresponding `.fsi`
signature file.` with no demo residue.

**Verdict: PASS** — a principle edit flags the stale template region (SC-008), regeneration
reflects the change, and hand-written prose outside the markers is preserved (SC-005).
