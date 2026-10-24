<template>
  <div class="card card-hover flex items-start gap-4 p-5">
    <div class="icon-tile" :class="toneWrap">
      <Icon :icon="icon" class="text-xl" />
    </div>
    <div class="min-w-0 flex-1">
      <p class="text-[12px] font-semibold uppercase tracking-[0.1em] text-slate-400 dark:text-slate-500">
        {{ label }}
      </p>
      <div v-if="loading" class="mt-2 h-7 w-24 skeleton" />
      <p v-else class="stat-value mt-2 truncate">{{ value }}</p>
      <p v-if="hint" class="mt-1.5 text-xs text-slate-500 dark:text-slate-400">{{ hint }}</p>
    </div>
  </div>
</template>

<script>
const TONES = {
  teal: 'bg-teal-50 text-teal-600 dark:bg-teal-400/10 dark:text-teal-300',
  cyan: 'bg-cyan-50 text-cyan-600 dark:bg-cyan-400/10 dark:text-cyan-300',
  violet: 'bg-violet-50 text-violet-600 dark:bg-violet-400/10 dark:text-violet-300',
  amber: 'bg-amber-50 text-amber-600 dark:bg-amber-400/10 dark:text-amber-300',
  emerald: 'bg-emerald-50 text-emerald-600 dark:bg-emerald-400/10 dark:text-emerald-300',
  rose: 'bg-rose-50 text-rose-600 dark:bg-rose-400/10 dark:text-rose-300',
}

export default {
  name: 'StatCard',
  props: {
    label: { type: String, required: true },
    value: { type: [String, Number], default: '' },
    hint: { type: String, default: '' },
    icon: { type: String, default: 'activity' },
    tone: { type: String, default: 'teal' },
    loading: { type: Boolean, default: false },
  },
  computed: {
    toneWrap() {
      return TONES[this.tone] || TONES.teal
    },
  },
}
</script>
