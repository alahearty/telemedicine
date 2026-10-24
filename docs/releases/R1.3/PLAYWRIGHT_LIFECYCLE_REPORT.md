# Playwright Lifecycle Report

## Root cause and remediation

Playwright's Windows `webServer` teardown left the Vue CLI/API process tree alive after otherwise successful tests. Vite improved startup, but native child shutdown was still nondeterministic in this environment.

`scripts/run-e2e.cjs` now owns the complete lifecycle:

1. Starts the already-built Release API assembly with isolated synthetic test settings.
2. Polls the real `/health/live` response.
3. Starts Vite on a strict port and polls `/`.
4. Runs the Playwright CLI and preserves its exit code/artifacts.
5. Terminates only the exact child process trees in `finally`, on success or failure.

The Playwright configuration retains one worker, Chromium, traces/screenshots/videos on failure, list output and the HTML report.

## Verification

```powershell
cd telemedicine-spa
npm run test:e2e
netstat -ano | Select-String ':5231|:8080'
```

Final result: 7 passed in 35.4 seconds; command exit code 0. The post-run listener check returned no entries for ports 5231 or 8080. Earlier controlled startup failures (missing runner, invalid content root, and missing database configuration) also exited nonzero and did not retain managed listeners. Synthetic local identities/data only are used.
