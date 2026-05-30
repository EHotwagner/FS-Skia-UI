# FS Skia UI Development Container

This is a general-purpose Podman container for F# application development that needs Skia UI or SkiaSharp-style graphics support. It is intentionally not personalized: it does not install Ki, Emacs, Codex, Claude, custom keyboard settings, private repositories, Git identity, or agent skill sync jobs.

The container gives consumers a clean base they can adapt for their own editor, project layout, package feeds, and GPU/display requirements.

## Files

| File | Purpose |
|---|---|
| `Container/Containerfile.fs-skia-ui` | Reusable Arch Linux image with .NET SDKs, F# tools, native graphics libraries, Node/npm, and basic CLI utilities. |
| `Container/create-fs-skia-ui-dev.sh` | Host launcher that builds the image, creates a rootless Podman container, mounts a workspace, and forwards GPU/display/audio where available. |
| `Container/fs-skia-ui-entrypoint.sh` | Minimal startup script that prepares runtime directories and the local NuGet feed. |
| `Container/fsautocomplete-lsp` | Optional LSP shim for tools that need a stable fsautocomplete executable path. |

## What Is Included

- Arch Linux base image.
- .NET SDK channels `6.0`, `8.0`, and `10.0` by default. The repo targets .NET 10, while FAKE currently needs .NET 6 reference assemblies.
- F# tools: `fsautocomplete`, `fantomas`, `fake-cli`, `paket`, and `fable`.
- Native graphics/UI libraries commonly needed by SkiaSharp, GTK, OpenGL, Vulkan, X11, Wayland, and font rendering.
- Node/npm plus `prettierd`, useful for Fable/web companion projects.
- A non-root `developer` user.
- A mounted workspace at `/workspace`.
- A local NuGet feed at `/home/developer/.local/share/nuget-local`, also mounted from the host at `~/.local/share/nuget-local`.

## What Is Not Included

- No personal editor.
- No Git user name or email unless you pass them at runtime.
- No GitHub auth setup.
- No Codex, Claude, or other agent tooling.
- No project-specific repository clones.
- No automatic package packing on startup.
- No private package sources.

## Quick Start

From the repository root:

```bash
chmod +x Container/create-fs-skia-ui-dev.sh Container/fs-skia-ui-entrypoint.sh
./Container/create-fs-skia-ui-dev.sh --workspace="$PWD" --rebuild
```

The script builds `Containerfile.fs-skia-ui`, starts a container named `fs-skia-ui-dev`, mounts the selected workspace at `/workspace`, verifies basic F# tooling, and attaches a Bash shell.

Inside the container:

```bash
cd /workspace
dotnet restore
dotnet build
dotnet run --project path/to/App.fsproj
```

## Common Script Options

```bash
# Use a specific project directory as /workspace.
./Container/create-fs-skia-ui-dev.sh --workspace=/path/to/FS-Skia-UI

# Rebuild the image and recreate the container.
./Container/create-fs-skia-ui-dev.sh --rebuild

# Start without attaching a shell.
./Container/create-fs-skia-ui-dev.sh --no-attach

# Publish different ports. The first --port replaces the defaults.
./Container/create-fs-skia-ui-dev.sh --port=3000:3000 --port=5173:5173

# Pass runtime environment variables.
./Container/create-fs-skia-ui-dev.sh --env=ASPNETCORE_ENVIRONMENT=Development

# Add an extra host mount.
./Container/create-fs-skia-ui-dev.sh --volume="$HOME/.nuget:/home/developer/.nuget-host:ro"

# Disable host graphics/audio integration for headless builds.
./Container/create-fs-skia-ui-dev.sh --no-gpu --no-display --no-audio
```

You can also set these environment variables before running the script:

```bash
export IMAGE=my-fs-skia-ui-dev
export CONTAINER=my-fs-skia-ui-dev
export DOTNET_CHANNELS="6.0 8.0 10.0"
export TZ=America/New_York
export GIT_USER_NAME="Your Name"
export GIT_USER_EMAIL="you@example.com"
```

## Adapting The Image

Most consumers should make small, explicit changes to `Containerfile.fs-skia-ui`:

- Add a preferred editor by installing it with `pacman`, `npm`, or a vendor script.
- Add project-specific native libraries near the existing graphics packages.
- Add extra .NET SDK channels with `--dotnet-channels` instead of hardcoding them.
- Add private NuGet sources at runtime or in a derived image, not in the shared base image.
- Add project bootstrap commands to a derived entrypoint if they are specific to one repository.

A derived image is usually cleaner than editing the base directly:

```Dockerfile
FROM fs-skia-ui-dev

USER root
RUN pacman -Syu --noconfirm helix && pacman -Scc --noconfirm
USER developer
```

Build it with:

```bash
podman build -t my-fs-skia-ui-dev -f Containerfile.my-fs-skia-ui .
```

## Local NuGet Feed

The launcher mounts this host path:

```text
~/.local/share/nuget-local
```

to this container path:

```text
/home/developer/.local/share/nuget-local
```

The entrypoint registers that path as a NuGet source named `local-feed`. To test local packages:

```bash
dotnet pack src/MyLibrary/MyLibrary.fsproj -c Release -o ~/.local/share/nuget-local
dotnet restore
```

If a package version is already cached and restore keeps using the old copy:

```bash
dotnet nuget locals all --clear
dotnet restore --no-cache
```

## Display And GPU Forwarding

The launcher detects and forwards what it can:

- NVIDIA: passes `--device nvidia.com/gpu=all` when `nvidia-smi` works on the host.
- Generic Linux DRI: passes `/dev/dri` when present.
- Wayland: mounts `XDG_RUNTIME_DIR` and forwards `WAYLAND_DISPLAY` when the socket exists.
- X11: mounts `/tmp/.X11-unix` and `$XAUTHORITY` when available.
- PulseAudio/PipeWire Pulse: mounts the Pulse native socket when available.

After startup, the script runs `xeyes` in the background when display forwarding is configured. That is only a visibility test; close the small window after confirming display forwarding.

### X11 Permission Problems

If GUI windows fail with access errors, the host X server may reject container clients.

Try a temporary local permission grant on the host:

```bash
xhost +SI:localuser:$(id -un)
```

If your Podman user namespace maps the container user differently, you may need a broader temporary test:

```bash
xhost +local:
```

Revoke it after testing:

```bash
xhost -local:
```

Prefer Wayland forwarding when the app stack supports it.

### GPU Problems

Check device visibility inside the container:

```bash
ls -la /dev/dri
glxinfo -B
vulkaninfo --summary
```

If `/dev/dri` is missing, start with `--no-gpu` to confirm the app works without acceleration, then fix host permissions or Podman device rules.

For NVIDIA, rootless Podman usually needs the NVIDIA container toolkit configured on the host. If `--device nvidia.com/gpu=all` fails, test the host runtime first with a small NVIDIA-enabled container before changing this image.

### Audio Problems

The script forwards the PulseAudio-compatible socket at:

```text
$XDG_RUNTIME_DIR/pulse/native
```

If audio does not work, verify the socket exists on the host and that the app is actually using PulseAudio/PipeWire Pulse.

## SELinux Hosts

Arch Linux usually does not enforce SELinux labels, so the launcher uses normal `:rw` mounts. On Fedora, RHEL, or other SELinux systems, add `:Z` to custom volume mounts:

```bash
./Container/create-fs-skia-ui-dev.sh --volume="$PWD:/workspace:Z"
```

If you adapt the script for SELinux, change the workspace mount from:

```bash
-v "$WORKSPACE:/workspace:rw"
```

to:

```bash
-v "$WORKSPACE:/workspace:Z"
```

## Common Build Failures

### `dotnet-install.sh` download fails

The image build needs internet access. Confirm the host can reach:

```text
https://dot.net
https://api.nuget.org
https://registry.npmjs.org
```

Then rerun:

```bash
./Container/create-fs-skia-ui-dev.sh --rebuild
```

### Pacman mirror fails

The image pins a short mirror list near the top of `Containerfile.fs-skia-ui`. Replace those mirror URLs with mirrors close to the consumer's region or organization.

### NuGet restore fails for private feeds

Do not bake private credentials into this general image. Prefer one of these:

```bash
dotnet nuget add source URL --name NAME --username USER --password TOKEN --store-password-in-clear-text
```

or mount a prepared NuGet config:

```bash
./Container/create-fs-skia-ui-dev.sh --volume="$HOME/.nuget/NuGet:/home/developer/.nuget/NuGet:ro"
```

## Common Runtime Failures

### `No usable version of libssl was found`

Install or pin the native OpenSSL package expected by your target framework. The base image includes current Arch `openssl`; older target frameworks or old package versions may expect an older ABI.

### `Unable to load shared library libSkiaSharp`

Confirm the project references the correct SkiaSharp native assets package for Linux. Depending on the project, that may be one of:

```xml
<PackageReference Include="SkiaSharp.NativeAssets.Linux" Version="..." />
<PackageReference Include="SkiaSharp.NativeAssets.Linux.NoDependencies" Version="..." />
```

Then clear NuGet caches and restore again:

```bash
dotnet nuget locals all --clear
dotnet restore --no-cache
```

### Fonts render incorrectly

The image includes `fontconfig` and `freetype2`, but not every desktop font. Add fonts in a derived image or mount a host font directory:

```bash
./Container/create-fs-skia-ui-dev.sh --volume="$HOME/.local/share/fonts:/home/developer/.local/share/fonts:ro"
fc-cache -fv
```

### File ownership looks wrong on the host

The launcher uses:

```bash
--userns=keep-id
```

That maps the container user to the host user for mounted files. If a different host setup still produces wrong ownership, remove and recreate the container after fixing `/etc/subuid`, `/etc/subgid`, or rootless Podman configuration.

## Maintenance Checklist

When adapting this for another project, keep the base image reusable:

- Keep personal tools out of `Containerfile.fs-skia-ui`.
- Put project-specific clone/build steps in a separate derived image or script.
- Keep credentials outside the image.
- Prefer runtime `--env` and `--volume` flags for local machine differences.
- Document any new native dependency with the error message that required it.
