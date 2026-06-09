# Evidence Audit Evidence

`EvidenceAudit` ran with synthetic-propagation and diff-scan outputs present
(`readiness/logs/evidence-audit.txt`, `readiness/diff-scan-hits.json`).

diff-scan-hits=0 (no blocking pattern hits). readiness-contract-hits=0. window-visibility
contract files present and observed. persistent-launch evidence present (supported host).

Remaining synthetic signals are the keyboard warm-up keystroke-delivery captures (SC-007,
T035/T036/T038): native keystroke INJECTION within the focus window is deferred (085 also
deferred live native injection). The warm-up FIFO is real, deterministic host code that builds
and is documented; the live window itself is proven (interactive-visible-window.md). These
`[S]`/`[S*]` are accepted via `--accept-synthetic` with written justification recorded in
`readiness/synthetic-evidence.json` and mirrored to the PR. Accepted synthetic evidence remains
synthetic and is reported separately from real task evidence; `--accept-synthetic` is logged and
never changes the verdict (Principle V).
