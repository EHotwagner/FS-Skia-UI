# Keyboard Input Evidence Notes

- Tier: Tier 1 contracted public API change in `FS.Skia.UI.KeyboardInput`.
- Parser: `YamlDotNet` pinned to `17.1.0` in `src/Lib/Lib.fsproj`.
- MVU boundary: `InputRuntime` is stateful; `KeyboardInput.update` is pure and emits `InputEffect` values for host interpretation.
- Synthetic evidence: none planned or used. Checked-in YAML and replay fixtures are real repository fixtures.
- Unsupported v1 scope: touch, gamepad, automatic keymap rewriting, executable YAML host actions, and full command grammar execution.
- Evidence obligations: semantic tests, FSI transcript, package surface baseline, sample smoke, and performance timing evidence.
