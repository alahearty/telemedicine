# Dependency Audit

## Before and after

| Scope | Baseline | Release 1.3 |
|---|---:|---:|
| npm all dependencies | 36: 16 high, 20 moderate | 0 |
| npm production dependencies | 18: 6 high, 12 moderate | 0 |
| NuGet direct/transitive | 0 vulnerable packages | 0 vulnerable packages |

No critical advisories existed at the R1.2 baseline. No advisory suppression, override, or `npm audit fix --force` was used. The reduction came from removing Vue CLI/webpack and migrating the vulnerable Tailwind scanner chain to supported tooling.

## Reproduction

```powershell
cd telemedicine-spa
npm ci
npm audit --json
npm audit --omit=dev --json
cd ..
dotnet list telemedicine-webapi/telemedicine-webapi.sln package --vulnerable --include-transitive --no-restore --format json
```

`npm ci` installed 143 packages and audited 144 with zero findings. The final npm audit returned exit code 0 with zero vulnerabilities at every severity. The NuGet JSON result listed all eight projects and no vulnerable package entries.

The lockfile matches the manifest and reproduced the Vite production build. Dependency posture is point-in-time and must be reassessed when packages or advisory data change.
