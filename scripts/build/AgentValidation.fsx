module BuildAgentValidation

let validationContractSyntheticFixtures =
    [ "malformed-validation-contract.yml"
      "unknown-target.fixture.yml"
      "duplicate-rule.fixture.yml"
      "invalid-path-pattern.fixture.yml"
      "duplicate-rule-id"
      "unknown-gate"
      "invalid-path-pattern"
      "ValidationContractRejected"
      "governance-owned diagnostics"
      "governance-owned diagnostic"
      "no success verdict"
      "malformed-agent-verdict.json"
      "missing status"
      "missing authority"
      "missing requiredGates"
      "unknown failureClass"
      "AgentVerdictRejected" ]

let validationContractParserNotes =
    [ "malformed-field"
      "unknown target"
      "duplicate rule"
      "invalid path pattern" ]
