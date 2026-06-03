# Simulated Pin-Bump Proof — Feature 054 (US1, SC-003)

The extended `fs-skia-template-update` step 3 must move **both** pins in one flow,
leaving them equal with no manual second edit (FR-002, contract C2).

**Authoritative commands** (the documented step-3 flow, throwaway `0.1.99-preview.1`):

```bash
old=0.1.56-preview.1; new=0.1.99-preview.1
# props (existing sed — Version="<ver>" form):
sed -i "s/Version=\"$old\"/Version=\"$new\"/g" template/base/Directory.Packages.props
# build.fsx #r literal (added in step 3 — note the `|` delimiter, not `#`,
# because the pattern contains `#r`):
sed -i "s|\(#r \"nuget: FS\.Skia\.UI\.Build, \)[^\"]*\"|\1$new\"|" template/base/build.fsx
```

## Result

```
after one-flow bump: build.fsx=0.1.99-preview.1 props=0.1.99-preview.1
PASS both moved together (no manual second edit)
```

Then reverted to `0.1.56-preview.1` (both pins). Working tree clean afterwards.

## Note on the sed delimiter (defect found & fixed during this proof)

The first draft of step 3 used a `#`-delimited `s#…#…#`. Because the search
pattern contains `#r`, sed terminated the expression early
(*"unknown option to 's'"*). The skill (and its regenerated `.claude` peer) were
corrected to a `|` delimiter, which the pattern never contains. The proof above
runs the corrected command.

**Failure class:** pin drift after a routine post-merge bump (the original §4.1
finding). **Next action:** none — the one-flow bump structurally keeps both pins
equal; the parity gate ([[deliberate-mismatch-gate]]) backstops it.
