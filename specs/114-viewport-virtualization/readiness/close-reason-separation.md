# close-reason-separation — applicability (feature 114, T002)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 114 opens no new window, so there is no new window close to classify. The evidence path is
deterministic and headless (the `Collections.visibleRange`/`DataGrid` overscan + realized-window tests,
the `DataGridModel` offscreen-relocation tests, the a11y total/position tests, the `Perf.runScript`
`VirtualItemsMaterialized`/`VirtualItemsTotal` metrics, and the standing Scene-parity suite); neither a
user close nor an evidence close occurs in this feature's evidence capture.
