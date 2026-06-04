namespace FS.Skia.UI.SkillSupport

module Random =

    // RngState is opaque in the .fsi; the representation is the xorshift64 stream
    // word. A struct record keeps it allocation-free when threaded through a
    // consumer's pure `update`.
    [<Struct>]
    type RngState = { State: uint64 }

    let seedRng (seed: uint64) : RngState =
        // splitmix64: expand the seed so any value (including 0UL) yields a usable,
        // well-mixed initial word. `mutable z` is a LOCAL working value inside this
        // pure function (no ambient/shared state) — the only mutation in the module.
        let mutable z = seed + 0x9E3779B97F4A7C15UL
        z <- (z ^^^ (z >>> 30)) * 0xBF58476D1CE4E5B9UL
        z <- (z ^^^ (z >>> 27)) * 0x94D049BB133111EBUL
        z <- z ^^^ (z >>> 31)
        // Guard the all-zero xorshift64 fixed point.
        { State = if z = 0UL then 0x9E3779B97F4A7C15UL else z }

    let nextRng (state: RngState) : uint64 * RngState =
        // xorshift64 step. `mutable x` is a LOCAL working value; the function is
        // pure (same input ⇒ same output), so a consumer's `update` stays replayable.
        let mutable x = state.State
        x <- x ^^^ (x <<< 13)
        x <- x ^^^ (x >>> 7)
        x <- x ^^^ (x <<< 17)
        x, { State = x }

    let nextBelow (n: int) (state: RngState) : int * RngState =
        if n <= 0 then
            invalidArg (nameof n) "n must be > 0"
        let value, next = nextRng state
        int (value % uint64 n), next
