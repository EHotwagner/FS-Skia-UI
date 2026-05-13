# KeyboardInputGallery Live Example

Run the Skia app:

```bash
dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj
```

The sample starts with `colemacs-dh` (`Colemak-DH`) as the active layout, captures window keyboard events through `ViewerEvent.KeyDown` / `ViewerEvent.KeyUp`, and renders the active layout as part of the Skia scene. It also exposes these layout profiles for switching:

- `qwerty` - QWERTY
- `dvorak` - Dvorak
- `colemacs-dh` - Colemak-DH
- `workman` - Workman
- `symbols` - Custom Symbols

Input commands:

- `h` resolves movement on the physical `KeyH` position.
- `l` resolves movement on the physical `KeyL` position.
- `1` changes the stateful selection mode to `line`.
- `space`, then `h`, pushes and resolves the popup space mode.
- `c`, then `h`, then `release-c`, demonstrates a temporary held copy mode.
- `d`, then `h`, then `release-d`, demonstrates a temporary held delete mode.
- `Q`, `V`, `K`, `W`, and `S` switch layouts.
- `F2` toggles the visible layout overlay.
- Close the window to exit.
