# Dependency Governance Evidence

task=T045
command=./fake.sh build -t DependencyReport
result=pass
log=specs/018-persistent-gui-runtime/readiness/logs/t045-dependency-report.txt
package-impact=generated package resolution now fails exact-version drift and `NU1603`
new-runtime-dependencies=none
docs-impact=readiness records updated for package, generated verify, visual evidence, and workflow guidance

`DependencyReport` passed. No new runtime package was added for this feature.
The package impact is enforcement: generated product validation now records and
blocks unresolved `FS.Skia.UI.*` package mismatch or `NU1603` fallback.
