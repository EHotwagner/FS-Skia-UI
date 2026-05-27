# HUD Layout Readability Evidence

## Status

T016 generated product HUD layout evidence passed for 1280x720 and 640x480.

## Commands

- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`
- `dotnet run --project template/base/src/Product/Product.fsproj --no-build -- --layout-evidence specs/020-asteroids-integration-feedback/readiness/generated-layout-1280x720.txt 1280 720`
- `dotnet run --project template/base/src/Product/Product.fsproj --no-build -- --layout-evidence specs/020-asteroids-integration-feedback/readiness/generated-layout-640x480.txt 640 480`

## Evidence

The generated product executable wrote:

- `specs/020-asteroids-integration-feedback/readiness/generated-layout-1280x720.txt`
- `specs/020-asteroids-integration-feedback/readiness/generated-layout-640x480.txt`

Both reports state `proof-level=ReadableLayout`, named HUD and gameplay
regions, four HUD text bounds, one active gameplay bound, `NoLayoutOverlap`,
and `accepted=True`.

```text
output-size=1280x720
proof-level=ReadableLayout
hud-region=hud:0,0,1280,96
gameplay-region=gameplay:0,96,1280,624
overlap-status=NoLayoutOverlap
accepted=True

output-size=640x480
proof-level=ReadableLayout
hud-region=hud:0,0,640,96
gameplay-region=gameplay:0,96,640,384
overlap-status=NoLayoutOverlap
accepted=True
```
