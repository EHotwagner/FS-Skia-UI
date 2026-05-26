# Game Visual Evidence

supported-host=true
evidence-kind=screenshot
board-readable=true
input-or-progress-observed=true
fallback-reason=none
command=dotnet run --project template/base/src/Product/Product.fsproj -- --screenshot-evidence specs/018-persistent-gui-runtime/readiness/logs/t038-game-screenshot-evidence.txt

supported-host=true
evidence-kind=pixel-readback
board-readable=true
input-or-progress-observed=true
fallback-reason=screenshot-unavailable
command=dotnet run --project template/base/src/Product/Product.fsproj -- --pixel-readback-evidence specs/018-persistent-gui-runtime/readiness/logs/t038-game-pixel-readback-evidence.txt

Evidence logs:
- `specs/018-persistent-gui-runtime/readiness/logs/t038-screenshot-command.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t038-game-screenshot-evidence.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t038-pixel-readback-command.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t038-game-pixel-readback-evidence.txt`

Screenshot is the preferred generated game proof. Pixel-readback is recorded
only as fallback evidence with an explicit screenshot-unavailable reason. Both
records include readable board/grid proof and input-or-progress observation.
