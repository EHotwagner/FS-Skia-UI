# Honest `Invalidated` (SC-008 / FR-001a)

evidence-kind=semantic-test
status=pass
authoritative=true
command=dotnet test tests/Layout.Tests/Layout.Tests.fsproj --filter Feature097 ; dotnet test tests/Layout.Tests/Layout.Tests.fsproj
artifact=tests/Layout.Tests/Feature097IncrementalTests.fs ; tests/Layout.Tests/Tests.fs (FR-001a contract test)
failure-class=product-defect

## Claim

After an incremental call, `LayoutResult.Invalidated` reports the ACTUAL re-measured set (the requested
dirty set AFTER flex-line / fixed-size-ancestor propagation), NOT the verbatim requested input — replacing
the old stub's verbatim echo. `Revision = previous.Revision + 1`. Only `Bounds` are constrained to
byte-identity; `Invalidated`/`Revision` are incremental metadata.

## Evidence rows

case=localized-edit-under-fixed-ancestor  invalidated=⊋ {requested}, bounded by the fixed-size-ancestor subtree (e.g. {"0.0.0","0.0","0.0.1"} for a requested {"0.0.0"})
case=empty-patch                          invalidated=[] (empty)
case=content-sized-chain-to-root          invalidated=whole tree (honest full re-measure)
revision=previous.Revision + 1 (verified)

old-behaviour=the stub echoed `changedNodeIds` verbatim; the existing Layout.Tests test asserting that was
updated to assert the FR-001a honest set (and INV-1 Bounds byte-identity vs full evaluate).

result=Invalidated reports the genuine post-propagation re-measured set.
