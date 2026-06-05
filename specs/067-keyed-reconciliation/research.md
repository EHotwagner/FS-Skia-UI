# Phase 0 Research — Internal Keyed Reconciliation

All Technical Context unknowns are resolved below. No `NEEDS CLARIFICATION`
remains.

## R1 — How to make the reconciler genuinely "internal-only" with zero surface delta

**Decision**: declare `module internal FS.Skia.UI.Controls.Reconcile`, and **do
not** add `Reconcile.fsi` to the Controls capability `contracts:` list in
`template/capabilities.yml`.

**Rationale**:
- The repo already uses `module internal` for non-public framework code —
  `src/SkiaViewer/SceneRenderer.fs:17` (`module internal SceneRenderer`) and
  `build/Governance/Evidence/TaskParser.fs:54` (`module internal Lines`). This is
  the established, idiomatic mechanism and is *not* the per-binding `internal`
  modifier Principle II forbids (Principle II governs **top-level bindings** inside
  a public module; a module-level `internal` declaration is allowed and precedented).
- `PackageSurfaceCheck` / `ApiSurfaceGen` compute the emitted public surface
  **only** from the explicit `contracts:` `.fsi` list in `template/capabilities.yml`
  (`build/Governance/ApiSurfaceGen.fs` `plan`/`currency` read `row.Contracts`).
  A `.fsi` not in that list contributes nothing to the surface, so the baseline is
  byte-for-byte unchanged → SC-005, FR-002.
- `module internal` also makes the symbols genuinely unreachable from package
  consumers (assembly-internal accessibility), so "internal-only" is *enforced*,
  not merely a documentation claim.

**Alternatives considered**:
- *Public module excluded from `contracts:`* — would pass `PackageSurfaceCheck`
  but leaves the symbols reflection-reachable by consumers, contradicting FR-002's
  honest intent. Rejected.
- *No `.fsi` at all (like `SceneRenderer`)* — legal for an internal module, but the
  repo's heavy `.fsi` discipline and Principle I ("sketch the surface first") are
  better served by a curated `Reconcile.fsi` declaring `module internal`. We keep
  the `.fsi` for a reviewable, pinned contract; it simply stays out of the
  capability contract list. Accepted with the `.fsi`.

## R2 — How property tests reach an internal module from a separate test assembly

**Decision**: add one assembly attribute to the Controls project —
`[<assembly: InternalsVisibleTo("Controls.Tests")>]` — in a new `AssemblyInfo.fs`
compiled first.

**Rationale**: `tests/Controls.Tests` is a separate assembly (default
`AssemblyName = Controls.Tests`; no override in the fsproj). `InternalsVisibleTo`
is an assembly-level attribute, not a top-level binding access modifier, so
Principle II is unaffected. This is the standard .NET seam for unit-testing
internal code and keeps the production surface internal while the round-trip
property exercises `diff`/`apply` directly. The attribute is the *only* coupling
to the test assembly name; if the assembly name is later overridden, update the
string.

**Alternatives considered**: exposing a thin public test shim (rejected — that
*is* a public-surface delta, violating FR-002/SC-005); reflection from the test
(rejected — brittle, obscures intent).

## R3 — FsCheck availability and version

**Decision**: add `<PackageReference Include="FsCheck" />` to
`tests/Controls.Tests/Controls.Tests.fsproj` (central version already pinned:
`Directory.Packages.props:47` → `FsCheck 3.3.3`). No version literal in the fsproj.

**Rationale**: `Controls.Tests.fsproj` currently references only Expecto +
test SDK (no FsCheck), but `FsCheck 3.3.3` is already a `PackageVersion` and is
referenced by `tests/SkillSupport.Tests` and `tests/Governance.Tests`, so the
harness and version are established repo-wide. This is a **test-only** reference;
it adds no product dependency (DependencyReport unaffected). FsCheck 3.x API note:
properties are written with `Prop.forAll`/`Check.One` and custom generators via
`Arb`/`Gen`; Expecto integrates through `Expecto.FsCheck` or a direct
`Check.One config prop` inside a `test` case.

## R4 — Patch data model

**Decision**: a small DU algebra (full shapes in [data-model.md](./data-model.md)):
`NodePatch<'msg>` = `Keep | Replace of Control<'msg> | Update of UpdatePatch<'msg>`;
`UpdatePatch` carries attribute changes (by `Name`), a `FieldChange` for `Content`
and for `Accessibility`, and an ordered `ChildOp<'msg> list`; `ChildOp` =
`ChildKeep | ChildInsert | ChildRemove | ChildMove` (FR-004). `diff` returns
`ReconcileResult<'msg> = { Patch; Diagnostics }`.

**Rationale**: a closed DU per the spec's required operation set keeps the diff
total and pattern-matchable; `FieldChange<'a> = Unchanged | ChangedTo of 'a`
avoids the awkward `'a option option` for "field set to `None`" vs "field
unchanged". `Replace` carries the full next subtree so `apply` is a pure
substitution. Diagnostics ride alongside the patch rather than throwing (FR-011,
SC-007).

**Alternatives considered**: a flat operation list with absolute paths (rejected —
harder to recurse and to prove the round-trip structurally); reusing
`ControlDiagnostic` for *all* signalling including non-error keeps (rejected — keep
the patch and diagnostics as distinct channels).

## R5 — Sibling matching rule (keys-first, then positional)

**Decision**: within one sibling list — (1) bucket previous and next children by
`Key` (first-occurrence wins on duplicates, emitting a `KeyCollision` diagnostic
for each later collision); (2) pair equal keys as the **same** node regardless of
position; (3) match the residual *unkeyed* children positionally among themselves;
(4) leftover previous-only nodes → `ChildRemove`, next-only → `ChildInsert`;
(5) a matched node whose relative order changed → `ChildMove`, otherwise
`ChildKeep`, each carrying the recursive `NodePatch` of its matched pair.

**Rationale**: this is the single deterministic rule the spec's
interacting-requirements note mandates ("keys win"; residual unkeyed match
positionally). First-occurrence duplicate handling keeps the function total and
the output reproducible (FR-009/FR-011). A `Kind` mismatch between a matched pair
collapses to `Replace` before any attribute diff (FR-006).

**Move detection scope**: emit a `ChildMove` for any matched child whose index in
the previous order differs from its index in the next order under a simple,
deterministic scan — **not** a minimal-move (LIS) computation. The spec scopes
performance/minimality **out** ("minimal-ish… correctness, not a benchmarked fast
path"); US1's acceptance only requires *zero replaces* on a pure reorder and a
*move without an attribute sub-patch*, both of which a simple scheme satisfies.
LIS minimization is a deferred optimization.

## R6 — Attribute diff by name (FR-007)

**Decision**: compare `Attributes` as name-keyed maps: for each `Name` present in
next, emit `AttrSet` if absent-in-prev or value-differs; for each `Name` present
in prev but absent in next, emit `AttrRemoved`. Order-independent; the emitted
change list is sorted by `Name` for determinism.

**Rationale**: `Attr` is `{ Name; Category; Value }` and the IR does not guarantee
attribute order, so diffing by position would be non-deterministic. Sorting the
output by `Name` pins byte-stable output (FR-009/SC-004). Value equality uses
structural `=` on `AttrValue<'msg>`; the `'msg` payload and `UntypedValue of obj`
participate via F# structural/reference equality — acceptable because the diff
never inspects `'msg` semantically, only compares.

## R7 — Round-trip property generator (FR-008/SC-002)

**Decision**: an FsCheck generator for `Control<int>` (concrete `'msg = int`)
producing trees of bounded depth/breadth with a mix of keyed and unkeyed children,
occasional duplicate keys, varied `Kind`/`Content`/`Attributes`/`Accessibility`,
and empty-children leaves. The property: `apply prev (diff prev next).Patch`
structurally equals `next` over ≥1000 cases; a second property asserts
`diff prev next = diff prev next` (determinism, SC-004).

**Rationale**: `'msg = int` makes generated trees fully structural-equatable.
Bounded recursion depth (e.g. ≤4) and small key alphabets force frequent key
reuse and reorders so the generator actually exercises moves, inserts, removes,
and duplicate-key paths rather than degenerate identical trees. Structural
equality is the round-trip oracle; no normalization beyond attribute-order
canonicalization is needed because `apply` reconstructs `next`'s ordering from the
patch.

## R8 — Routing / governance (confirmed, no edit)

`src/Controls/**` already matches `controls-public-surface`
(`build/Governance/Routing.fs:128`), which prints the escalated `FocusedAuthority`
gate set and requires `readiness/typed-controls-front-door.md` +
`readiness/package-surface-expectations.md`. No `Routing.fs` change is needed;
the feature adds the third feature-specific artifact `readiness/keyed-reconciliation.md`.
