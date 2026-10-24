const { test, expect } = require('@playwright/test')

const API = process.env.R1_API_URL || 'http://127.0.0.1:5231/api'
const DEMO_PHYSICIAN_EMAIL = 'physician@demo.local'
const DEMO_PHYSICIAN_PASSWORD = 'Physician@12345'

function unique(prefix) {
  return `${prefix}.${Date.now()}.${Math.floor(Math.random() * 100000)}`
}

async function registerPatient(request, email, lastName) {
  const response = await request.post(`${API}/auth/register`, {
    data: {
      email,
      password: 'Patient1!x',
      accountType: 'Patient',
      firstName: 'Flow',
      lastName,
      gender: 'Female',
      birth: '1992-05-06',
      phone: '+27 11 700 1000',
      address: '1 E2E Street',
      emergencyContactName: 'E2E Contact',
      emergencyContactPhone: '+27 11 700 1001',
    },
  })
  expect(response.ok()).toBeTruthy()
  return response.json()
}

async function loginApi(request, email, password) {
  const response = await request.post(`${API}/auth/login`, { data: { email, password } })
  expect(response.ok()).toBeTruthy()
  const body = await response.json()
  return body.data.token
}

async function getJson(request, path, token) {
  const response = await request.get(`${API}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
  })
  expect(response.ok()).toBeTruthy()
  return response.json()
}

async function resolveDemoBookingData(request, patientToken, slotCount = 2) {
  const physiciansBody = await getJson(request, '/Physician', patientToken)
  const physicians = physiciansBody.data ?? physiciansBody
  const physician = physicians.find(p => p.email === DEMO_PHYSICIAN_EMAIL)
  expect(physician).toBeTruthy()

  const servicesBody = await getJson(request, '/TelemedicineServices', patientToken)
  const services = servicesBody.data ?? servicesBody
  const service = services.find(s => s.serviceName === 'General Consultation') ?? services[0]
  expect(service).toBeTruthy()

  const from = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString()
  const to = new Date(Date.now() + 40 * 24 * 60 * 60 * 1000).toISOString()
  const slots = await getJson(
    request,
    `/physicians/${physician.id}/slots?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`,
    patientToken,
  )
  expect(slots.length).toBeGreaterThanOrEqual(slotCount)
  return { physician, service, slots }
}

async function bookApi(request, patientToken, service, physician, slot) {
  const paymentResponse = await request.post(`${API}/TelemedicinePayment`, {
    headers: { Authorization: `Bearer ${patientToken}` },
    data: { telemedicineServiceId: service.id, amountPaid: service.cost },
  })
  expect(paymentResponse.ok()).toBeTruthy()
  const payment = await paymentResponse.json()

  const bookingResponse = await request.post(`${API}/Appointment`, {
    headers: { Authorization: `Bearer ${patientToken}` },
    data: {
      serviceId: service.id,
      paymentToken: payment.data.paymentToken,
      appointmentDate: slot.start,
      physicianId: physician.id,
      durationMinutes: slot.durationMinutes,
    },
  })
  expect(bookingResponse.ok()).toBeTruthy()
  const booking = await bookingResponse.json()
  return booking.data.id
}

test.describe.serial('Release 1 patient and physician workflows', () => {
  const browserErrors = []

  test.beforeEach(async ({ page, context }) => {
    browserErrors.length = 0
    await context.setExtraHTTPHeaders({
      'X-RateLimit-Partition': `playwright-${Date.now()}-${Math.random()}`,
    })
    page.on('console', message => {
      if (message.type() === 'error') browserErrors.push(`console: ${message.text()}`)
    })
    page.on('pageerror', error => browserErrors.push(`pageerror: ${error.message}`))
    page.on('response', response => {
      if (response.status() >= 500) browserErrors.push(`http ${response.status()}: ${response.url()}`)
    })
    page.on('requestfailed', request => {
      browserErrors.push(`request: ${request.failure()?.errorText || 'failed'} ${request.url()}`)
    })
  })

  test.afterEach(async ({ page }) => {
    if (browserErrors.length) {
      await page.screenshot({ path: test.info().outputPath('runtime-failure.png'), fullPage: true })
      throw new Error(`Unexpected browser runtime failures:\n${browserErrors.join('\n')}`)
    }
  })

  test('patient registers, books a slot, reschedules and cancels', async ({ page, request }) => {
    const email = unique('e2e.patient') + '@demo.local'
    const lastName = 'Patient' + Date.now().toString().slice(-6)

    await page.goto('/signup')
    await page.getByTestId('signup-account-type').selectOption('Patient')
    await page.getByTestId('signup-first-name').fill('Flow')
    await page.getByTestId('signup-last-name').fill(lastName)
    await page.getByTestId('signup-email').fill(email)
    await page.getByTestId('signup-password').fill('Patient1!x')
    await page.getByTestId('signup-confirm-password').fill('Patient1!x')
    await page.getByTestId('signup-birth').fill('1992-05-06')
    await page.getByTestId('signup-phone').fill('+27 11 700 1000')
    await page.getByTestId('signup-submit').click()

    await page.waitForURL('**/login')
    await page.getByTestId('login-email').fill(email)
    await page.getByTestId('login-password').fill('Patient1!x')
    await page.getByTestId('login-submit').click()
    await page.waitForURL('**/dashboard')
    await expect(page.locator('h1').filter({ hasText: 'Welcome back' })).toBeVisible()

    await page.goto('/dashboard/doctors')
    await expect(page.getByRole('heading', { name: 'Find a doctor' })).toBeVisible()
    const demoCard = page.getByTestId('doctor-card').filter({ hasText: 'Demo Physician' })
    await expect(demoCard).toBeVisible()
    await demoCard.getByTestId('view-doctor').click()
    await page.waitForURL(/\/dashboard\/doctors\/\d+/)
    await expect(page.getByRole('heading', { name: /Dr\. Demo Physician/ })).toBeVisible()

    await page.getByTestId('slot-chip').first().click()
    await page.getByTestId('continue-from-profile').click()
    await page.waitForURL(/\/dashboard\/book-service\?physicianId=\d+/)

    const serviceCard = page.getByTestId('service-card').filter({ hasText: 'General Consultation' })
    await expect(serviceCard).toBeVisible()
    await serviceCard.click()

    const doctorCard = page.getByTestId('doctor-card').filter({ hasText: 'Demo Physician' })
    await expect(doctorCard).toBeVisible()
    if (!(await page.getByText(/Selected:/).isVisible().catch(() => false))) {
      await doctorCard.click()
    }
    if (!(await page.getByText(/Selected:/).isVisible().catch(() => false))) {
      await page.getByTestId('slot-chip').first().click()
    }
    await expect(page.getByText(/Selected:/)).toBeVisible()
    await page.getByTestId('continue-to-payment').click()

    await page.waitForURL('**/dashboard/payment')
    await expect(page.getByTestId('pay-button')).toBeVisible()
    await page.getByTestId('pay-button').click()
    await expect(page.getByTestId('payment-success')).toBeVisible({ timeout: 30000 })

    await page.getByRole('link', { name: 'View appointment' }).click()
    await page.waitForURL(/\/dashboard\/appointments\/\d+/)
    const appointmentId = Number(page.url().split('/').pop())
    await expect(page.getByTestId('appointment-status')).toHaveText(/pending/i)

    await page.getByTestId('reschedule-toggle').click()
    await expect(page.getByTestId('slot-chip').first()).toBeVisible()
    await page.getByTestId('slot-chip').first().click()
    await page.getByRole('button', { name: /Confirm reschedule/ }).click()
    await expect(page.getByText('Appointment rescheduled.')).toBeVisible()

    page.once('dialog', async dialog => {
      await dialog.accept('E2E cancellation')
    })
    await page.getByTestId('cancel-appointment').click()
    await expect(page.getByText('Appointment cancelled.')).toBeVisible()
    await expect(page.getByTestId('appointment-status')).toHaveText(/cancelled/i)

    // The API must reflect the terminal state, not just the UI.
    const token = await loginApi(request, email, 'Patient1!x')
    const detail = await getJson(request, `/Appointment/${appointmentId}`, token)
    expect(String(detail.data.status).toLowerCase()).toBe('cancelled')
  })

  test('physician saves availability and accepts and declines requests', async ({ page, request }) => {
    const lastName = 'DoctorFlow' + Date.now().toString().slice(-6)
    const email = unique('e2e.doctorflow') + '@demo.local'
    await registerPatient(request, email, lastName)
    const patientToken = await loginApi(request, email, 'Patient1!x')
    const { physician, service, slots } = await resolveDemoBookingData(request, patientToken, 2)
    const firstAppointmentId = await bookApi(request, patientToken, service, physician, slots[0])
    const secondAppointmentId = await bookApi(request, patientToken, service, physician, slots[1])

    await page.goto('/login')
    await page.getByTestId('login-email').fill(DEMO_PHYSICIAN_EMAIL)
    await page.getByTestId('login-password').fill(DEMO_PHYSICIAN_PASSWORD)
    await page.getByTestId('login-submit').click()
    await page.waitForURL('**/physician')

    await page.goto('/physician/availability')
    await expect(page.getByRole('heading', { name: 'Working schedule' })).toBeVisible()
    await expect.poll(() => page.locator('form input[type="checkbox"]:checked').count()).toBeGreaterThan(0)
    await page.getByTestId('save-availability').click()
    await expect(page.getByText('Schedule saved.')).toBeVisible()

    await page.goto('/physician/appointments')
    const pendingRows = page.locator('tbody tr').filter({ hasText: lastName }).filter({ hasText: 'Pending' })
    await expect(pendingRows).toHaveCount(2)

    await pendingRows.first().getByTestId('appointment-details-link').click()
    await page.waitForURL(/\/physician\/appointments\/\d+/)
    await expect(page.getByTestId('appointment-status')).toHaveText(/pending/i)
    await page.getByTestId('accept-appointment').click()
    await expect(page.getByText('Appointment accepted.')).toBeVisible()
    await expect(page.getByTestId('appointment-status')).toHaveText(/active/i)

    await page.goto('/physician/appointments')
    const pendingAfterAccept = page.locator('tbody tr').filter({ hasText: lastName }).filter({ hasText: 'Pending' })
    await expect(pendingAfterAccept).toHaveCount(1)
    await pendingAfterAccept.first().getByTestId('appointment-details-link').click()
    await page.waitForURL(/\/physician\/appointments\/\d+/)
    page.once('dialog', async dialog => {
      await dialog.accept('E2E physician decline')
    })
    await page.getByTestId('decline-appointment').click()
    await expect(page.getByText('Appointment declined.')).toBeVisible()
    await expect(page.getByTestId('appointment-status')).toHaveText(/declined/i)
  })
})
