const { defineConfig, devices } = require('@playwright/test')

module.exports = defineConfig({
  testDir: './tests/e2e',
  timeout: 120000,
  expect: { timeout: 20000 },
  fullyParallel: false,
  workers: 1,
  retries: 0,
  reporter: [
    ['list'],
    ['html', { outputFolder: 'playwright-report', open: 'never' }],
  ],
  use: {
    baseURL: 'http://127.0.0.1:8080',
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    ...devices['Desktop Chrome'],
    launchOptions: {
      args: ['--use-fake-device-for-media-stream', '--use-fake-ui-for-media-stream'],
    },
  },
  webServer: [
    {
      command: 'dotnet run --no-build --project ../telemedicine-webapi/src/telemedicine.api/telemedicine.api.csproj --urls http://127.0.0.1:5231',
      url: 'http://127.0.0.1:5231/health',
      timeout: 180000,
      reuseExistingServer: true,
      env: {
        ASPNETCORE_ENVIRONMENT: 'Development',
        DefaultConnection: 'Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123',
        ConnectionStrings__DefaultConnection: 'Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123',
        JwtSettings__Key: 'bkoRhqz4meFCPauYpjyIlZZDCO59rRjrH4IRloPdvg8=',
        JwtSettings__Issuer: 'Telemedicine',
        JwtSettings__Audience: 'https://localhost:44396/',
        DemoData__Enabled: 'true',
        Database__ApplyMigrationsOnStartup: 'true',
        RateLimiting__PartitionHeaderEnabled: 'true',
        RateLimiting__Authentication__PermitLimit: '1000',
        RateLimiting__Payment__PermitLimit: '1000',
        RateLimiting__Scheduling__PermitLimit: '1000',
      },
    },
    {
      command: 'npm run serve -- --host 127.0.0.1 --port 8080',
      url: 'http://127.0.0.1:8080/',
      timeout: 180000,
      reuseExistingServer: true,
      env: { VITE_API_TARGET: 'http://127.0.0.1:5231' },
    },
  ],
})
