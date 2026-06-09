---
phase: specify
date: 2026-06-08
severity: minor
---

## Process friction

The specify phase ran cleanly end-to-end (hook discovery → branch creation → spec +
checklist + source snapshot). One minor friction: the source spec was an external GitHub
URL, and the `WebFetch` tool returned a *summarized* rendering rather than the verbatim
markdown — which is unsafe for an in-repo provenance snapshot. I fell back to a direct
`curl` of the `raw.githubusercontent.com` URL to capture the exact source text for
`source-spec.md`. What would have helped: a documented convention in the specify skill to
prefer a raw fetch (or explicitly request verbatim, no-summarization output) when the
input is an external URL destined for a `source-spec.md` snapshot.

## Generalizable code

None. This phase produced specification Markdown only (spec.md, source-spec.md,
requirements checklist) — no F# code was written, so there is no support-library helper
candidate this phase.

## Skill gaps

Minor. A small helper skill/utility for "snapshot an external URL spec verbatim into the
feature directory" (handling GitHub `blob` → `raw` URL rewriting and guaranteeing
non-summarized capture) would have removed the manual curl step. Otherwise the existing
speckit-specify + feedback-capture skills covered the phase fully.

## Research links

No hard problem requiring external research arose this phase.
- Spec Kit hooks & extensions model: <https://github.com/github/spec-kit>
- GitHub raw content convention (blob → raw): <https://docs.github.com/en/repositories/working-with-files/using-files/viewing-and-understanding-files>
