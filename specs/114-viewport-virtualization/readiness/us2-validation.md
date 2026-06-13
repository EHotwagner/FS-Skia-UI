# US2 independent validation — overscan opt-in, at-rest byte-identical

**Story**: Overscan is opt-in and at-rest output is byte-identical.

## Path

With overscan at its default (0), confirm the realized rows, control geometry, and rendered scene are
byte-identical to the pre-feature path; with opt-in overscan N confirm only the visible rows plus up to N
real, edge-clamped adjacent rows materialize, the visible region is unchanged, and no rows are fabricated
or duplicated; confirm scrolling reuses keyed row containers where the diff permits.

## Evidence

- `tests/Controls.Tests/Feature114OverscanParityTests.fs` — overscan-0 realizes exactly the contiguous
  historic slice (same keys, byte-identical scene on repeat builds); opt-in overscan extends each edge by
  exactly N real contiguous rows with the visible region unchanged; edge-clamped (no `r-1` / past-the-end
  rows); a one-row scroll keeps most row keys stable (keyed reuse possible, FR-008).
- `byte-identity-authority.md` — the standing Scene-parity golden suite under `Dev` is the authority for
  the at-rest rendered-output + geometry clause.

Result: PASS (SC-002 / SC-003 / SC-007).
