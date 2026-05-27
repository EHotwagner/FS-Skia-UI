status=ok
mode=persistent-evidence
command=--launch-evidence
self-closed-for-evidence=true
first-frame-presented=True
input-dispatch=not-required
window-opened=true
renderer-mode=skia
user-close-observed=false
exit-path=true

# Repeated Supported-Host Attempts

attempt-count=20
pass-count=20
pass-ratio=100%
sc-001-threshold=95%
failed-attempts=0
failed-blocked-stages=none
attempt-log=specs/021-persistent-launch-evidence/readiness/logs/t040-repeated-launches.txt
attempt-artifacts=specs/021-persistent-launch-evidence/readiness/repeated-launches/
benign-warning-note=Some attempts printed GTK module warnings; each still reported status=ok with first-frame and controlled-close facts.
