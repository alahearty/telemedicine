import { reactive } from 'vue'
import { getConsultationPolicy } from '@/api'
import { statusLabel } from '@/utils/appointments'

const state = reactive({
  policy: null,
  loading: false,
  error: '',
  serverOffsetMs: 0,
})

let loadPromise = null

export async function ensureConsultationPolicy(force = false) {
  if (state.policy && !force) return state.policy
  if (loadPromise && !force) return loadPromise
  state.loading = true
  state.error = ''
  loadPromise = getConsultationPolicy()
    .then((policy) => {
      const normalized = {
        allowOutsideWindow: Boolean(policy.allowOutsideWindow),
        joinWindowMinutesBefore: Number(policy.joinWindowMinutesBefore ?? 15),
        joinWindowMinutesAfter: Number(policy.joinWindowMinutesAfter ?? 30),
        serverTimeUtc: policy.serverTimeUtc,
      }
      state.policy = normalized
      const serverTime = normalized.serverTimeUtc ? new Date(normalized.serverTimeUtc).getTime() : NaN
      state.serverOffsetMs = Number.isFinite(serverTime) ? serverTime - Date.now() : 0
      return normalized
    })
    .catch((error) => {
      state.error = error.message || 'Could not load consultation settings.'
      return null
    })
    .finally(() => {
      state.loading = false
      loadPromise = null
    })
  return loadPromise
}

export function useConsultationPolicy() {
  return { state, ensureConsultationPolicy }
}

export function chatAccessFor(appointment) {
  const status = statusLabel(appointment?.status).toLowerCase()
  if (status === 'pending') {
    return { canChat: false, label: 'Awaiting confirmation', detail: 'Chat unlocks when the physician accepts.' }
  }
  if (status === 'active' || status === 'not attended' || status === 'attended') {
    return { canChat: true, label: 'Message', detail: status === 'attended' ? 'Follow-up chat' : 'Securely message your care team.' }
  }
  return { canChat: false, label: 'Unavailable', detail: 'Chat is not available for this appointment.' }
}

export function videoAccessFor(appointment, policy) {
  if (!appointment) {
    return { canJoin: false, state: 'blocked', label: 'Video unavailable', detail: '' }
  }
  const status = statusLabel(appointment.status).toLowerCase()
  if (status === 'pending') {
    return { canJoin: false, state: 'pending', label: 'Awaiting confirmation', detail: 'Video opens after the physician accepts.' }
  }
  if (status === 'attended' || status === 'completed') {
    return { canJoin: false, state: 'ended', label: 'Consultation completed', detail: '' }
  }
  if (status !== 'active') {
    return { canJoin: false, state: 'blocked', label: 'Video unavailable', detail: '' }
  }

  const options = policy || {
    allowOutsideWindow: false,
    joinWindowMinutesBefore: 15,
    joinWindowMinutesAfter: 30,
    serverTimeUtc: null,
  }
  const startValue = appointment.appointmentDate || appointment.scheduleStartTime
  const start = startValue ? new Date(startValue) : null
  if (!start || Number.isNaN(start.getTime())) {
    return { canJoin: options.allowOutsideWindow, state: options.allowOutsideWindow ? 'open' : 'blocked', label: options.allowOutsideWindow ? 'Join video' : 'Video unavailable', detail: '' }
  }
  const endValue = appointment.scheduleEndTime
  const end = endValue && !Number.isNaN(new Date(endValue).getTime())
    ? new Date(endValue)
    : new Date(start.getTime() + 30 * 60 * 1000)
  const opensAt = new Date(start.getTime() - Math.max(0, options.joinWindowMinutesBefore) * 60 * 1000)
  const closesAt = new Date(end.getTime() + Math.max(0, options.joinWindowMinutesAfter) * 60 * 1000)
  const now = Date.now() + state.serverOffsetMs
  const inWindow = now >= opensAt.getTime() && now <= closesAt.getTime()

  if (options.allowOutsideWindow || inWindow) {
    return {
      canJoin: true,
      state: 'open',
      label: 'Join video',
      detail: options.allowOutsideWindow && !inWindow ? 'Demo mode allows early joining.' : 'The room is open.',
      opensAt,
      closesAt,
    }
  }
  if (now < opensAt.getTime()) {
    return {
      canJoin: false,
      state: 'waiting',
      label: 'Video not open yet',
      detail: `Opens ${opensAt.toLocaleString(undefined, { weekday: 'short', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })}`,
      opensAt,
      closesAt,
    }
  }
  return {
    canJoin: false,
    state: 'closed',
    label: 'Video window closed',
    detail: 'Use secure chat to follow up with your care team.',
    opensAt,
    closesAt,
  }
}
