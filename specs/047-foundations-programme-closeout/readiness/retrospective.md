# Dogfood retrospective + recurring-run mechanism (US4 — T014/T016, FR-009, SC-005)

Confirms the named **dogfood** features kept the consumer-governance harness honest, and
identifies the discoverable, runnable **recurring-run mechanism** that keeps it from
rotting now that framework-author work uses the light `inner-loop` tier by default.

## Dogfood features — full serialized pipeline exercised green

| `dogfood_feature` | Stage | Pipeline evidence | Outcome |
|---|---|---|---|
| **042** `042-foundations-two-tier-process` | Stage 1 (two-tier process) | `specs/042-foundations-two-tier-process/readiness/evidence-audit.md`, `…/focused-gates.md`; merged to `main` | full pipeline green, `EvidenceAudit` PASS, zero synthetic |
| **043** `043-foundations-evidence-engine` | Stage 4 (Python→F# evidence-engine port) | `specs/043-foundations-evidence-engine/readiness/evidence-audit.md`, `…/focused-gates.md`; byte-for-byte parity proven before deletion; merged to `main`, 34 tasks, `EvidenceAudit` PASS, zero synthetic | full pipeline green |

Feature 042 is the registered dogfood id in `Routing.fs`
(`dogfoodFeatureIds = [ "042" ]`), forced to the full pipeline regardless of its diff;
feature 043 ran the full serialized set as the highest-risk port, gated by the Stage-0
golden-evidence parity fixtures. Both squash-merged to `main` with `EvidenceAudit` PASS
and zero synthetic evidence.

**`harness_kept_honest`:** yes. The two dogfood features each drove the full consumer
ceremony — the serialized six-target gate set plus the in-process `EvidenceGraph` /
`EvidenceAudit` — to green, so the consumer-grade governance path was exercised end-to-end
even as framework-internal work shifted to the light `inner-loop` tier. The harness did
not silently weaken: every gate that protects a generated consumer ran on real features.

## Recurring-run mechanism (the standing protection)

To keep the consumer path exercised on a cadence — without standing up a live external CI
service — the recurring run is realized as a **committed, discoverable schedule
definition** plus a **documented manual fallback**:

| Field | Value |
|---|---|
| `schedule_file_path` | [`.specify/schedules/foundations-dogfood-pipeline.yml`](../../../.specify/schedules/foundations-dogfood-pipeline.yml) (under the existing `.specify/` governance surface, beside `extensions.yml`) |
| `schedule_spec` | Names the dogfood set (`042`, `043`), the full serialized six-target pipeline as the body to re-run, and a weekly cadence |
| `manual_fallback` | The serialized six-target command sequence, runnable by hand (below) |
| `no_live_ci` | `true` — complete when *defined, discoverable, and runnable*; no live CI service need exist |

### Manual full-pipeline fallback (runnable by hand)

Run from the repo root, **sequentially** (one target to completion before the next;
never concurrently — FAKE shares `.fake` state). On Windows use `fake.cmd`.

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Mechanism verification (T016, SC-005)

| Check | Command | Result |
|---|---|---|
| Schedule file is **tracked** | `git ls-files .specify/schedules/foundations-dogfood-pipeline.yml` | prints the path (tracked after commit) |
| Schedule file is **discoverable** | lives under `.specify/schedules/`, beside `extensions.yml`; linked from this retrospective and the after-baseline | a maintainer finds it without prior knowledge |
| Manual fallback is **runnable by hand** | the six `./fake.sh build -t <target>` commands above (each an existing, unchanged target) | runnable; no new target, no live CI |
| **No live-CI dependency** | `no_live_ci: true` in the schedule file; the fallback is local `./fake.sh` | satisfied |

`verdict = mechanism present, discoverable, and runnable` — no external CI service is
required to exist (FR-009).

## Cross-links (SC-005)

This retrospective is linked from the after-baseline
[`docs/reports/_baselines/2026-06-02-foundations-after.md`](../../../docs/reports/_baselines/2026-06-02-foundations-after.md),
which in turn links the closing ADR
[`docs/adr/0006-foundations-programme-closeout.md`](../../../docs/adr/0006-foundations-programme-closeout.md)
— so the Stage-7 closeout artifacts form a connected record.
