/**
 * Centralized API service - consumes all backend endpoints
 */
const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'https://localhost:7231/api';

function getAuthToken() {
  return localStorage.getItem('access-token') || '';
}

async function handleResponse(response) {
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'An error occurred' }));
    const msg = error.message || error.error || error.title || error.errors?.[0] || `HTTP ${response.status}`;
    throw new Error(msg);
  }
  const contentType = response.headers.get('content-type');
  if (contentType?.includes('application/json')) {
    return response.json();
  }
  return response.text();
}

function authHeaders() {
  const token = getAuthToken();
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

// --- Auth ---
export async function login(email, password) {
  const res = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  const data = await handleResponse(res);
  const payload = data.data || data;
  return { token: payload.token, role: payload.role };
}

export async function register(payload) {
  const res = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  return handleResponse(res);
}

// --- Hospitals ---
export async function getHospitals() {
  const res = await fetch(`${API_BASE_URL}/Hospital`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getHospital(id) {
  const res = await fetch(`${API_BASE_URL}/Hospital/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function createHospital(payload) {
  const res = await fetch(`${API_BASE_URL}/Hospital`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ name: payload.name, location: payload.address }),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function updateHospital(id, payload) {
  const res = await fetch(`${API_BASE_URL}/Hospital/${id}`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify({ id, hospitalName: payload.name, address: payload.address }),
  });
  return handleResponse(res);
}

export async function deleteHospital(id) {
  const res = await fetch(`${API_BASE_URL}/Hospital/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Patients ---
export async function getPatients() {
  const res = await fetch(`${API_BASE_URL}/Patient`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getPatient(id) {
  const res = await fetch(`${API_BASE_URL}/Patient/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getCurrentPatient() {
  const res = await fetch(`${API_BASE_URL}/Patient/me`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function createPatient(payload) {
  const res = await fetch(`${API_BASE_URL}/Patient`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function updatePatient(id, payload) {
  const res = await fetch(`${API_BASE_URL}/Patient/${id}`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify({ ...payload, id }),
  });
  return handleResponse(res);
}

export async function deletePatient(id) {
  const res = await fetch(`${API_BASE_URL}/Patient/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Physicians ---
export async function getPhysicians() {
  const res = await fetch(`${API_BASE_URL}/Physician`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getPhysician(id) {
  const res = await fetch(`${API_BASE_URL}/Physician/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getCurrentPhysician() {
  const res = await fetch(`${API_BASE_URL}/Physician/me`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function createPhysician(payload) {
  const res = await fetch(`${API_BASE_URL}/Physician`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function updatePhysician(id, payload) {
  const res = await fetch(`${API_BASE_URL}/Physician/${id}`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify({ ...payload, id }),
  });
  return handleResponse(res);
}

export async function deletePhysician(id) {
  const res = await fetch(`${API_BASE_URL}/Physician/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Telemedicine Services ---
export async function getTelemedicineServices() {
  const res = await fetch(`${API_BASE_URL}/TelemedicineServices`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return Array.isArray(data) ? data : (data.data ?? data);
}

export async function getTelemedicineService(id) {
  const res = await fetch(`${API_BASE_URL}/TelemedicineServices/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

// --- Appointments ---
export async function scheduleAppointment(serviceId, paymentToken, appointmentDate = null, physicianId = null, durationMinutes = 30) {
  const res = await fetch(`${API_BASE_URL}/Appointment`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ serviceId, paymentToken, appointmentDate, physicianId, durationMinutes }),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getPatientAppointments(patientId) {
  const res = await fetch(`${API_BASE_URL}/Appointment/patient/${patientId}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getPhysicianAppointments(physicianId) {
  const res = await fetch(`${API_BASE_URL}/Appointment/physician/${physicianId}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getUnattendedAppointments() {
  const res = await fetch(`${API_BASE_URL}/Appointment/unattended`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAppointment(id) {
  const res = await fetch(`${API_BASE_URL}/Appointment/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function cancelAppointment(id, reason = '') {
  const query = reason ? `?reason=${encodeURIComponent(reason)}` : '';
  const res = await fetch(`${API_BASE_URL}/Appointment/${id}${query}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

export async function updateAppointment(id, payload) {
  const res = await fetch(`${API_BASE_URL}/Appointment/${id}`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify({ ...payload, id }),
  });
  return handleResponse(res);
}

export async function respondToAppointment(payload) {
  const res = await fetch(`${API_BASE_URL}/Appointment/respond`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  return handleResponse(res);
}

// --- Availability ---
export async function getPhysicianAvailability(physicianId) {
  const res = await fetch(`${API_BASE_URL}/physicians/${physicianId}/availability`, { headers: authHeaders() });
  return handleResponse(res);
}

export async function replacePhysicianAvailability(rules) {
  const res = await fetch(`${API_BASE_URL}/physicians/me/availability`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify(rules),
  });
  return handleResponse(res);
}

export async function getPhysicianSlots(physicianId, fromUtc, toUtc) {
  const params = new URLSearchParams();
  if (fromUtc) params.set('from', fromUtc);
  if (toUtc) params.set('to', toUtc);
  const res = await fetch(`${API_BASE_URL}/physicians/${physicianId}/slots?${params.toString()}`, {
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Payments ---
export async function makePayment(telemedicineServiceId, amountPaid) {
  const res = await fetch(`${API_BASE_URL}/TelemedicinePayment`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ telemedicineServiceId, amountPaid }),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

// --- Todo Lists ---
export async function getTodoLists() {
  const res = await fetch(`${API_BASE_URL}/TodoLists`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getTodoList(id) {
  const res = await fetch(`${API_BASE_URL}/TodoLists/${id}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function createTodoList(payload) {
  const res = await fetch(`${API_BASE_URL}/TodoLists`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function updateTodoList(payload) {
  const res = await fetch(`${API_BASE_URL}/TodoLists`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  return handleResponse(res);
}

export async function deleteTodoList(id) {
  const res = await fetch(`${API_BASE_URL}/TodoLists/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Todo Items ---
export async function createTodoItem(payload) {
  const res = await fetch(`${API_BASE_URL}/TodoItems`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function updateTodoItem(payload) {
  const res = await fetch(`${API_BASE_URL}/TodoItems`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  return handleResponse(res);
}

export async function deleteTodoItem(id) {
  const res = await fetch(`${API_BASE_URL}/TodoItems/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  return handleResponse(res);
}

// --- Analytics (correct URLs) ---
export async function getAnalyticsUsers() {
  const res = await fetch(`${API_BASE_URL}/Analytics/users`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAnalyticsServices() {
  const res = await fetch(`${API_BASE_URL}/Analytics/services`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAnalyticsPayments() {
  const res = await fetch(`${API_BASE_URL}/Analytics/payments`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAnalyticsVisitors() {
  const res = await fetch(`${API_BASE_URL}/Analytics/visitors`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAnalyticsTransactions() {
  const res = await fetch(`${API_BASE_URL}/Analytics/transactions`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function getAnalyticsReports() {
  const res = await fetch(`${API_BASE_URL}/Analytics/reports`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

// --- Health Analysis Report ---
export async function createHealthReport(payload) {
  const res = await fetch(`${API_BASE_URL}/HealthAnalysisReport/create`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export async function addReportComment(payload) {
  const res = await fetch(`${API_BASE_URL}/HealthAnalysisReport/add-comment`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
  return handleResponse(res);
}

export async function getPatientHealthReports(patientId) {
  const res = await fetch(`${API_BASE_URL}/HealthAnalysisReport/${patientId}`, { headers: authHeaders() });
  const data = await handleResponse(res);
  return data.data ?? data;
}

export { API_BASE_URL };
