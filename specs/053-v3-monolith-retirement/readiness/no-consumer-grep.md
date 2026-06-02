# No-consumer grep (SC-001 proof) + CPM/template pin verify (FR-005)

The retirement is proven by a repo-wide grep returning **zero** hits over the
build-relevant trees. Programme history under `docs/`/`specs/` is excluded (it
records the retirement and is allowed to name the retired identity).

## Command

```bash
grep -rn -E 'Lib\.fsproj|src/Lib|"FS\.Skia\.UI"' \
     src samples tests template build *.sln Directory.Packages.props \
  | grep -vE '/bin/|/obj/'
```

## Result

**Zero hits.** `src/Lib` is deleted; no `Lib.fsproj` `ProjectReference` or
path-string remains; the exact `"FS.Skia.UI"` monolith id is no longer named by any
build-relevant source. Negative guards that must still bite name the retired id via
string parts (`"FS.Skia." + "UI"`, `$@"..\{lib}\{lib}.fsproj"`) — the established
in-repo pattern (cf. `GeneratedProduct.fs` `"FS.Skia.UI." + "Charts"`) — so they keep
asserting absence without re-introducing a literal consumer reference.

A complementary backslash/forward-slash check is also empty:

```bash
grep -rn -E '\\Lib\\Lib|/Lib/Lib' src samples tests template build *.sln \
  | grep -vE '/bin/|/obj/'
```

## CPM / template pin verify (FR-005, verify-only — no edit)

No central-package-management or template pin ever named the monolith; confirmed empty:

```bash
grep -nE 'Include="FS\.Skia\.UI"' Directory.Packages.props                 # NONE
grep -nE 'Include="FS\.Skia\.UI"' template/base/Directory.Packages.props   # NONE
grep -rnE 'Include="FS\.Skia\.UI"' template/                               # NONE
```

failure class: GovernanceConsumerLeak. next action: none — the grep is clean.
artifact path: this file. authoritative command: the grep above.
