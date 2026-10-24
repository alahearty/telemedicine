import { computed, ref } from 'vue'

const STORAGE_KEY = 'color-theme'
const MODES = ['light', 'dark', 'system']

const mode = ref('light')
const systemDark = ref(false)
let mediaQuery = null

function resolveIsDark(value) {
  if (value === 'dark') return true
  if (value === 'light') return false
  return systemDark.value
}

const isDark = computed(() => resolveIsDark(mode.value))

function paint() {
  if (typeof document === 'undefined') return
  const dark = isDark.value
  document.documentElement.classList.toggle('dark', dark)
  document.documentElement.style.colorScheme = dark ? 'dark' : 'light'
}

export function setTheme(next) {
  if (!MODES.includes(next)) return
  mode.value = next
  try {
    localStorage.setItem(STORAGE_KEY, next)
  } catch {
    // Storage can be unavailable in private browsing; the in-memory value still works.
  }
  paint()
}

export function initTheme() {
  if (typeof window === 'undefined') return
  mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
  systemDark.value = mediaQuery.matches
  mediaQuery.addEventListener('change', (event) => {
    systemDark.value = event.matches
    if (mode.value === 'system') paint()
  })

  let stored = null
  try {
    stored = localStorage.getItem(STORAGE_KEY)
  } catch {
    stored = null
  }
  mode.value = MODES.includes(stored) ? stored : 'light'
  paint()
}

export function useTheme() {
  return { mode, isDark, setTheme, modes: MODES }
}
