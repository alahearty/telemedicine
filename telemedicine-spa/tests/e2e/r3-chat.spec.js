const { test, expect } = require('@playwright/test')

async function login(page, email, password) {
  await page.goto('/login', { waitUntil: 'networkidle' })
  await page.getByPlaceholder('you@example.com').fill(email)
  await page.getByPlaceholder('Your password').fill(password)
  await page.getByRole('button', { name: /sign in/i }).click()
  await page.waitForURL('**/dashboard')
}

test('scheduled appointments expose chat and video actions with persisted messaging', async ({ page }) => {
  await login(page, 'patient@demo.local', 'Patient@12345')
  await page.goto('/dashboard/appointments', { waitUntil: 'networkidle' })

  const active = page.locator('article').filter({ hasText: 'Active' }).first()
  await expect(active).toBeVisible()
  await expect(active.getByRole('button', { name: 'Join video' })).toBeVisible()
  await expect(active.getByRole('link', { name: 'Message', exact: true })).toBeVisible()

  const pending = page.locator('article').filter({ hasText: 'Pending' }).first()
  await expect(pending.getByText('Awaiting confirmation').first()).toBeVisible()
  await expect(pending.getByRole('button', { name: 'Join video' })).toHaveCount(0)

  await active.getByRole('link', { name: 'Message', exact: true }).click()
  await page.waitForURL(/\/dashboard\/chat\?appointmentId=\d+/)
  await expect(page.getByRole('heading', { name: 'Messages' })).toBeVisible()

  const text = `Playwright chat verification ${Date.now()}`
  await page.locator('textarea[aria-label="Message"]').fill(text)
  await page.getByRole('button', { name: 'Send message' }).click()
  await expect(page.getByText(text).first()).toBeVisible()

  await page.reload({ waitUntil: 'networkidle' })
  await expect(page.getByText(text).first()).toBeVisible()
})
