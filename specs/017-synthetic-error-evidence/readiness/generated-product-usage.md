# Generated Product Usage

No generated product usage path changes are introduced by this feature.

- Default/minimal generated product runtime behavior is unchanged.
- The feature changes Spec Kit governance prompts, task templates, evidence
  graph output, and evidence audit output only.
- Generated task guidance now teaches `[SEH]` classification, but generated
  app code and sample product code are not modified for runtime behavior.

Validation:

- `GeneratedGuidanceCheck`, `TemplateCheck`, `TemplateSmoke`, and
  `GeneratedProductCheck` completed during focused and aggregate validation.
