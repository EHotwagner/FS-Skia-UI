---
name: fsdocs-technical
description: This skill should be used when the user asks to "write architecture docs", "document design decisions", "create an ADR", "write technical documentation", "explain the architecture", "document internals", "add a migration guide", or needs to create Markdown documentation in `docs/` that explains architecture, design decisions, and system internals for an FSharp.Formatting site.
version: 0.1.0
---

# Write Technical Documentation

Create Markdown files in `docs/` that explain architecture, design decisions, and system internals for an FSharp.Formatting documentation site.

**Key capabilities:**
- Write architecture overviews that explain system structure and component relationships
- Create ADR-style design decision records
- Document subsystem internals and data flows
- Write migration guides for breaking changes
- Use FSharp.Formatting cross-references and navigation conventions

## Overview

Technical documentation lives as `.md` files in the `docs/` directory alongside literate scripts. These pages explain the "why" behind design choices, the "how" of system architecture, and the "what changed" in migration guides. They complement API reference (auto-generated from code) and tutorials (literate scripts) by providing context that neither code nor examples can convey.

## Workflow

### 1. Explore the Codebase

Before writing, understand the topic thoroughly:
- Read the relevant source files and their structure
- Identify key types, modules, and their relationships
- Understand the design trade-offs that were made
- Note any non-obvious decisions that need explanation

### 2. Identify Audience and Purpose

Determine what type of document is needed:

| Type | Audience | Purpose |
|---|---|---|
| Architecture overview | New contributors, evaluators | Understand system structure |
| Design decision (ADR) | Current and future maintainers | Record why a choice was made |
| Subsystem deep-dive | Contributors working in that area | Understand internals |
| Migration guide | Library consumers | Upgrade between versions |

### 3. Create the Markdown File

Place the file in `docs/` with a descriptive name. Start with FSharp.Formatting frontmatter:

```markdown
---
title: Architecture Overview
category: Design
categoryindex: 4
index: 1
description: High-level architecture and component relationships.
---
```

### 4. Write the Content

Structure depends on document type. General principles:
- Lead with a one-paragraph summary of the entire document
- Use headings to create scannable structure
- Include code snippets to ground abstract concepts in concrete types
- Link to API docs and other pages for details

### 5. Cross-Reference API Docs

Link to auto-generated API pages using `cref` syntax:

| Reference type | Syntax |
|---|---|
| Type | `cref:T:Namespace.TypeName` |
| Method | `cref:M:Namespace.TypeName.MethodName` |
| Property | `cref:P:Namespace.TypeName.PropertyName` |
| Module | `cref:T:Namespace.ModuleName` |
| Module function | `cref:M:Namespace.ModuleName.functionName` |

Use these to connect narrative descriptions to precise API definitions.

### 6. Add Diagrams

For architecture and data flow:
- ASCII diagrams for simple relationships (inline in Markdown)
- Image files in `docs/img/` for complex diagrams
- Reference images with standard Markdown: `![Diagram description](img/diagram.png)`

Keep ASCII diagrams simple — they work well for 3-5 component relationships. Use image files for anything more complex.

### 7. Verify with Build

```
dotnet fsdocs build --clean
```

Check the rendered output for:
- Correct navigation placement (category and index ordering)
- Working cross-references
- Proper formatting of code blocks and tables

## Document Types

### Architecture Overview

Explain the system at a high level. Use the `templates/architecture.md` template as a starting point.

Key sections:
- **Overview** — one paragraph summarizing the system
- **Components** — what the major parts are and what each does
- **Data flow** — how information moves through the system
- **Key decisions** — the most important design choices and their rationale
- **Dependencies** — external dependencies and why they were chosen

### Design Decision Record (ADR)

Record a specific design choice. Use the `templates/decision.md` template.

Key sections:
- **Context** — the situation and forces at play
- **Decision** — what was decided
- **Consequences** — what follows from the decision (both positive and negative)
- **Alternatives considered** — what else was evaluated and why it was rejected

Number ADRs sequentially: `adr-001-use-result-type.md`, `adr-002-parser-architecture.md`. This creates a decision log that grows with the project.

### Subsystem Deep-Dive

Explain one part of the system in detail. Structure around:
- What the subsystem does and its boundaries
- Key types and their relationships
- Important algorithms or processes
- Error handling and edge cases
- Extension points and how to modify behavior

### Migration Guide

Help users upgrade between versions. Structure around:
- What changed and why
- Step-by-step migration instructions with before/after code
- Breaking changes with explicit fix for each
- New features available after migration

## Writing Principles

### Lead with the summary
Every document should start with a paragraph that gives the reader enough context to decide whether to keep reading. Write this paragraph last, after the rest of the document is complete — it is easier to summarize what exists.

### Use concrete examples
Abstract descriptions of architecture are hard to follow. Ground every concept in a specific type, function, or data flow from the actual codebase. Use `cref` links to connect narrative to code.

### Keep documents focused
Each document should have one clear purpose. An architecture overview should not also be a tutorial. A design decision record should not also be a migration guide. Create separate documents and cross-link them.

### Update or supersede, do not delete
Technical documentation is a living record. When a design decision is revisited, create a new ADR that references the old one and mark the old one as superseded. When architecture changes, update the overview document — do not create a second one.

## Frontmatter and Navigation

Categories control the sidebar organization:

```markdown
---
category: Design
categoryindex: 4
index: 1
---
```

Recommended category structure:

| Category | `categoryindex` | Content |
|---|---|---|
| Overview | 1 | Landing page, getting started |
| Tutorials | 2 | Literate script examples |
| How-To Guides | 3 | Task-oriented guides |
| Design | 4 | Architecture, ADRs |
| Reference | 5 | Detailed reference material |

Use `index` gaps (1, 3, 5...) within categories to allow insertion without renumbering.

## Best Practices

**DO:**
- ✅ Explain the "why" behind decisions, not just the "what"
- ✅ Lead every document with a one-paragraph summary
- ✅ Use cross-references (`cref:T:...`) to link narrative to API docs
- ✅ Include code snippets showing key types and patterns
- ✅ Keep architecture docs updated when the system changes
- ✅ Number ADRs sequentially and never delete old ones (mark as superseded)

**DON'T:**
- ❌ Repeat what API docs already say — link to them instead
- ❌ Write documentation that will be immediately stale (avoid version-specific details in architecture docs)
- ❌ Create deeply nested directory structures in `docs/` — keep it flat
- ❌ Skip the context section in ADRs — future readers need to know the situation
- ❌ Use complex diagrams in ASCII — use image files for anything beyond simple relationships

## Additional Resources

### Templates
- **`templates/architecture.md`** — Architecture overview template with sections for components, data flow, and key decisions
- **`templates/decision.md`** — ADR-style template with context, decision, consequences, and alternatives

## Implementation Workflow

1. Explore the codebase to understand the topic thoroughly
2. Determine the document type and audience
3. Create `.md` file in `docs/` with proper frontmatter
4. Write content following the appropriate document type structure
5. Add cross-references to API docs and related pages
6. Verify with `dotnet fsdocs build --clean`

Focus on explaining design rationale and system structure that code alone cannot convey.

## Scope / when to use

Use this skill to **write Markdown `.md` pages in `docs/`** — architecture
overviews, ADR-style design decision records, subsystem deep-dives, and migration
guides for an FSharp.Formatting site. For executable tutorials use
[[fsdocs-examples]]; for reference generated from source use [[fsdocs-api-doc]]; to
build and verify the pages use [[fsdocs-build]]; and if the docs pipeline is not
set up, run [[fsdocs-setup]] first. This skill is for the "why" and the "how it
fits together", not API reference or step-by-step tutorials.

## Driven-library API

This skill drives the external **FSharp.Formatting** content pipeline — there is no
FS.Skia.UI backing `.fsi` surface. The driven contract is the Markdown frontmatter
surface (`title`, `category`, `categoryindex`, `index`, `description`) that controls
navigation, and the `cref:` cross-reference forms (`cref:T:`, `cref:M:`, `cref:P:`)
that link narrative prose to auto-generated API pages.

## Persistent-problem mandate

Frontmatter fields, category-ordering behavior, and `cref` resolution change across
FSharp.Formatting versions, and broken cross-references fail silently. Consult the
**official online docs first** for the content/metadata guide of the pinned version
before relying on a field or `cref` form documented here, and verify the rendered
navigation and links rather than assuming they resolved.

## Related

- [[fsdocs-setup]] — scaffold the `docs/` directory these pages live in
- [[fsdocs-build]] — build that renders and validates cross-references
- [[fsdocs-api-doc]] — API pages that `cref` links here point to
- [[fsdocs-examples]] — executable tutorials that complement design prose

## Sources

- FSharp.Formatting content/metadata guide: https://fsprojects.github.io/FSharp.Formatting/content.html
- FSharp.Formatting documentation: https://fsprojects.github.io/FSharp.Formatting/
- FSharp.Formatting repository: https://github.com/fsprojects/FSharp.Formatting
