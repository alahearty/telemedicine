import { reactive } from 'vue'

const KEYS = ['access-token', 'user-id', 'user-name', 'user-email', 'user-role', 'physician-id']

function read() {
  if (typeof window === 'undefined') {
    return { name: '', email: '', role: '', userId: '' }
  }
  return {
    name: localStorage.getItem('user-name') || '',
    email: localStorage.getItem('user-email') || '',
    role: localStorage.getItem('user-role') || '',
    userId: localStorage.getItem('user-id') || '',
  }
}

export const session = reactive(read())

export function refreshSession() {
  Object.assign(session, read())
}

export function setSessionName(name) {
  const value = (name || '').trim()
  if (!value) return
  try {
    localStorage.setItem('user-name', value)
  } catch {
    // Ignore storage failures; the in-memory session still updates.
  }
  session.name = value
}

export function clearSession() {
  if (typeof window !== 'undefined') {
    KEYS.forEach((key) => localStorage.removeItem(key))
  }
  Object.assign(session, { name: '', email: '', role: '', userId: '' })
}

export function useSession() {
  return session
}
