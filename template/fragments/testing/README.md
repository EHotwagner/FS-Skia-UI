# Testing Fragment

Adds testing helper package references and product validation guidance.

Generated evidence can record semantic scene facts such as lander, terrain,
landing pad, and HUD metrics from deterministic-scene-evidence.
deterministic-scene-evidence does not prove semantic object presence in a live
screenshot unless the screenshot artifact is captured from the live viewer
window. A pixel-readback fallback must record `fallback-reason` and
`proves-screenshot=false`.
