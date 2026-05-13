# First Visible Frame Timing

Observed Linux Vulkan smoke logs all reported first-frame or elapsed timings below 2 seconds:

| Sample | Evidence | Timing |
|--------|----------|--------|
| BasicViewer | `readiness/smoke/t026-basicviewer-smoke.txt` | under 2s |
| ParityGallery | `readiness/smoke/t029-paritygallery-vulkan.txt` | 312ms in captured log |
| EffectsGallery | `readiness/smoke/t029-effectsgallery-vulkan.txt` | under 2s |
| ChartsGallery | `readiness/smoke/t039-chartsgallery-vulkan.txt` | under 2s |
| DataGridGallery | `readiness/smoke/t039-datagridgallery-vulkan.txt` | under 2s |
| LayoutGraphGallery | `readiness/smoke/t050-layoutgraphgallery-vulkan.txt` | 438ms in captured log |
| Screenshot path | `readiness/smoke/t055-basicviewer-screenshot.txt` | 349ms elapsed in captured log |

Result: 7 of 7 available supported-workstation smoke runs meet the 2-second target.
