# Quickstart: Sokoban Feedback Follow-ups

Run FAKE-backed commands one at a time. Do not run `./fake.sh`, `fake.cmd`, or `dotnet fake` concurrently because they share repository `.fake` state.

## Focused Validation Order

1. `dotnet tool restore`
2. `./fake.sh build -t Dev`
3. `./fake.sh build -t GeneratedGuidanceCheck`
4. `./fake.sh build -t TemplateCheck`
5. `./fake.sh build -t GeneratedProductCheck`
6. `./fake.sh build -t EvidenceGraph`
7. `./fake.sh build -t EvidenceAudit`

Use `./fake.sh build -t Verify` only after focused targets are clean, or as the single broad FAKE-backed command for a final pass.

## Required Readiness Evidence

Record the implementation evidence under:

```text
specs/032-sokoban-feedback-followups/readiness/
```

Required files:

- `default-text-glyph-capture.md`
- `interactive-window-close-evidence.md`
- `consumer-guidance-scan.md`
- `readiness-contract-scan.md`
- `task-guidance-scan.md`

## Validation Notes

- Default text evidence must include a decodable screenshot and glyph-shaped coverage checks.
- Persistent close evidence must prove real interactive-window launch, first frame, close request, clean exit, and accepted status.
- Guidance scans must find default text, interactive close evidence, consumer API map, readiness contract, and task validator pitfalls.
- If any FAKE-backed failure looks race-like or concurrent FAKE context is unknown, rerun the affected FAKE-backed commands sequentially before product debugging.
