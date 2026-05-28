# Generated Product Usage

Status: reviewed.

Generated app products expose screenshot capture only through the explicit
`--screenshot-evidence <path>` command. The default interactive launch branch
does not run screenshot capture, write screenshot files, or self-close for
evidence.

Validation:

- `./fake.sh build -t GeneratedGuidanceCheck`: PASS
- `./fake.sh build -t GeneratedProductCheck`: PASS
- `./fake.sh build -t TemplateCheck`: PASS
