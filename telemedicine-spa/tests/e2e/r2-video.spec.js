const { test, expect, chromium } = require('@playwright/test')

const API = 'http://127.0.0.1:5231/api'

async function login(request, email, password) {
  const response = await request.post(`${API}/auth/login`, { data: { email, password } })
  expect(response.ok()).toBeTruthy()
  const body = await response.json()
  return (body.data || body).token
}

async function json(request, path, token, method = 'get', data) {
  const response = await request[method](`${API}${path}`, { headers: { Authorization: `Bearer ${token}` }, data })
  expect(response.ok()).toBeTruthy()
  const body = await response.json()
  return body.data ?? body
}

test('authorized patient and physician establish and end a real browser peer connection', async ({ request }) => {
  const patientToken = await login(request, 'patient@demo.local', 'Patient@12345')
  const physicianToken = await login(request, 'physician@demo.local', 'Physician@12345')
  const patient = await json(request, '/Patient/me', patientToken)
  const appointments = await json(request, `/Appointment/patient/${patient.id}`, patientToken)
  const eligible = appointments.filter(item => item.status === 'Active' || item.status === 0)
  expect(eligible.length, 'Synthetic data must contain an accepted appointment').toBeGreaterThan(0)
  let session
  for (const appointment of eligible) {
    const candidate = await json(request, `/consultations/appointment/${appointment.id}`, patientToken, 'post')
    if (!['Completed', 'Cancelled', 'Failed'].includes(candidate.status)) { session = candidate; break }
  }
  expect(session, 'Synthetic data must contain an open consultation').toBeTruthy()

  const browser = await chromium.launch({
    headless: true,
    args: ['--use-fake-device-for-media-stream', '--use-fake-ui-for-media-stream'],
  })
  const patientContext = await browser.newContext({ permissions: ['camera', 'microphone'] })
  const physicianContext = await browser.newContext({ permissions: ['camera', 'microphone'] })

  try {
    await patientContext.addInitScript(({ token }) => { localStorage.setItem('access-token', token); localStorage.setItem('user-role', 'Patient') }, { token: patientToken })
    await physicianContext.addInitScript(({ token }) => { localStorage.setItem('access-token', token); localStorage.setItem('user-role', 'Physician') }, { token: physicianToken })
    const patientPage = await patientContext.newPage()
    const physicianPage = await physicianContext.newPage()

    await patientPage.goto(`/dashboard/consultations/${session.sessionId}`)
    await physicianPage.goto(`/physician/consultations/${session.sessionId}`)
    await patientPage.getByRole('button', { name: 'Allow camera and microphone' }).click()
    await physicianPage.getByRole('button', { name: 'Allow camera and microphone' }).click()
    await expect(patientPage.getByText('Connected', { exact: true })).toBeVisible({ timeout: 30000 })
    await expect(physicianPage.getByText('Connected', { exact: true })).toBeVisible({ timeout: 30000 })
    await expect(patientPage.locator('video[aria-label="Remote participant video"]')).toHaveJSProperty('readyState', 4)
    await expect(physicianPage.locator('video[aria-label="Remote participant video"]')).toHaveJSProperty('readyState', 4)

    await patientPage.getByRole('button', { name: 'Mute microphone' }).click()
    await expect(patientPage.getByRole('button', { name: 'Unmute microphone' })).toBeVisible()
    await physicianPage.getByRole('button', { name: 'Turn camera off' }).click()
    await expect(physicianPage.getByRole('button', { name: 'Turn camera on' })).toBeVisible()
    await patientPage.getByRole('button', { name: 'End consultation' }).click()
    await expect(physicianPage.getByText('Ended', { exact: true })).toBeVisible()
  } finally {
    await patientContext.close()
    await physicianContext.close()
    await browser.close()
  }
})
