#!/usr/bin/env bash
set -euo pipefail

IMAGE="${IMAGE:-fs-skia-ui-dev}"
CONTAINER="${CONTAINER:-fs-skia-ui-dev}"
SCRIPT_PATH="$(readlink -f "${BASH_SOURCE[0]}")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"
CONTAINERFILE="$SCRIPT_DIR/Containerfile.fs-skia-ui"
WORKSPACE="$(pwd)"
REBUILD=false
ATTACH=true
ATTACH_SHELL="/bin/bash"
ENABLE_GPU=true
ENABLE_DISPLAY=true
ENABLE_AUDIO=true
DOTNET_CHANNELS="${DOTNET_CHANNELS:-6.0 8.0 10.0}"
TZ_VALUE="${TZ:-UTC}"

PORT_FLAGS=(
    -p 5000:5000
    -p 5001:5001
    -p 5173:5173
    -p 8080:8080
    -p 8081:8081
)
CUSTOM_PORTS=false
EXTRA_ENV_FLAGS=()
EXTRA_VOLUME_FLAGS=()

usage() {
    cat <<'EOF'
Usage: create-fs-skia-ui-dev.sh [options]

Options:
  --workspace=PATH          Host project directory mounted at /workspace.
                            Defaults to the current directory.
  --container=NAME          Container name. Default: fs-skia-ui-dev.
  --image=NAME              Image name. Default: fs-skia-ui-dev.
  --dotnet-channels=LIST    Space-separated .NET channels. Default: "6.0 8.0 10.0".
  --timezone=ZONE           Timezone copied into the image. Default: UTC.
  --port=HOST:CONTAINER     Add or replace published ports. Repeatable.
  --env=KEY=VALUE           Pass an environment variable. Repeatable.
  --volume=SRC:DST[:OPTS]   Add a Podman volume mount. Repeatable.
  --rebuild                 Remove any existing container and rebuild image.
  --no-attach               Start container without entering a shell.
  --bash                    Attach with /bin/bash. This is the default.
  --no-gpu                  Do not pass GPU devices through.
  --no-display              Do not mount Wayland/X11 display sockets.
  --no-audio                Do not mount PulseAudio socket.
  --help                    Show this help.

Examples:
  ./Container/create-fs-skia-ui-dev.sh --workspace=$PWD --rebuild
  ./Container/create-fs-skia-ui-dev.sh --port=3000:3000 --env=ASPNETCORE_ENVIRONMENT=Development
EOF
}

for arg in "$@"; do
    case "$arg" in
        --workspace=*) WORKSPACE="${arg#*=}" ;;
        --container=*) CONTAINER="${arg#*=}" ;;
        --image=*) IMAGE="${arg#*=}" ;;
        --dotnet-channels=*) DOTNET_CHANNELS="${arg#*=}" ;;
        --timezone=*) TZ_VALUE="${arg#*=}" ;;
        --port=*)
            if [[ "$CUSTOM_PORTS" == false ]]; then
                PORT_FLAGS=()
                CUSTOM_PORTS=true
            fi
            PORT_FLAGS+=(-p "${arg#*=}")
            ;;
        --env=*) EXTRA_ENV_FLAGS+=(-e "${arg#*=}") ;;
        --volume=*) EXTRA_VOLUME_FLAGS+=(-v "${arg#*=}") ;;
        --rebuild) REBUILD=true ;;
        --no-attach) ATTACH=false ;;
        --bash) ATTACH_SHELL="/bin/bash" ;;
        --no-gpu) ENABLE_GPU=false ;;
        --no-display) ENABLE_DISPLAY=false ;;
        --no-audio) ENABLE_AUDIO=false ;;
        --help) usage; exit 0 ;;
        *) echo "Unknown option: $arg" >&2; usage; exit 1 ;;
    esac
done

WORKSPACE="$(readlink -f "$WORKSPACE")"
if [[ ! -d "$WORKSPACE" ]]; then
    echo "ERROR: workspace does not exist: $WORKSPACE" >&2
    exit 1
fi
mkdir -p "${HOME}/.local/share/nuget-local"

attach_container() {
    [[ "$ATTACH" == true ]] || exit 0
    echo "Connecting to container '$CONTAINER'..."
    exec podman exec -it "$CONTAINER" "$ATTACH_SHELL"
}

detect_gpu() {
    if command -v nvidia-smi >/dev/null 2>&1 && nvidia-smi >/dev/null 2>&1; then
        echo "nvidia"
    elif [[ -d /dev/dri ]]; then
        echo "dri"
    else
        echo "none"
    fi
}

x_socket_path() {
    case "${DISPLAY:-}" in
        :*)
            local display_number="${DISPLAY#:}"
            display_number="${display_number%%.*}"
            printf '/tmp/.X11-unix/X%s' "$display_number"
            ;;
        *) return 1 ;;
    esac
}

container_status="$(podman ps -a --format '{{.Names}}|{{.Status}}' | awk -F'|' -v name="$CONTAINER" '$1 == name { print $2; exit }')"

if [[ -n "$container_status" ]]; then
    if [[ "$container_status" == Up* ]]; then
        state="running"
    else
        state="$container_status"
    fi

    if [[ "$REBUILD" == true ]]; then
        echo "Rebuilding: removing existing container ($state)"
        podman rm -f "$CONTAINER"
    elif [[ "$state" == "running" ]]; then
        echo "Container '$CONTAINER' is already running."
        attach_container
    else
        echo "Starting existing container '$CONTAINER' (was $state)"
        podman start "$CONTAINER"
        echo "Container '$CONTAINER' is running."
        attach_container
    fi
fi

if [[ "$ENABLE_GPU" == true ]]; then
    GPU="$(detect_gpu)"
else
    GPU="disabled"
fi
echo "GPU mode: $GPU"

echo "Building image: $IMAGE"
podman build \
    --pull=always \
    --build-arg "CONTAINER_CACHEBUST=$(date -u +%Y-%m-%dT%H:%M:%SZ)" \
    --build-arg "DOTNET_CHANNELS=$DOTNET_CHANNELS" \
    --build-arg "TZ=$TZ_VALUE" \
    -t "$IMAGE" \
    -f "$CONTAINERFILE" \
    "$SCRIPT_DIR/.."

GPU_FLAGS=()
case "$GPU" in
    nvidia) GPU_FLAGS=(--device nvidia.com/gpu=all) ;;
    dri) GPU_FLAGS=(--device /dev/dri) ;;
esac

HOST_UID="$(id -u)"
CONTAINER_RUNTIME_DIR="/run/user/$HOST_UID"
RUNTIME_FLAGS=()
if [[ -n "${XDG_RUNTIME_DIR:-}" && -d "$XDG_RUNTIME_DIR" ]]; then
    RUNTIME_FLAGS=(
        -e XDG_RUNTIME_DIR="$CONTAINER_RUNTIME_DIR"
        -v "${XDG_RUNTIME_DIR}:${CONTAINER_RUNTIME_DIR}:rw"
    )
fi

DISPLAY_FLAGS=()
if [[ "$ENABLE_DISPLAY" == true ]]; then
    if [[ -n "${WAYLAND_DISPLAY:-}" && -n "${XDG_RUNTIME_DIR:-}" && -S "${XDG_RUNTIME_DIR}/${WAYLAND_DISPLAY}" ]]; then
        DISPLAY_FLAGS+=(-e WAYLAND_DISPLAY="$WAYLAND_DISPLAY")
    fi

    XAUTHORITY_VALUE="${XAUTHORITY:-$HOME/.Xauthority}"
    if [[ -n "${DISPLAY:-}" ]] && x_socket="$(x_socket_path)" && [[ -S "$x_socket" && -f "$XAUTHORITY_VALUE" ]]; then
        DISPLAY_FLAGS+=(
            -e DISPLAY="$DISPLAY"
            -e XAUTHORITY=/tmp/.Xauthority
            -v /tmp/.X11-unix:/tmp/.X11-unix:rw
            -v "${XAUTHORITY_VALUE}:/tmp/.Xauthority:ro"
        )
    fi
fi

PULSE_FLAGS=()
if [[ "$ENABLE_AUDIO" == true && -n "${XDG_RUNTIME_DIR:-}" && -S "${XDG_RUNTIME_DIR}/pulse/native" ]]; then
    PULSE_FLAGS=(
        -e PULSE_SERVER=unix:/tmp/pulse-socket
        -v "${XDG_RUNTIME_DIR}/pulse/native:/tmp/pulse-socket:ro"
    )
fi

echo "Creating container: $CONTAINER"
podman run -dt \
    --name "$CONTAINER" \
    --stop-timeout=15 \
    --userns=keep-id \
    "${GPU_FLAGS[@]}" \
    "${RUNTIME_FLAGS[@]}" \
    "${DISPLAY_FLAGS[@]}" \
    "${PULSE_FLAGS[@]}" \
    "${PORT_FLAGS[@]}" \
    "${EXTRA_ENV_FLAGS[@]}" \
    "${EXTRA_VOLUME_FLAGS[@]}" \
    -e GIT_USER_NAME="${GIT_USER_NAME:-}" \
    -e GIT_USER_EMAIL="${GIT_USER_EMAIL:-}" \
    -v "$WORKSPACE:/workspace:rw" \
    -v "${HOME}/.local/share/nuget-local:/home/developer/.local/share/nuget-local:rw" \
    "$IMAGE"

echo ""
echo "Container '$CONTAINER' is running."
echo "Workspace: $WORKSPACE -> /workspace"

case "$GPU" in
    nvidia)
        podman exec "$CONTAINER" nvidia-smi --query-gpu=name,driver_version --format=csv,noheader || true
        ;;
    dri)
        podman exec "$CONTAINER" bash -lc 'ls /dev/dri 2>/dev/null' && echo "DRI GPU passthrough: OK" || echo "DRI GPU passthrough: unavailable"
        ;;
    none|disabled)
        echo "GPU passthrough: $GPU"
        ;;
esac

if [[ "${#DISPLAY_FLAGS[@]}" -gt 0 ]]; then
    podman exec "$CONTAINER" bash -lc 'xeyes >/tmp/xeyes.log 2>&1 &' && echo "Display forwarding: xeyes started for verification" || echo "Display forwarding: could not verify"
else
    echo "Display forwarding: not configured"
fi

podman exec "$CONTAINER" bash -lc 'dotnet --version && fsautocomplete --help >/dev/null && echo "F# tooling: OK"'
attach_container
