# Misc Tasks

- [ ] Change F# scripts that use absolute `#r` paths to add folders to the assembly search path with `#I "path/to/folder"`, then reference assemblies from those folders with `#r`.
- [ ] Note: a gate reverted `aggregate-hang-diagnostics.md` to the stale 020 template — a known gotcha; I'll re-author it after the gates finish.
- [ ] `dotnet fsi` fails to start window because of transiive dependendies failure. that can be corrected various ways. compiled dll, separate `.fsx` loader...
- [ ] ⚠ /home/developer/projects/FS-Skia-UI/.agents/skills/fs-skia-reconciliation/SKILL.md: invalid YAML:
  mapping values are not allowed in this context at line 2 column 414
