# Contract: Lowering Parity (the keystone — applies to all 41 controls)

This is the load-bearing correctness contract of the whole migration (FR-002,
SC-002). Every migrated typed `view` MUST lower to a `Control<'msg>` **structurally
equal** to the `Control<'msg>` the equivalent legacy builder produces for the same
logical inputs. It is the same proof method `065` used; `070` runs it 41 times.

## The contract

For each of the 41 controls, build the same logical control two ways and assert
structural equality after normalizing attribute order:

```fsharp
// Example — a control with a dedicated legacy *.create (Label):
let legacy = Label.create [ Label.text "Name" ]                         // existing API
let typed  = Typed.Label.view { Typed.Label.defaults with Text = "Name" } |> Widget.toControl
Expect.equal (normalize typed) (normalize legacy) "typed Label lowers to legacy IR"

// Example — a control whose only legacy path is the generic kind builder (ListView):
let model, _ = Collections.init "orders" 100 24.0 240.0
let legacy = Control.standard <list-view kind> [ (* the attrs the legacy path emits *) ]
let typed  = Typed.ListView.view { (Typed.ListView.defaults "orders") with Items = items } model |> Widget.toControl
Expect.equal (normalize typed) (normalize legacy) "typed ListView lowers to legacy IR"
```

`normalize` sorts `Attributes` by name (the `065` helper) so ordering is ignored;
`Kind`, `Key`, `Children`, `Content`, and `Accessibility` must match exactly.

## Per-control obligations

- **Pure controls** (display, input, containers, navigation, overlay,
  charts/graph without runtime state): one parity assertion over `view props`.
- **Stateful controls** (`text-area`, the five selection collections, any chart
  that owns runtime state): a parity assertion over `view props model` for a
  representative model state, **plus** an `update`-delegation assertion that the
  typed `update msg model` equals the reused model's `update msg model` exactly
  (FR-004/SC-003) — model and effects identical, no new effect type, no I/O.
- **Optional events**: an assertion that `event = None` lowers to **no** event
  binding and `event = Some m` lowers to the same binding the legacy path emits
  (FR-005), for every control that carries an event.
- **`custom-control`**: no parity row — its contract is the `Widget.ofControl`
  round-trip invariant `Widget.toControl (Widget.ofControl c) = c` (already proven
  in `065`), re-asserted for a representative custom control.

## Coverage

The parity matrix in `readiness/typed-lowering-parity.md` MUST have a row for each
of the 41 controls (40 parity rows + the `custom-control` bridge row), each marked
PASS. SC-002 requires 100% — no control divergent. Any control that cannot achieve
real parity in this feature carries the `[S]` disclosure (Principle V), is named in
the evidence, and is the documented exception (intent: zero `[S]`, FR-011).
