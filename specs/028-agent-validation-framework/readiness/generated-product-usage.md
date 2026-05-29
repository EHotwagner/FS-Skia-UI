# Generated Product Usage

Status: PASS

Generated product validation remains explicit and evidence-command driven.
Default generated product launch stays product-owned and evidence-free unless
the user invokes a governed evidence command.

Relevant gates:

- `TemplateCheck`
- `GeneratedProductCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- `TargetMetadataDrift`

Generated app guidance now treats target metadata and command compatibility as
part of the validation contract. Generated workflows should continue to call
documented `./fake.sh build -t <Target>` commands rather than inferring target
behavior from prose.
