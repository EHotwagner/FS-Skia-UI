# Contract: API-reference doc-preservation check (FR-004 / FR-005)

Governance test in `tests/Package.Tests/PackageApiReferenceTests.fs`, validating output of
`scripts/generate-package-api-reference.fsx`. Classification helper reused from
`build/Governance/ControlsDocCoverage.fs(.fsi)`.

## Old signal (removed)

The test asserted the **placeholder boilerplate sentence** is present in the Scene and Testing
references:

```fsharp
[ scene; testing ] |> List.iter (fun r ->
    Expect.stringContains r "Public contract type exposed by this FS.Skia.UI package." "...")
```

Brittle: removing the boilerplate from a package (the deferred non-Controls doc cleanup) re-breaks
the check, exactly as feature 106 had to special-case Controls.

## New signal (FR-004) — package-agnostic

For **every** tracked package reference (`docs/api-surface/<PackageId>.md`), the embedded
"## Curated Signatures" body MUST contain **at least one `///`-prefixed summary line that is not a
placeholder**:

```fsharp
// substantive = a /// line whose summary text is NOT classified as placeholder boilerplate
let hasSubstantiveSummary (reference: string) =
    reference.Replace("\r\n","\n").Split('\n')
    |> Array.exists (fun line ->
        let t = line.TrimStart()
        t.StartsWith("///", StringComparison.Ordinal)
        && not (ControlsDocCoverage.isPlaceholderSummary t))
```

- Holds for Scene/Testing whether or not they still carry placeholder boilerplate.
- Optional corroboration: the reference header reports `xml-summary-count: N` with `N > 0`.

## Retained guarantee (FR-005)

The check MUST FAIL when the reference body carries **zero** `///` summary lines (the generator
dropped summaries). Proven by a red-before fixture: a synthetic reference string with no `///` lines
makes `hasSubstantiveSummary` return `false` → the assertion fails. The guarantee is retained; only
its brittle sample (placeholder-sentence presence) is replaced.

## Generator (unchanged)

`generate-package-api-reference.fsx` continues to embed each package's full `.fsi` verbatim under
"## Curated Signatures" and to compute `xml-summary-count` over that emitted body — so the `///`
lines inspected by the check are a faithful preservation signal, not a decoupled self-report.
