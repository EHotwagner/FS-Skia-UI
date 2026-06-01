// ContractView.fsi — the generated/derived `validation.contract.yml` view of the compiled
// routing policy (feature 042, FR-007; Principle II). Build-tooling only; pure. `render`
// is the single emitter so the retained file's existing consumers (build.fsx,
// TargetMetadataDrift, AgentReady) keep reading a coherent artifact while `Routing.fs`
// stays the sole source of truth. Drift is made structurally impossible: detection folds
// into TargetMetadataDrift, regeneration into RefreshSurfaceBaselines (research R1).
module FS.Skia.UI.Build.ContractView

open FS.Skia.UI.Build

/// The canonical `validation.contract.yml` text derived from the routing policy: schema
/// header, defaults (broad fallback, unknown_gate_rejection, final gates), the tiers, the
/// routing_rules (from `Routing.rules`), and the dogfood feature ids. Pure & deterministic
/// (stable ordering) so byte-equality is the currency contract.
val render: rules: Routing.RoutingRule list -> dogfoodFeatureIds: string list -> string

/// PURE currency check (FR-007, SC-007): None when the on-disk contract equals `render`
/// output; Some diagnostic ("validation.contract.yml is stale — regenerate from Routing.fs
/// via ./fake.sh build -t RefreshSurfaceBaselines") otherwise. The edge passes the on-disk
/// text; this function does no I/O.
val currencyDrift: onDiskContract: string -> rules: Routing.RoutingRule list -> dogfoodFeatureIds: string list -> string option
