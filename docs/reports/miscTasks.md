# Misc Tasks

- [ ] Change F# scripts that use absolute `#r` paths to add folders to the assembly search path with `#I "path/to/folder"`, then reference assemblies from those folders with `#r`.
- [ ] Note: a gate reverted `aggregate-hang-diagnostics.md` to the stale 020 template — a known gotcha; I'll re-author it after the gates finish.
- [ ] `dotnet fsi` fails to start window because of transiive dependendies failure. that can be corrected various ways. compiled dll, separate `.fsx` loader...
