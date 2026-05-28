# Contract: Generated Framework Guidance

## Scope

Applies to generated app docs, template fragments, generated source comments where appropriate, and generated product tests.

## Required Topics

### App Message Qualification

Generated guidance must show how to qualify app-owned message cases when opened viewer namespaces contain lifecycle names with the same identifier. `CloseRequested` is the required example.

### Domain Vector to Scene Point Conversion

Generated game guidance must show a small conversion helper from app-owned vector records to scene point records and explain that structurally similar fields still need explicit conversion.

### Semantic Scene Evidence

Generated evidence guidance must state that deterministic scene evidence proves stable rendering metadata and scene facts, but does not by itself prove semantic object presence such as lander, terrain, landing pad, or HUD metrics unless the app reports those facts explicitly.

### Screenshot and Fallback Vocabulary

Generated evidence guidance must distinguish:

- live screenshot proof
- pixel-readback fallback
- deterministic scene evidence
- unsupported host evidence
- failed screenshot attempts

Fallback records must include `fallback-reason`, `deterministic-fallback-kind` when relevant, and `proves-screenshot=false`.

## Verification

- `GeneratedGuidanceCheck` verifies generated guidance includes each required topic.
- Template/generated product tests verify the required examples appear in generated output.
- Audit fixtures reject wording that claims screenshot proof from deterministic scene evidence or pixel-readback-only fallback.
