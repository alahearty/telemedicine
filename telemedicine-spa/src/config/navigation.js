export const NAVIGATION = {
  patient: [
    {
      label: 'Overview',
      items: [{ label: 'Dashboard', to: '/dashboard', icon: 'dashboard', exact: true }],
    },
    {
      label: 'Care',
      items: [
        { label: 'Book Appointment', to: '/dashboard/book-service', icon: 'calendar-plus' },
        { label: 'My Appointments', to: '/dashboard/appointments', icon: 'calendar-check' },
        { label: 'Messages', to: '/dashboard/chat', icon: 'message-circle', exact: true },
        { label: 'Find a Doctor', to: '/dashboard/doctors', icon: 'stethoscope' },
      ],
    },
    {
      label: 'Account',
      items: [
        { label: 'Update Record', to: '/dashboard/update-record', icon: 'file-text' },
        { label: 'Payment', to: '/dashboard/payment', icon: 'wallet' },
        { label: 'Account', to: '/dashboard/account', icon: 'user', exact: true },
      ],
    },
  ],
  admin: [
    {
      label: 'Overview',
      items: [
        { label: 'Dashboard', to: '/dashboard', icon: 'dashboard', exact: true },
        { label: 'Hospitals', to: '/dashboard/hospital', icon: 'building', exact: true },
      ],
    },
    {
      label: 'Account',
      items: [{ label: 'Account', to: '/dashboard/account', icon: 'user', exact: true }],
    },
  ],
  physician: [
    {
      label: 'Overview',
      items: [{ label: 'Dashboard', to: '/physician', icon: 'dashboard', exact: true }],
    },
    {
      label: 'Practice',
      items: [
        { label: 'Appointments', to: '/physician/appointments', icon: 'calendar-check' },
        { label: 'Messages', to: '/physician/chat', icon: 'message-circle', exact: true },
        { label: 'Availability', to: '/physician/availability', icon: 'clock', exact: true },
        { label: 'Patients', to: '/physician/patients', icon: 'users', exact: true },
        { label: 'Unattended', to: '/physician/unattended', icon: 'bell', exact: true },
      ],
    },
    {
      label: 'Account',
      items: [{ label: 'Profile', to: '/physician/profile', icon: 'user', exact: true }],
    },
  ],
}

export function roleKey(path, role) {
  const normalized = (role || '').toLowerCase()
  if ((path || '').startsWith('/physician') || normalized.includes('physician') || normalized.includes('doctor')) {
    return 'physician'
  }
  if (normalized.includes('admin') || normalized.includes('administrator')) return 'admin'
  return 'patient'
}

export function groupsFor(path, role) {
  return NAVIGATION[roleKey(path, role)]
}

export function isItemActive(item, path) {
  if (!item) return false
  if (item.exact) return path === item.to
  return path === item.to || path.startsWith(`${item.to}/`)
}

export function findPageLabel(path, groups) {
  const items = groups.flatMap((group) => group.items)
  const exact = items.find((item) => item.to === path)
  if (exact) return exact.label
  const parent = items.find((item) => !item.exact && path.startsWith(`${item.to}/`))
  return parent?.label || 'Dashboard'
}
