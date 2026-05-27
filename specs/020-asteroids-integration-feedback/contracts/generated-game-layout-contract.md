# Contract: Generated Game Layout

Generated graphical game samples that claim HUD readability must expose a
stable layout contract.

## Required Facts

- Scene output size used for validation.
- A named HUD/status region with positive bounds.
- A named gameplay region with positive bounds.
- Text bounds for score, lives, wave, status, or equivalent HUD values.
- Active gameplay entity bounds relevant to overlap checks.
- Movement policy proving entities wrap, clamp, spawn, and collide inside the
  gameplay region.

## Required Checks

- Default supported size readability.
- At least one documented constrained or small-window readability scenario.
- HUD text contained in the HUD region.
- HUD text does not overlap other HUD text.
- HUD text does not overlap active gameplay content.
- Gameplay entities remain inside the gameplay region unless intentionally
  layered and disclosed.

## Failure Conditions

Generated validation must fail when:

- A readability claim omits HUD region, gameplay region, or relevant text
  bounds.
- Required bounds are unsupported without explicit unsupported diagnostics.
- HUD text overlaps HUD text or gameplay content.
- Gameplay movement still uses full-scene coordinates after a HUD region is
  reserved.
