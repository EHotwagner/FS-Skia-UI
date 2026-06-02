# Graphics-environment decision table (T004)

This mirrors `specs/049-fix-escalated-flake/contracts/graphics-env-contract.md`. It
governs how the compiled build front-end and the processes it spawns select a
graphics backend. **Scope**: internal build front-end (`build/**`); no public
package / `.fsi` surface is involved.

## Display-state classification

Computed from two ambient variables: `WAYLAND_DISPLAY` and `DISPLAY` (each
"present" means set and non-empty).

| Classification | `WAYLAND_DISPLAY` | `DISPLAY` | Meaning |
|----------------|-------------------|-----------|---------|
| **DualDisplay** | present | present | the failure condition — normalization applies |
| **WaylandOnly** | present | absent  | real Wayland desktop — no-op |
| **X11Only**     | absent  | present | already on the working path — no-op |
| **Neither**     | absent  | absent  | non-Linux / headless-no-display — no-op |

## C1 — Per-condition mutation table (deterministic, self-applied)

Only when **both** a Wayland and an X11 display are advertised does the front-end
force the X11 path. No operator action (env export, manual rerun) is required.

| Classification | `WAYLAND_DISPLAY` | `GDK_BACKEND` | `SDL_VIDEODRIVER` |
|----------------|-------------------|---------------|-------------------|
| DualDisplay    | **removed**       | `x11`         | `x11`             |
| WaylandOnly    | unchanged         | unchanged     | unchanged         |
| X11Only        | unchanged         | unchanged     | unchanged         |
| Neither        | unchanged         | unchanged     | unchanged         |

`normalizeGraphicsEnv` is **pure**, **total** (defined for every map incl. empty),
**idempotent** (`normalize (normalize m) = normalize m` — after one pass the state is
X11Only → no-op), and **non-destructive beyond the three named keys**.

## C2 — Normalization propagates to every spawned child

Every process the front-end launches — `dotnet test`, `dotnet fsi`, and nested
`bash ./fake.sh build -t <target>` (generated-product `Dev`/`Test`/`Verify`) — runs
under the normalized environment, guaranteed two ways:

1. **Inheritance**: the front-end normalizes its own process environment at startup,
   and children are spawned with `UseShellExecute=false`, so they inherit it.
2. **Edge re-application**: each child's `startInfo.Environment` is normalized at the
   spawn site, independent of inheritance.

Verifiable assertion: a child launched under a DualDisplay ambient environment MUST
observe **no** `WAYLAND_DISPLAY` and MUST observe `GDK_BACKEND=x11`.

## C3 — Safety on already-working hosts

On headed Linux (single display) and non-Linux developer hosts the C1 condition is
false, so the front-end and all children behave **identically** to before this
feature, including identical visual output.

## C4 — Bounded failure, never an indefinite hang

A graphics-initializing child MUST complete within its existing timeout or be killed
at the bound. On a timeout kill, the front-end emits a diagnostic that (a) names a
probable graphics-backend initialization failure as a candidate cause and (b) points
to `runtime-limitations.md`, so an environment failure is distinguishable from a
product regression.

## C5 — Real failures are never masked

The front-end MUST NOT rewrite, suppress, or ignore a child's nonzero exit code. A
genuine test/product failure still surfaces as a failure. FR-004 is satisfied by
removing the teardown-crash *cause*, not by exit-code manipulation.

## C6 — No target-surface change

No FAKE target is added, removed, or renamed; `validation.contract.yml`,
`TargetMetadata`, and `TargetMetadataDrift` outputs are unchanged. The escalated path
remains serialized and order-sensitive.

## `.fsi` exemption rationale

The normalization logic lives in the build front-end, which (consistent with every
existing `build/Governance` module) ships **no `.fsi`** — it is an internal compiled
application, not a packed library with a curated public surface — so Principle II's
`.fsi` requirement is **N/A** here.
