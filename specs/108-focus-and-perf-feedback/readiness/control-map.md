# Control.map / Widget.map proof (T030, SC-007, FR-014)

enforcing-test=tests/Controls.Tests/Feature108CompositionTests.fs

`Control.map f` changes ONLY the message type: `Kind`/`Key`/`Content`/`Accessibility`/`Children` shape
are preserved, and each `Attr`'s `AttrValue` is rewritten so only the handler-bearing cases thread `f`
(`MessageValue (f m)`, `EventValue (handler >> f)`). It lowers structurally equal to authoring directly
in `'b`:

```
Control.map Wrap (Button.create [ Button.text "x"; Button.onClick Inc ] |> Control.withKey "b1")
  ==(%A)==  Button.create [ Button.text "x"; Button.onClick (Wrap Inc) ] |> Control.withKey "b1"
```

(`Button.onClick` lowers to `MessageValue`, so the `%A` projection is exact, SC-007.) Keys / focus
identity survive a nested map (a keyed child keeps its `Key`). `Widget.map f = ofControl ∘ Control.map f
∘ toControl` — proven by `toControl (Widget.map Wrap (ofControl c)) ==(%A)== Control.map Wrap c`.
