# Contrast — `ContrastCheck` stays the sole contrast authority (feature 096, T025, SC-007, FR-008)

evidence-kind=contrast
renderer-mode=DeterministicRenderOnly
status=pass

command=./fake.sh build -t ContrastCheck
verdict=pass (Status: Ok)

No migrated control's bridged styling regresses its contrast result. The bridge adds **no second
contrast policy** and **no new token literal**: any styling flows through E3's `Style.resolve` over the
DTCG-sourced `DesignTokens` set, and `ContrastCheck` remains the single contrast authority (FR-008).
The widened geometry (`slider`/`text-box`/`radio-group`/`switch`) reproduces the prior procedural
colours at `Normal` (byte-identical) and composes runtime states only through the existing resolver, so
no colour originates outside the token set.

`DesignTokenDrift` also passes — the token set is unchanged.

failure-class=none
authoritative-gate=ContrastCheck (Status: Ok)
