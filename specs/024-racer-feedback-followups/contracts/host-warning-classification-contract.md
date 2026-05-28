# Host Warning Classification Contract

## Scope

Applies to readiness diagnostics and generated validation output that classify
host warnings observed during viewer launch.

## Benign GTK Warning Rules

The following GTK module warnings MAY be classified as benign host warnings:

- `Gtk-Message: Failed to load module "colorreload-gtk-module"`
- `Gtk-Message: Failed to load module "window-decorations-gtk-module"`

Classification as benign requires:

- first-frame launch evidence succeeded
- the warning text is preserved verbatim in evidence output
- no unrelated warning or error is being hidden by the benign classification
- the launch is not marked failed solely because these known warnings appeared

## Failure Rules

Unknown warnings, process exits, missing first-frame evidence, renderer errors,
or package/build failures MUST remain visible and MUST NOT be converted into
benign host warnings by broad GTK matching.

## Evidence

- `specs/024-racer-feedback-followups/readiness/host-warning-classification.md`
  records classifier input, preserved warning text, launch status, warning
  class, and final readiness status.
