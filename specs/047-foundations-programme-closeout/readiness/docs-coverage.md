# Doc-coverage proof (US3 — T013, FR-006/FR-007, SC-004)

Per-surface evidence that each of the five contributor-facing surfaces describes the
new development model's **four required concepts** and that **none** presents the
serialized six-target order as the unconditional default (it is the escalated
`maintainer-verify` path). Captured at the pinned SHA `4276bd0`.

The four required concepts:

1. **Two-tier process** (`inner-loop` light tier vs escalated tiers)
2. **`Route` entry point** (`./fake.sh build -t Route`)
3. **Governance library as the single home of all rules** (`FS.Skia.UI.Build`)
4. **Generate-don't-sync** (artifacts generated from a single source, not hand-synced)

## Per-surface coverage

| Surface | Two-tier | `Route` entry | Governance lib (single home) | Generate-don't-sync | Serialized order = escalated (not default) |
|---|---|---|---|---|---|
| `README.md` | ✅ "two-tier process", `inner-loop` | ✅ `./fake.sh build -t Route` | ✅ `FS.Skia.UI.Build`, "single source, not hand-synced" | ✅ routing contract + `.claude` mirror "generated from a single source" | ✅ "full serialized gate order is the escalated path, not the default" |
| `docs/reports/build.md` | ✅ tiers incl. `inner-loop` | ✅ `./fake.sh build -t Route` | ✅ "All build/governance rules live in the single compiled library `FS.Skia.UI.Build`" | ✅ `validation.contract.yml` from `Routing.fs`; `.claude` from `.agents` | ✅ serialized order only "when `Route` escalates to several gates" |
| `docs/reports/speckit.md` | ✅ `inner-loop` vs escalate | ✅ `./fake.sh build -t Route` | ✅ "single home of all rules" (`build/Governance/**`) | ✅ "generated from a single source, not hand-synced" | ✅ "Spec Kit work no longer applies the full serialized governance order to every change" |
| `CLAUDE.md` | ✅ `inner-loop` tier | ✅ `./fake.sh build -t Route` | ✅ "single home of all rules" | ✅ `validation.contract.yml` + `.claude` tree generated | ✅ "escalated `maintainer-verify` path … no longer the unconditional default" |
| `AGENTS.md` | ✅ `inner-loop` tier | ✅ `./fake.sh build -t Route` | ✅ "single home of all rules" | ✅ `validation.contract.yml` + `.claude` tree generated, "neither can drift" | ✅ "escalated `maintainer-verify` path … no longer the unconditional default" |

## Reproduction

```bash
for f in README.md docs/reports/build.md docs/reports/speckit.md CLAUDE.md AGENTS.md; do
  grep -ciE 'inner-loop|two-tier' "$f"        # two-tier   (>0 each)
  grep -ci 'build -t Route' "$f"              # Route      (>0 each)
  grep -ciE 'FS\.Skia\.UI\.Build|single home of all rules' "$f"  # gov lib (>0 each)
  grep -ciE 'generated from|not hand-synced'  "$f"  # generate-don't-sync (>0 each)
  grep -ciE 'escalated|maintainer-verify|not the unconditional|no longer the unconditional|not the default' "$f"  # serialized=escalated (>0 each)
done
```

All five surfaces return a non-zero count for every concept (verified at the pinned SHA).

## FR-007 — no unconditional serialized-order instruction

No surface instructs readers to run the full serialized six-target order as the
unconditional default. `CLAUDE.md` and `AGENTS.md` explicitly label it the **escalated
`maintainer-verify` path** ("no longer the unconditional default — run it only when
`Route` escalates"); `build.md` and `speckit.md` gate it behind `Route` escalation;
`README.md` states "the full serialized gate order is the escalated path, not the
default." The stale `branch-vs-`master`` reference and the `fake-cli`/`build.fsx`
wrapper claim in `build.md` were corrected in this pass (see
`readiness/scaffolding-proof.md` §3). Verdict: **SC-004 satisfied**.
