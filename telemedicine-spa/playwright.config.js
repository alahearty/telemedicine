const { defineConfig, devices } = require('@playwright/test')

module.exports = defineConfig({
  testDir: './tests/e2e',
  timeout: 60_000,
  retries: 0,
  workers: 1,
  reporter: [['list'], ['html', { outputFolder: 'playwright-report', open: 'never' }]],
  use: {
    baseURL: 'http://127.0.0.1:8080',
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    ...devices['Desktop Chrome'],
  },
  webServer: [
    {
      command: 'dotnet run --no-build --project ../telemedicine-webapi/src/telemedicine.api/telemedicine.api.csproj --urls http://127.0.0.1:5231',
      url: 'http://127.0.0.1:5231/health',
      timeout: 120_000,
      reuseExistingServer: true,
      env: { ASPNETCORE_ENVIRONMENT: 'Development', DemoData__Enabled: 'true' },
    },
    {
      command: 'npm run serve -- --host 127.0.0.1 --port 8080',
      url: 'http://127.0.0.1:8080/',
      timeout: 120_000,
      reuseExistingServer: true,
      env: { VUE_APP_API_TARGET: 'http://127.0.0.1:5231' },
    },
  ],
})
