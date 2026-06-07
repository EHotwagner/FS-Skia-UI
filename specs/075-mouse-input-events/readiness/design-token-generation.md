# Design Token Generation

PASS: the 20 generated tokens in src/Controls/DesignTokens.fs are a current, byte-identical regeneration of the DTCG source src/Controls/design-tokens.tokens.json.

- generated-tokens: 20 (10 primitives x light/dark)
- generated-file: src/Controls/DesignTokens.fs
- single-source: src/Controls/design-tokens.tokens.json
- regenerate: ./fake.sh build -t RefreshSurfaceBaselines
- failure-class: stale-generated-design-tokens