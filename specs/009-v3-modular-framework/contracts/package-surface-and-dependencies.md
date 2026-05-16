# Contract: Package Surface And Dependencies

## Purpose

V3 package boundaries must be reviewable through `.fsi` contracts, dependency
checks, and package-specific surface baselines.

## Package Contracts

Every public capability package must provide:

- packable project metadata
- curated public `.fsi` files
- semantic tests through the public package surface
- package-specific surface baseline
- dependency ownership documented by the capability catalog

## Dependency Rules

- Scene must not depend on Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet.
- SkiaViewer may depend on Scene, Silk.NET, and SkiaSharp.
- Elmish may depend on Scene, SkiaViewer, and Fable.Elmish.
- KeyboardInput may depend on Scene and YamlDotNet.
- Layout may depend on Scene and Yoga.Net.
- Charts may depend on Scene.
- Testing may depend only on the packages required by its documented helpers.

## Validation Contract

Package validation must fail on:

- missing `.fsi` for a public module
- top-level visibility modifiers used to replace `.fsi` ownership
- public surface drift without a baseline update
- dependency leaks into Scene
- unapproved package identity changes
- generated product references to framework implementation projects in consumer
  mode

Failures must name package id, project path, dependency or symbol involved, and
required remediation.
