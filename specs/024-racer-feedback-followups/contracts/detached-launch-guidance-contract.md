# Detached Launch Guidance Contract

## Scope

Applies to generated Linux background GUI launch guidance in docs, generated
project instructions, and readiness examples.

## Required Linux Pattern

Guidance MUST recommend a detached-session pattern for Linux GUI apps that:

- starts the process in a detached session
- captures stdout and stderr to a log file
- redirects standard input away from the terminal
- exposes the log path for later diagnosis

An accepted pattern is:

```bash
setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/app-run.txt 2>&1 < /dev/null &
```

## Prohibited Default

Guidance MUST NOT present simple terminal detachment, plain shell backgrounding,
or plain `nohup dotnet run ... &` as the preferred reliable default for GUI app
startup when the detached-session pattern is available.

## Evidence

- `specs/024-racer-feedback-followups/readiness/detached-launch-guidance.md`
  records reviewed guidance files, accepted command patterns, rejected stale
  guidance, and log/stdin handling facts.
