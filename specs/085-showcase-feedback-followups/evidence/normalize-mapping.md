# normalize key-name mapping evidence (085, US3 — SC-003, FR-007/FR-008)

evidence-kind=unit-test
status=ok
test=tests/KeyboardInput.Tests "Feature 085 normalize key-name families (US3)" (5/5 pass)
source=src/KeyboardInput/KeyboardInput.fs ViewerKeyboard.normalize

## Mapping (ViewerKeyboard.normalize)

raw=Number5 -> Digit 5
raw=Digit5 -> Digit 5
raw=Keypad5 -> Digit 5
raw=Key5 -> Digit 5
raw=KeyL -> Letter 'L'
case-insensitive=true (number5/DIGIT5/KeyPad5/kEy5 -> Digit 5; keyl -> Letter 'L')
raw=Totally-Unknown -> Unknown "Totally-Unknown"
raw=Number (prefix only) -> Unknown "Number"
raw=KeyLong (multi-char suffix) -> Unknown "KeyLong"

## No regression (existing names unchanged)

raw=Left -> ArrowLeft
raw=F5 -> Function 5
raw=L -> Letter 'L'
raw=5 -> Digit 5

## Contract

The `ViewerKey` union and `KeyboardInput.fsi` are **unchanged** — this is a behavior-only
addition of match arms (Number/Digit/Keypad digit families + Key{n}/Key{X}) placed before the
terminal `| _ -> Unknown raw` arm, which is preserved (totality, FR-008).
