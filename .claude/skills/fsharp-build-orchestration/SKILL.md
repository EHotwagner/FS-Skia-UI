---
name: fsharp-build-orchestration
description: Drive FAKE targets from the compiled front-end; golden-diff parity with DiffPlex; property/unit tests with Expecto + FsCheck.
compatibility: F# governance library (build/Governance) + build/Build.fsproj under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-build-orchestration

Cookbook for the compiled build front-end, the parity gates, and the test harness. Owns **C18**
(build orchestration / CLI), **C19** (golden-output text diffing), and **C20** (property/unit
testing). Verdicts come from the capability report (`metadata.source`) §8.

## When to use

- Registering and dispatching FAKE targets from the compiled `build/Build.fsproj` exe (no FSX runner,
  no FCS).
- The Stage-4/5 golden-parity gate: prove ported output is byte-identical to the Stage-0 fixtures.
- Unit- and property-testing the ported parsers/algorithms in `tests/Governance.Tests`.

## Library verdicts

- **Orchestration / CLI (C18) → `Fake.Core.Target` 6.1.4** (present). The D2 spike proved it drives
  targets from the compiled exe with **no FSX runner** and **no `FSharp.Compiler.*`** transitive
  (FR-008). Dispatch via `Target.runOrDefaultWithArguments`; delegate each target body to a library
  function. **Argu** (typed CLI) and **Spectre.Console** (tables/colour) are **optional** — adopt only
  if the `Route` CLI grows.
- **Golden diff (C19) → DiffPlex 1.9.0.** Readable unified/side-by-side diffs for the parity gate and
  the Stage-2 generation-currency check. **Adopt DiffPlex.**
- **Testing (C20) → Expecto 10.2.2** (in-tree) **+ FsCheck 3.3.3.** Re-point moved-logic tests at the
  real library functions (assert typed errors, not strings). `Expecto.FsCheck` adds the ergonomic
  `testProperty`; until referenced, call FsCheck's `Check` directly inside an Expecto `test`.

## API walkthrough + runnable examples

### C18 — targets and dependency edges (`Fake.Core.Target`)

`Target.create name body` registers a target; `==>` (from `Fake.Core.TargetOperators`) declares a
dependency edge. Bodies delegate to library functions so a mistyped target/predicate fails to **compile**.

```fsharp
open Fake.Core
open Fake.Core.TargetOperators

/// C18 — register two targets and the edge Build -> Test. Dispatch (elsewhere)
/// via Target.runOrDefaultWithArguments "Test".
let defineTargets () =
    Target.create "Build" (fun _ -> ())
    Target.create "Test" (fun _ -> ())
    "Build" ==> "Test" |> ignore
```

### C19 — golden-output parity gate (DiffPlex)

Every ported parser/algorithm/renderer must produce **byte-identical** output to the Stage-0 golden
fixtures before the Python/Bash is deleted (Invariant 6). Return both the verdict and readable drift
for the failure report.

```fsharp
open DiffPlex
open DiffPlex.DiffBuilder
open DiffPlex.DiffBuilder.Model

/// C19 — true when ported output equals the committed golden, with readable drift.
let goldenParity (golden: string) (ported: string) =
    let model = InlineDiffBuilder(Differ()).BuildDiffModel(golden, ported)

    let drift =
        model.Lines
        |> Seq.filter (fun line -> line.Type <> ChangeType.Unchanged)
        |> Seq.map (fun line -> sprintf "%A | %s" line.Type line.Text)
        |> Seq.toList

    List.isEmpty drift, drift
```

### C20 — unit tests (Expecto)

```fsharp
open Expecto

let parserTests =
    testList "parser parity" [
        test "task id is extracted from a well-formed line" {
            let line = "- [X] T024 [skillist: []] do the thing"
            let id = line.Split(' ') |> Array.tryItem 2
            Expect.equal id (Some "T024") "the id token is the third field"
        }
    ]
```

### C20 — property tests (FsCheck inside Expecto)

Without `Expecto.FsCheck` referenced, call FsCheck's `Check.QuickThrowOnFailure` inside a normal
Expecto `test` — it throws on a counterexample (failing the test), exactly the gate behaviour. With
`Expecto.FsCheck`, prefer `testProperty "name" prop`.

```fsharp
open Expecto
open FsCheck

let propertyTests =
    testList "graph properties" [
        test "reverse is an involution" {
            Check.QuickThrowOnFailure(fun (xs: int list) -> List.rev (List.rev xs) = xs)
        }
    ]
```

## Cautions

- **FAKE concurrency:** never run FAKE-backed targets concurrently (shared `.fake` state); use the
  deterministic serialized order from `CLAUDE.md`/`AGENTS.md`.
- **`Expecto.FsCheck` is a separate package** — `testProperty` needs it; otherwise call `Check`
  directly (as above).
- **Build-tooling scope.** All packages are referenced by `build/**` or `tests/Governance.Tests`,
  **never shipped** in a generated product; versions go in `Directory.Packages.props` (Central Package
  Management). No FCS.
- Record warm/cold build wall-clock vs the Stage-0 baseline; the compiled library should beat the
  207 KB script recompilation.

## Consuming stages

Stage 5 (compiled build front-end + routing), Stage 4/5 (golden-parity exit gates), all stages
(Expecto + FsCheck regression/property tests). See the plan in `metadata.source`.

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Sources / links

- Fake.Core.Target: <https://fake.build/reference/fake-core-target.html>
- DiffPlex: <https://github.com/mmanela/diffplex>
- Expecto: <https://github.com/haf/expecto> · FsCheck 3: <https://fscheck.github.io/FsCheck/>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §8.

## Related

[[fsharp-parsing]], [[fsharp-graph-algorithms]] (the ported logic under test),
[[fsharp-code-generation]] (DiffPlex also backs the currency check), [[fsharp-io-globbing]],
[[fsharp-shell-process]] (the residual processes the front-end drives).
