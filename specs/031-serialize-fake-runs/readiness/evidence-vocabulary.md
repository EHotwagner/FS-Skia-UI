# Evidence Vocabulary

Status: pending implementation validation.

- FAKE-backed: commands invoking `fake.sh`, `fake.cmd`, or `dotnet fake`.
- Serialized: run one FAKE-backed command at a time with recorded order.
- Non-authoritative aggregate: broad logs such as `Verify` that do not replace
  focused command-order evidence.
- Race-like FAKE failure: a failure with suspected or unknown concurrent
  FAKE-backed context; rerun affected commands sequentially before product
  debugging.
