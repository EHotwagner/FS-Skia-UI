# SC-007 — runtime untouched (Invariant 2)

$ git diff --stat master...HEAD -- 'src/'
(empty — 0 product src/** files changed across the whole branch)

$ git status --short -- 'src/'
(empty — no uncommitted product changes)

The runtime (Scene -> SkiaViewer -> Elmish) and every product `.fsi` are
untouched. Every 043 change lives in `build/Governance/**`, `build.fsx`,
`template/**`, `.specify/**`, `tests/Governance.Tests/**`, and `docs/**` — the
build-tooling + governance surface only (Invariant 2 / SC-007).
