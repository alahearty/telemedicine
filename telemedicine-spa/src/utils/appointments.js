const LABELS = {
  0: 'Active',
  1: 'Not attended',
  2: 'Attended',
  3: 'Cancelled',
  4: 'Pending',
  5: 'Declined',
}

export function statusLabel(status) {
  if (typeof status === 'string') {
    if (/^\d+$/.test(status)) return LABELS[Number(status)] || 'Unknown'
    return status
  }
  return LABELS[status] || 'Unknown'
}

export function statusClass(status) {
  const value = statusLabel(status).toLowerCase()
  if (value === 'active' || value === 'attended' || value === 'completed') {
    return 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/40 dark:text-emerald-300'
  }
  if (value === 'pending' || value === 'not attended') {
    return 'bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-300'
  }
  if (value === 'cancelled' || value === 'declined') {
    return 'bg-red-100 text-red-700 dark:bg-red-900/40 dark:text-red-300'
  }
  return 'bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300'
}

export function isUpcoming(status) {
  const value = statusLabel(status).toLowerCase()
  return value === 'pending' || value === 'active' || value === 'not attended'
}

export function isTerminal(status) {
  const value = statusLabel(status).toLowerCase()
  return value === 'attended' || value === 'cancelled' || value === 'declined'
}

export function formatDateTime(value) {
  if (!value) return 'Not scheduled'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString(undefined, {
    weekday: 'short', year: 'numeric', month: 'short', day: 'numeric',
    hour: '2-digit', minute: '2-digit',
  })
}
