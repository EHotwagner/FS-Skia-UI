namespace FS.Skia.UI.SkillSupport

module Wrap =

    let wrapDeltaX (worldWidth: float) (fromX: float) (toX: float) : float =
        // Reduce the raw delta into (-worldWidth, worldWidth) via the remainder, then
        // fold the half that is farther than worldWidth/2 around the seam. The result is
        // the signed distance of least magnitude in (-worldWidth/2, worldWidth/2].
        let half = worldWidth / 2.0
        let m = (toX - fromX) % worldWidth

        if m > half then m - worldWidth
        elif m <= -half then m + worldWidth
        else m
