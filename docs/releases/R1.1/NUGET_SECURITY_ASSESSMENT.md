# NuGet security assessment

Command: `dotnet list telemedicine-webapi/telemedicine-webapi.sln package --vulnerable --include-transitive --format json`.

Initial confirmed findings were transitive: AutoMapper 12.0.1 (GHSA-rvv3-g6hj-g44x, high); Microsoft.AspNetCore.DataProtection 10.0.0 (GHSA-9mv3-2cwr-p262, critical); System.Security.Cryptography.Pkcs 9.0.4/10.0.0 (GHSA-846f-f764-76mx high and GHSA-2fmv-rv2c-59x7 moderate); and System.Security.Cryptography.Xml 10.0.0 (multiple high advisories). DataProtection affected test-host authentication infrastructure; AutoMapper is used for DTO mapping; crypto packages arrived through framework/test dependency paths.

Remediation used the smallest supported servicing upgrades: Microsoft .NET packages 10.0.13, explicit crypto 10.0.13 pins, and AutoMapper 15.1.1. The removed AutoMapper DI extension is built into modern AutoMapper; registration and mapping tests were adjusted for its supported API.

Post-remediation vulnerability report exit code: 0. The JSON result lists all eight projects with no vulnerable packages. Solution build and all backend tests pass. No advisory suppression was added.
