# Follow-Up Proposals

| ID | Title | Blocked Area | Public Surface Gap | Proposed Next Spec Scope | Compatibility Risk | Evidence |
|----|-------|--------------|--------------------|---------------------------|--------------------|----------|
| API-REC-001 | Validation-first constructors for constrained public records | Public record construction | Several existing records can represent invalid dimensions, identifiers, or bounded values through free record construction. | Specify optional helper constructors and validation-first APIs without removing record construction. | Additive if helpers are introduced separately. | `record-invariants.md` |
