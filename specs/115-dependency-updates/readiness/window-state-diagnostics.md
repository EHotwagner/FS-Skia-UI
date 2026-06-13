# window-state-diagnostics — applicability (feature 115, T003)

Every diagnostic-class is not-applicable for feature 115 (no new window is created; the existing
interactive host launch contract is unchanged — the change is dependency pins + a governance asset, with
no `src/**` source edit).

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit)
diagnostic-class=product-defect status=none (no window-bound product surface exercised by this feature)

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable (deterministic dependency-pin + governance-asset gate runs; no new Vulkan surface)
input-devices=not-applicable (no live device loop; no interactive scenario is constructed)
