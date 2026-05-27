# Elmish/MVU Applicability

Task: T004

Generated apps in this feature are stateful and I/O-bearing.

Required model:

- `Model` captures generated app state.
- `Msg` captures user actions, external responses, and internal transitions.
- App commands describe app-owned requested work.
- `init` returns initial state plus app commands.
- `update` remains pure and returns next state plus app commands.
- Viewer rendering, persistent-window, and screenshot effects are produced or interpreted at the host boundary.

Required evidence:

- public `init` and `update` paths exercised
- transition and emitted-command assertions
- host interpreter evidence with real dependencies where safe
- generated docs and tests that keep app commands distinct from viewer effects

