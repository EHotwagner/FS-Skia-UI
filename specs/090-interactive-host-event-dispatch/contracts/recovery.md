# Contract: Nearest-Keyed-Ancestor Pointer Recovery (FR-004 / FR-004a / FR-005)

**Surface**: `src/Controls/**` — a new public, pure, option-returning resolver (working name
`nearestAuthored`), exported in the package `.fsi`. Tier 1, additive.

## Signature (FSI-first; drafted before `.fs`)

```fsharp
// Resolve a structural hit ControlId to the nearest ancestor (incl. self) that carries
// a withKey or an authored EventBinding, as that ancestor's authored ControlId. None when
// no keyed/bound ancestor exists on the hit node's path.
val nearestAuthored : result: ControlRenderResult<'msg> -> hit: ControlId -> ControlId option
```

## Guarantees

- **R1 (recovery).** A hit on a deep positional node (`"0.1"`, `"button"`) inside a **container-keyed**
  composite resolves to the **nearest** ancestor carrying a `withKey`/binding — the authored container
  identity (SC-003).
- **R2 (partial / honest).** When the hit node has **no** keyed-or-bound ancestor anywhere on its path,
  the result is **None**. It MUST NOT invent a `Kind`-based or root id the consumer never authored
  (FR-004a). On `None` the host falls back to `MapPointer` with the raw interaction.
- **R3 (non-regressive leaf).** A directly-keyed leaf resolves to **itself** (its `withKey` id) — a fixed
  point; only the previously-unroutable container-keyed (and the already-unroutable `None`) cases change
  (FR-005, SC-003 last clause).
- **R4 (total + deterministic).** Defined for every input; same input → same output; no clock/randomness;
  resume-safe. No layout-math change — it reads existing render/hit-test data only.

## Verification

- Container-keyed composite: `withKey` on the container, hit a point over an inner child →
  `nearestAuthored` returns the **container** id; the host then routes the container's binding (R1).
- Directly-keyed leaf: hit it → returns the leaf's own key (R3, regression guard).
- Unkeyed/unbound subtree: hit it → `None`; host falls back to `MapPointer` raw (R2).
- Property: `nearestAuthored` of an already-authored id = that id (idempotent fixed point).

Note: `Control<'msg>` has **no equality** ([[internal-module-in-controls-gotchas]]); tests compare the
returned `ControlId` (a `string`), not control values.
</content>
