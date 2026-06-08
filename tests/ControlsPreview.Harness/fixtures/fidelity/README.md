<!-- (* SYNTHETIC FIXTURE: gate test vectors, not product evidence. *) -->
# Fidelity gate fixtures (feature 080, SC-003)

These PNGs are the durable **red→green guard** for `ControlFidelityCheck`. They are synthetic
gate *test vectors*, **not** product evidence (the catalog previews in `docs/img/controls/` are
the real renders), so they are disclosed as a fixture set per Principle V — no task is `[S]`.

- `lowfi/<id>.png` — retained 079-style label-on-a-box renders (the pre-fix schematic previews).
  The gate MUST report each as **fail** against `<id>`'s pixel signature (≈0 coverage outside the
  title band).
- `faithful/<id>.png` — the regenerated faithful counterpart. The gate MUST report each as
  **pass**.

A label-on-a-box can never again pass the gate: if the renderer regresses, `faithful/*` stops
passing and/or `lowfi/*` starts passing, and `ControlFidelityCheck` fails with a named mismatch.
