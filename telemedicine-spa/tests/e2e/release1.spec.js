const { test, expect } = require('@playwright/test')

async function login(page, email, password) {
  await page.goto('/login')
  await page.getByPlaceholder('you@example.com').fill(email)
  await page.getByPlaceholder('Your password').fill(password)
  await page.getByRole('button', { name: /sign in/i }).click()
}

test.beforeEach(async ({ page, context }, testInfo) => {
  await context.setExtraHTTPHeaders({
    'X-RateLimit-Partition': `playwright-${testInfo.testId}-${Date.now()}-${Math.random()}`,
  })
  const failures = []
  page.on('console', message => {
    if (message.type() === 'error' && !/401 \(Unauthorized\)|403 \(Forbidden\)/.test(message.text())) {
      failures.push(`console: ${message.text()}`)
    }
  })
  page.on('response', response => {
    if (response.status() >= 500) failures.push(`http ${response.status()}: ${response.url()}`)
  })
  page.on('requestfailed', request => {
    failures.push(`request: ${request.failure()?.errorText || 'failed'} ${request.url()}`)
  })
  testInfo._runtimeFailures = failures
})

test.afterEach(async ({ page }, testInfo) => {
  if (testInfo.status !== testInfo.expectedStatus) {
    await page.screenshot({ path: testInfo.outputPath('failure.png'), fullPage: true })
    return
  }
  const failures = testInfo._runtimeFailures || []
  if (failures.length) {
    throw new Error(`Unexpected browser runtime failures:\n${failures.join('\n')}`)
  }
})

test('invalid credentials are rejected', async ({ page }) => {
  await login(page, 'patient@demo.local', 'invalid-password')
  await expect(page).toHaveURL(/\/login$/)
  await expect(page.getByText(/invalid email or password/i)).toBeVisible()
})

test('protected routes redirect anonymous users and remain protected after logout', async ({ page }) => {
  await page.goto('/dashboard/appointments')
  await expect(page).toHaveURL(/\/login\?redirect=/)
  await login(page, 'patient@demo.local', 'Patient@12345')
  await expect(page).toHaveURL(/\/dashboard$/)
  await page.evaluate(() => localStorage.clear())
  await page.goto('/dashboard/appointments')
  await expect(page).toHaveURL(/\/login\?redirect=/)
})

test('patient authenticates, opens directory and sees persisted appointments', async ({ page }) => {
  await login(page, 'patient@demo.local', 'Patient@12345')
  await expect(page).toHaveURL(/\/dashboard$/)
  await page.goto('/dashboard/doctors')
  await expect(page.getByRole('heading', { name: 'Find a doctor' })).toBeVisible()
  await expect(page.getByText('Dr. Demo Physician')).toBeVisible()
  await page.goto('/dashboard/appointments')
  await expect(page.getByRole('heading', { name: 'My appointments' })).toBeVisible()
  await expect(page.getByText(/synthetic|consultation/i).first()).toBeVisible()
  const count = await page.evaluate(async () => {
    const patient = await fetch('/api/Patient/me', { headers: { Authorization: `Bearer ${localStorage.getItem('access-token')}` } }).then(r => r.json())
    const id = patient.data?.id ?? patient.id
    const response = await fetch(`/api/Appointment/patient/${id}`, { headers: { Authorization: `Bearer ${localStorage.getItem('access-token')}` } })
    const body = await response.json()
    return (body.data ?? body).length
  })
  expect(count).toBeGreaterThan(0)
})

test('physician authenticates and sees only assigned appointment requests', async ({ page }) => {
  await login(page, 'physician@demo.local', 'Physician@12345')
  await expect(page).toHaveURL(/\/physician$/)
  await page.goto('/physician/appointments')
  await expect(page.getByRole('heading', { name: 'Appointments' })).toBeVisible()
  await expect(page.locator('tbody tr').first()).toBeVisible()
  const forbidden = await page.evaluate(async () => {
    const response = await fetch('/api/Appointment/physician/999999', { headers: { Authorization: `Bearer ${localStorage.getItem('access-token')}` } })
    return response.status
  })
  expect([403, 404]).toContain(forbidden)
})

test('chat and EMR endpoints reject unauthorised access without leaking errors', async ({ request }) => {
  const chat = await request.post('http://127.0.0.1:5231/chat-hub/publish-to-all?userName=attacker&message=test')
  expect(chat.status()).toBe(401)
  const hub = await request.get('http://127.0.0.1:5231/chat-hub/negotiate')
  expect([401, 405]).toContain(hub.status())
  const emr = await request.get('http://127.0.0.1:5231/api/HealthAnalysisReport/1')
  expect(emr.status()).toBe(401)
  const malformed = await request.get('http://127.0.0.1:5231/api/HealthAnalysisReport/not-an-id')
  expect(malformed.status()).toBe(401)
})
