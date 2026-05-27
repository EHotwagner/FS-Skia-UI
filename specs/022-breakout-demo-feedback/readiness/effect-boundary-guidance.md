# Effect Boundary Guidance

## Status

status=ok
story=US4
generated-source=template/base/src/Product/Program.fs
generated-tests=template/base/tests/Product.Tests/Tests.fs

## Contracts

- app-model=`Product.Program.Model`
- app-message=`Product.Program.Msg`
- app-command=`FS.Skia.UI.Controls.Elmish.AdapterCommand<Product.Program.Msg>`
- pure-update=`Product.Program.update`
- host-boundary=`Product.Program.interpretAtHostBoundary`
- viewer-effect=`FS.Skia.UI.SkiaViewer.ViewerEffect`
- generated-host=`Product.Program.generatedHost`
- persistent-launch=`Viewer.runApp viewerOptions Product.Program.generatedHost`

## Evidence

- `Product.Program.update (ViewerInput(Enter, true)) initialModel` transitions
  from `Initial` to `Main` and emits no app commands.
- `Product.Program.update SaveRequested initialModel` emits
  `DispatchHostCommand "save:Product"` as an app command without mutating the
  model.
- `Product.Program.interpretAtHostBoundary SaveRequested initialModel` returns
  the pure update result, exposes the app commands, and emits
  `RenderScene(view next)` as the viewer effect list.
- `Product.Program.generatedHost.Update SaveRequested initialModel` returns
  only viewer effects to SkiaViewer; app commands are not appended to viewer
  effect lists or relabeled as viewer effects.

## Reviewer Checklist

- app-command category can be identified by searching for
  `AdapterCommand<Product.Program.Msg>` and `DispatchHostCommand`.
- viewer-effect category can be identified by searching for `ViewerEffect` and
  `RenderScene`.
- host interpretation can be identified by searching for
  `interpretAtHostBoundary`.
- expected reviewer time: under 2 minutes in `template/base/src/Product/Program.fs`
  and `template/base/tests/Product.Tests/Tests.fs`.

## Verification

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore --filter "Generated guidance hardening" --logger "console;verbosity=minimal"`:
  15 passed.
- `./fake.sh build -t GeneratedGuidanceCheck`:
  passed; output saved to `readiness/generated-guidance-check-us4.log`.
- `./fake.sh build -t TemplateCheck`:
  passed; output saved to `readiness/template-check-us4.log`.
