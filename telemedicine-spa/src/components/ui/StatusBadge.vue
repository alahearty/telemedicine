<template>
  <span class="badge" :class="toneClass">
    <span class="h-1.5 w-1.5 rounded-full" :class="dotClass" />
    {{ label }}
  </span>
</template>

<script>
import { statusLabel } from '@/utils/appointments'

const TONES = {
  active: {
    badge: 'border-emerald-200 bg-emerald-50 text-emerald-700 dark:border-emerald-400/20 dark:bg-emerald-400/10 dark:text-emerald-300',
    dot: 'bg-emerald-500',
  },
  attended: {
    badge: 'border-slate-200 bg-slate-50 text-slate-600 dark:border-white/[0.08] dark:bg-white/[0.04] dark:text-slate-300',
    dot: 'bg-slate-400',
  },
  pending: {
    badge: 'border-amber-200 bg-amber-50 text-amber-700 dark:border-amber-400/20 dark:bg-amber-400/10 dark:text-amber-300',
    dot: 'bg-amber-500',
  },
  'not attended': {
    badge: 'border-amber-200 bg-amber-50 text-amber-700 dark:border-amber-400/20 dark:bg-amber-400/10 dark:text-amber-300',
    dot: 'bg-amber-500',
  },
  cancelled: {
    badge: 'border-rose-200 bg-rose-50 text-rose-700 dark:border-rose-400/20 dark:bg-rose-400/10 dark:text-rose-300',
    dot: 'bg-rose-500',
  },
  declined: {
    badge: 'border-rose-200 bg-rose-50 text-rose-700 dark:border-rose-400/20 dark:bg-rose-400/10 dark:text-rose-300',
    dot: 'bg-rose-500',
  },
}

export default {
  name: 'StatusBadge',
  props: {
    status: { type: [String, Number], default: '' },
  },
  computed: {
    label() {
      return statusLabel(this.status)
    },
    tone() {
      const value = this.label.toLowerCase()
      return TONES[value] || {
        badge: 'border-slate-200 bg-slate-50 text-slate-600 dark:border-white/[0.08] dark:bg-white/[0.04] dark:text-slate-300',
        dot: 'bg-slate-400',
      }
    },
    toneClass() {
      return this.tone.badge
    },
    dotClass() {
      return this.tone.dot
    },
  },
}
</script>
