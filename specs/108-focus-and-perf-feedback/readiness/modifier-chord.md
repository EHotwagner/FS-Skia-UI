# Modifier-aware key boundary proof (T030, SC-009, FR-016)

enforcing-test=tests/KeyboardInput.Tests/Feature108ModifierTests.fs

`ViewerKeyboard.normalizeEventWithModifiers : ViewerKeyEvent -> ViewerKey * bool * KeyModifiers` strips
`Ctrl+`/`Alt+`/`Shift+`/`Meta+` prefixes (any order, case-insensitive; `Cmd`/`Win`/`Super` alias to
`Meta`) into a base `ViewerKey` + a `KeyModifiers { Ctrl; Alt; Shift; Meta }` record:

| Raw key | base ViewerKey | modifiers |
|---------|----------------|-----------|
| "L" | Letter 'L' | noModifiers (byte-identical to `normalizeEvent`) |
| "Ctrl+L" | Letter 'L' | { Ctrl = true; … } |
| "shift+CTRL+alt+meta+ArrowLeft" | ArrowLeft | all four true |
| "Cmd+S" / "Win+S" / "Super+S" | Letter 'S' | { Meta = true } |

An unmodified key recovers `noModifiers` and the SAME `ViewerKey` as `normalizeEvent` (zero silent
loss; byte-identical routing). The live `runInteractiveApp` loop consults the additive `MapKeyChord`
seam before `MapKey` via `chordFallthrough`: a chord survives the backend as `ViewerKey.Unknown
"Ctrl+L"`, whose modifiers are re-parsed here and offered to `MapKeyChord` (default `None` → defers to
`MapKey`, so unmodified keys are byte-identical, SC-012).
