// Random.fsi — recurring arcade-helper family (FR-010, feature 062).
//
// Deterministic, replayable seeded RNG for a consumer's pure Elmish `update`.
// splitmix64 expands the seed into the initial state (avoiding the all-zero
// xorshift64 fixed point); xorshift64 generates the stream. Pure `state ->
// (value, nextState)` threading — NO ambient System.Random, no wall-clock, no
// shared mutable — so a consumer's `update` stays pure and replayable. Visibility
// lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module Random =

    /// Opaque pure RNG state (the xorshift64 stream word). Carry it through your
    /// `Model`; thread it with `nextRng`/`nextBelow`. The representation is
    /// `private` — construct it only via `seedRng`, advance it only via
    /// `nextRng`/`nextBelow`.
    [<Struct>]
    type RngState = private { State: uint64 }

    /// Expand a seed into an initial state. splitmix64 avoids the all-zero
    /// xorshift64 fixed point, so any `seed` (including `0UL`) yields a usable stream.
    val seedRng: seed: uint64 -> RngState

    /// One xorshift64 step: the next 64-bit value and the advanced state.
    val nextRng: state: RngState -> uint64 * RngState

    /// Uniform-ish value in `[0, n)`; requires `n > 0` (raises otherwise). Threads
    /// the state like `nextRng`.
    val nextBelow: n: int -> state: RngState -> int * RngState
