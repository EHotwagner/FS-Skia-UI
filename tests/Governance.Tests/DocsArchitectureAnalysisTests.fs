module DocsArchitectureAnalysisTests

// Feature 076 (US2, FR-006 / SC-002): the distinguishing deliverable of the docs
// site is that every major architecture page — and each deep-dive section closer —
// ends with a *both-sided* analysis: implementation strengths AND weaknesses, and
// design pros AND cons. A one-sided "analysis" fails the contract. This is the
// machine-checkable form of that requirement (mirrors the SkillQuality literal
// heading/keyword detector style). The required headings are exact so they cannot
// collide with prose (e.g. "Cons" vs "Considerations").

open System.IO
open Expecto
open GovernanceTestSupport

/// The closing-analysis contract: every gated page must contain an Analysis
/// section naming all four facets with these exact headings.
let requiredAnalysisHeadings =
    [ "## Analysis"
      "### Implementation strengths"
      "### Implementation weaknesses"
      "### Design pros"
      "### Design cons" ]

/// Deep-dive section closers that must also carry the closing analysis (the
/// governance and typed-control/Penpot sections — US3 / US4).
let deepDiveClosers =
    [ "docs/governance/speckit-placement.md"
      "docs/controls-design/typed-front-door.md"
      "docs/controls-design/design-tokens-penpot.md" ]

let private architecturePages () =
    let dir = fullPath "docs/architecture"

    if Directory.Exists dir then
        Directory.GetFiles(dir, "*.md")
        |> Array.map (fun p -> "docs/architecture/" + Path.GetFileName p)
        |> Array.sort
        |> Array.toList
    else
        []

[<Tests>]
let docsArchitectureAnalysisTests =
    testList
        "Docs architecture closing analysis (076 / SC-002)"
        [ test "every architecture page exists and closes with a both-sided analysis" {
              let pages = architecturePages ()

              Expect.isNonEmpty pages "docs/architecture/** must contain at least one architecture page"

              pages
              |> List.iter (fun page ->
                  Expect.isTrue (fileExists page) $"{page} exists"
                  let content = read page

                  requiredAnalysisHeadings
                  |> List.iter (fun heading ->
                      Expect.stringContains content heading $"{page} closing analysis names '{heading}'"))
          }

          test "each major part has an architecture page" {
              // SC-002: no major published part left undocumented (research R6 inventory).
              let pages = architecturePages () |> List.map Path.GetFileName |> Set.ofList

              [ "host-skiaviewer.md"
                "scene.md"
                "layout.md"
                "input.md"
                "elmish-mvu.md"
                "controls.md"
                "testing-skillsupport.md"
                "governance.md" ]
              |> List.iter (fun part -> Expect.isTrue (pages.Contains part) $"architecture page for {part} exists")
          }

          test "deep-dive section closers carry the both-sided analysis" {
              deepDiveClosers
              |> List.iter (fun page ->
                  Expect.isTrue (fileExists page) $"{page} exists"
                  let content = read page

                  requiredAnalysisHeadings
                  |> List.iter (fun heading ->
                      Expect.stringContains content heading $"{page} closing analysis names '{heading}'"))
          } ]
