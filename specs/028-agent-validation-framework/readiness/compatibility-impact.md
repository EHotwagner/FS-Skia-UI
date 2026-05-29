# Compatibility Impact

Status: PASS

Stable command compatibility is preserved:

- `./fake.sh build -t BuildWorkflowCheck` passed under native FAKE target
  registration.
- `./fake.sh build --list` exposes the runnable native target registry.
- `TargetMetadataDrift` passed after docs and `validation.contract.yml` were
  updated.

The local toolchain changed from `fake-cli` 5.23.1 to 6.1.4. Because this
container has a runtime-only `/usr/share/dotnet` host, `fake.sh` sets
`FAKE_SDK_RESOLVER_CUSTOM_DOTNET_PATH=/home/developer/.dotnet` by default.
Future container images should install a full SDK under the default dotnet root
or remove the runtime-only system dotnet host.
