<template>
  <svg
    class="inline-block shrink-0"
    width="1em"
    height="1em"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-width="1.8"
    stroke-linecap="round"
    stroke-linejoin="round"
    aria-hidden="true"
    focusable="false"
    v-html="markup"
  />
</template>

<script>
const ICONS = {
  dashboard:
    '<rect x="3" y="3" width="7" height="9" rx="1.7"/><rect x="14" y="3" width="7" height="5" rx="1.7"/><rect x="14" y="12" width="7" height="9" rx="1.7"/><rect x="3" y="16" width="7" height="5" rx="1.7"/>',
  calendar:
    '<rect x="3" y="4" width="18" height="17" rx="2.2"/><path d="M8 2v4M16 2v4M3 10h18"/>',
  'calendar-check':
    '<rect x="3" y="4" width="18" height="17" rx="2.2"/><path d="M8 2v4M16 2v4M3 10h18"/><path d="m8.5 15 2.2 2.2 4.3-4.4"/>',
  'calendar-plus':
    '<rect x="3" y="4" width="18" height="17" rx="2.2"/><path d="M8 2v4M16 2v4M3 10h18M12 13v6M9 16h6"/>',
  'calendar-x':
    '<rect x="3" y="4" width="18" height="17" rx="2.2"/><path d="M8 2v4M16 2v4M3 10h18M9.5 14l5 5M14.5 14l-5 5"/>',
  'calendar-clock':
    '<rect x="3" y="4" width="18" height="17" rx="2.2"/><path d="M8 2v4M16 2v4M3 10h18"/><circle cx="16" cy="16" r="4.4"/><path d="M16 13.8V16l1.6 1"/>',
  clock: '<circle cx="12" cy="12" r="9"/><path d="M12 7v5l3.5 2"/>',
  user: '<circle cx="12" cy="8" r="4"/><path d="M4.5 21c.8-3.6 3.7-6 7.5-6s6.7 2.4 7.5 6"/>',
  'user-circle': '<circle cx="12" cy="12" r="9"/><circle cx="12" cy="9.5" r="3"/><path d="M6.5 19c.7-2.9 2.8-4.5 5.5-4.5s4.8 1.6 5.5 4.5"/>',
  contact:
    '<path d="M5 3.5h14a1.5 1.5 0 0 1 1.5 1.5v14a1.5 1.5 0 0 1-1.5 1.5H5A1.5 1.5 0 0 1 3.5 19V5A1.5 1.5 0 0 1 5 3.5Z"/><circle cx="12" cy="9.5" r="2.8"/><path d="M7.5 18c.5-2.4 2.2-3.8 4.5-3.8s4 1.4 4.5 3.8"/>',
  users:
    '<circle cx="9" cy="8" r="3.4"/><path d="M2.8 20c.6-3.2 3-5.2 6.2-5.2s5.6 2 6.2 5.2"/><path d="M16 4.8a3.4 3.4 0 0 1 0 6.4M18.2 15.1c2 .6 3.3 2.3 3.7 4.9"/>',
  'user-check':
    '<circle cx="10" cy="8" r="4"/><path d="M3 21c.8-3.6 3.5-6 7-6 1.2 0 2.3.2 3.2.7"/><path d="m15.5 18 2 2 4-4.2"/>',
  'heart-pulse':
    '<path d="M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.6l-1-1a5.5 5.5 0 0 0-7.8 7.8l1 1L12 21l7.8-7.6 1-1a5.5 5.5 0 0 0 0-7.8Z"/><path d="M3.5 12h4l2-3 3 6 2-3h6"/>',
  heart:
    '<path d="M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.6l-1-1a5.5 5.5 0 0 0-7.8 7.8l1 1L12 21l7.8-7.6 1-1a5.5 5.5 0 0 0 0-7.8Z"/>',
  stethoscope:
    '<path d="M6 3v6a4 4 0 0 0 8 0V3"/><path d="M4 3h4M12 3h4"/><path d="M10 13v2.5a4.5 4.5 0 0 0 9 0V13"/><circle cx="19" cy="10.5" r="2.5"/>',
  baby:
    '<circle cx="12" cy="9" r="5"/><path d="M9.5 8.5h.01M14.5 8.5h.01M10 11.5c1.2 1 2.8 1 4 0"/><path d="M10 3.5a2 2 0 0 1 4 0"/><path d="M7 14.5 5.5 21M17 14.5 18.5 21"/>',
  bandage:
    '<rect x="3.5" y="8.5" width="17" height="7" rx="3.5" transform="rotate(-45 12 12)"/><path d="M9.5 9.5h.01M14.5 14.5h.01M12 12h.01"/>',
  bell: '<path d="M18 8a6 6 0 1 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9"/><path d="M10.3 21a2 2 0 0 0 3.4 0"/>',
  building:
    '<path d="M4 21V5.5A1.5 1.5 0 0 1 5.5 4h8A1.5 1.5 0 0 1 15 5.5V21"/><path d="M15 9h3.5A1.5 1.5 0 0 1 20 10.5V21M2 21h20"/><path d="M7 8h.01M10.5 8h.01M7 12h.01M10.5 12h.01M7 16h.01M10.5 16h.01"/>',
  search: '<circle cx="11" cy="11" r="7"/><path d="m20 20-3.2-3.2"/>',
  'chevron-down': '<path d="m6 9 6 6 6-6"/>',
  'chevron-right': '<path d="m9 6 6 6-6 6"/>',
  'arrow-up': '<path d="M12 19V5M5.5 11.5 12 5l6.5 6.5"/>',
  'arrow-right': '<path d="M5 12h14M13 6l6 6-6 6"/>',
  'arrow-left': '<path d="M19 12H5M11 18l-6-6 6-6"/>',
  menu: '<path d="M4 7h16M4 12h16M4 17h16"/>',
  x: '<path d="M6 6l12 12M18 6 6 18"/>',
  sun: '<circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/>',
  moon: '<path d="M21 12.8A8.5 8.5 0 1 1 11.2 3a6.5 6.5 0 0 0 9.8 9.8Z"/>',
  'log-out': '<path d="M10 17l5-5-5-5M15 12H3"/><path d="M15 4h4a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-4"/>',
  wallet:
    '<path d="M4 6.5A2.5 2.5 0 0 1 6.5 4H19a1 1 0 0 1 1 1v3"/><rect x="3" y="6.5" width="18" height="13" rx="2.5"/><path d="M16 12h3v3h-3a1.5 1.5 0 0 1 0-3Z"/>',
  receipt:
    '<path d="M5 3.5v17l2-1.5 2 1.5 2-1.5 2 1.5 2-1.5 2 1.5v-17l-2 1.5-2-1.5-2 1.5-2-1.5-2 1.5-2-1.5Z"/><path d="M8 8h6M8 12h6"/>',
  'file-text': '<path d="M6 2.8h7l5 5V21H6Z"/><path d="M13 2.8v5h5M8.5 12h7M8.5 16h7"/>',
  settings:
    '<circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.7 1.7 0 0 0 .3 1.9l.1.1-2.8 2.8-.1-.1a1.7 1.7 0 0 0-1.9-.3 1.7 1.7 0 0 0-1 1.6V21h-4v-.1a1.7 1.7 0 0 0-1-1.6 1.7 1.7 0 0 0-1.9.3l-.1.1L4.2 17l.1-.1a1.7 1.7 0 0 0 .3-1.9 1.7 1.7 0 0 0-1.6-1H3v-4h.1a1.7 1.7 0 0 0 1.6-1 1.7 1.7 0 0 0-.3-1.9L4.2 7 7 4.2l.1.1a1.7 1.7 0 0 0 1.9.3 1.7 1.7 0 0 0 1-1.6V3h4v.1a1.7 1.7 0 0 0 1 1.6 1.7 1.7 0 0 0 1.9-.3l.1-.1L19.8 7l-.1.1a1.7 1.7 0 0 0-.3 1.9 1.7 1.7 0 0 0 1.6 1h.1v4h-.1a1.7 1.7 0 0 0-1.6 1Z"/>',
  shield: '<path d="M12 2.5 20 6v5.5c0 5-3.4 8.4-8 10-4.6-1.6-8-5-8-10V6Z"/><path d="m8.7 12 2.2 2.2 4.4-4.5"/>',
  sparkles: '<path d="m12 3 1.6 4.4L18 9l-4.4 1.6L12 15l-1.6-4.4L6 9l4.4-1.6Z"/><path d="m19 14 .8 2.2L22 17l-2.2.8L19 20l-.8-2.2L16 17l2.2-.8Z"/><path d="m5 14 .6 1.7L7.3 16l-1.7.6L5 18.3l-.6-1.7L2.7 16l1.7-.6Z"/>',
  video: '<rect x="3" y="6" width="12.5" height="12" rx="2.5"/><path d="m15.5 10 5-2.5v9l-5-2.5"/>',
  'message-circle': '<path d="M21 11.5a8.5 8.5 0 0 1-8.5 8.5H7l-4 2 1.4-4.2A8.5 8.5 0 1 1 21 11.5Z"/><path d="M8 10h.01M12 10h.01M16 10h.01"/>',
  send: '<path d="m22 2-7 20-4-9-9-4Z"/><path d="M22 2 11 13"/>',
  check: '<path d="m5 12.5 4.2 4.2L19 7"/>',
  'alert-circle': '<circle cx="12" cy="12" r="9"/><path d="M12 7.5V13M12 16.5h.01"/>',
  info: '<circle cx="12" cy="12" r="9"/><path d="M12 11v5M12 7.5h.01"/>',
  plus: '<path d="M12 5v14M5 12h14"/>',
  edit: '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z"/>',
  trash: '<path d="M4 7h16M10 11v6M14 11v6"/><path d="M6 7l1 13h10l1-13M9 7V4h6v3"/>',
  activity: '<path d="M3 12h4l2.5-7 5 14 2.5-7h4"/>',
  'trending-up': '<path d="m3 17 6-6 4 4 8-8"/><path d="M15 7h6v6"/>',
  'bar-chart': '<path d="M4 20V10M10 20V4M16 20v-7M22 20H2"/>',
  home: '<path d="m3 10 9-7 9 7v10a1.5 1.5 0 0 1-1.5 1.5h-15A1.5 1.5 0 0 1 3 20Z"/><path d="M9 21v-6h6v6"/>',
  'help-circle': '<circle cx="12" cy="12" r="9"/><path d="M9.6 9a2.5 2.5 0 1 1 3.6 2.3c-.8.5-1.2 1-1.2 2.2M12 17h.01"/>',
  palette:
    '<path d="M12 3a9 9 0 0 0 0 18h1.5a2 2 0 0 0 1.6-3.2 2 2 0 0 1 1.6-3.2H19a3 3 0 0 0 3-3A9 9 0 0 0 12 3Z"/><path d="M7.5 10h.01M9.5 6.8h.01M14.2 6.8h.01M17 10h.01"/>',
  lock: '<rect x="4" y="10" width="16" height="11" rx="2.5"/><path d="M8 10V7a4 4 0 0 1 8 0v3"/>',
  monitor: '<rect x="2.5" y="3.5" width="19" height="13" rx="2"/><path d="M8 20.5h8M12 16.5v4"/>',
  mail: '<rect x="3" y="5" width="18" height="14" rx="2.5"/><path d="m4 7 8 6 8-6"/>',
  phone:
    '<path d="M6.5 3h2l1.4 4.2-2 1.4a12.5 12.5 0 0 0 6.5 6.5l1.4-2 4.2 1.4v2a2.5 2.5 0 0 1-2.7 2.5C10 18.4 5.6 14 4 7.7A2.5 2.5 0 0 1 6.5 3Z"/>',
  'map-pin': '<path d="M12 21s7-6 7-11a7 7 0 1 0-14 0c0 5 7 11 7 11Z"/><circle cx="12" cy="10" r="2.5"/>',
  'baby-carriage': '<path d="M5 9h14l-2 7H7Z"/><path d="M5 9V6a3 3 0 0 1 3-3M19 9a7 7 0 0 0-7-7v7"/><circle cx="9" cy="20" r="1.5"/><circle cx="17" cy="20" r="1.5"/>',
  dollar: '<path d="M12 2v20"/><path d="M17 5.5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>',
}

const ALIASES = {
  'user-account': 'user',
  'user-detail': 'contact',
  'user-round-check': 'user-check',
  'caret-down-fill': 'chevron-down',
  'chevron-down-fill': 'chevron-down',
  'notification-line': 'bell',
  'baby-carriage': 'baby-carriage',
  time: 'clock',
  'credit-card': 'wallet',
  'building-2': 'building',
  'arrow-up-right': 'trending-up',
  'help': 'help-circle',
  'settings-2': 'settings',
}

export default {
  name: 'LocalIcon',
  props: {
    icon: { type: String, required: true },
  },
  computed: {
    markup() {
      const raw = (this.icon || '').split(':').pop()
      const key = ALIASES[raw] || raw
      return ICONS[key] || ICONS.sparkles
    },
  },
}
</script>
