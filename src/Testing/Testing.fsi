namespace FS.Skia.UI.Testing

type PackageReferenceExpectation =
    { PackageId: string
      Required: bool }

type GeneratedProductExpectation =
    { Profile: string
      RequiredFiles: string list
      ForbiddenPrefixes: string list
      PackageReferences: PackageReferenceExpectation list }

module GeneratedProductAssertions =
    val summarize: expectation: GeneratedProductExpectation -> string
