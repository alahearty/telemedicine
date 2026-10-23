<template>
  <div>
    <div v-if="loading" class="flex items-center gap-3 py-6 text-slate-500 dark:text-slate-400">
      <div class="animate-spin rounded-full h-6 w-6 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading available slots...</span>
    </div>

    <div v-else-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
      {{ error }}
    </div>

    <div v-else-if="!groups.length" class="rounded-xl border border-slate-200 bg-slate-50 p-6 text-center text-sm text-slate-500 dark:border-slate-700 dark:bg-slate-800/50 dark:text-slate-400">
      No available slots in this period. Choose another doctor or date range.
    </div>

    <div v-else class="space-y-5">
      <div v-for="group in groups" :key="group.key">
        <p class="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-2">{{ group.label }}</p>
        <div class="flex flex-wrap gap-2">
          <button
            v-for="slot in group.slots"
            :key="slot.start"
            data-testid="slot-chip" type="button" class="slot-chip"
            :class="{ 'slot-chip-selected': slot.start === selectedStart }"
            @click="$emit('select', slot)"
          >
            {{ formatTime(slot.start) }}
          </button>
        </div>
      </div>
      <p class="text-xs text-slate-400">Times are shown in your local time zone. Slots are stored in UTC.</p>
    </div>
  </div>
</template>

<script>
import { getPhysicianSlots } from '@/api'

export default {
  name: 'SlotPicker',
  props: {
    physicianId: { type: [Number, String], required: true },
    fromUtc: { type: String, default: '' },
    toUtc: { type: String, default: '' },
    selectedStart: { type: String, default: '' },
    refreshKey: { type: Number, default: 0 },
  },
  emits: ['select'],
  data() {
    return {
      slots: [],
      loading: false,
      error: '',
    }
  },
  computed: {
    groups() {
      const map = new Map()
      for (const slot of this.slots) {
        const date = new Date(slot.start)
        const key = date.toLocaleDateString(undefined, { year: 'numeric', month: '2-digit', day: '2-digit' })
        if (!map.has(key)) {
          map.set(key, {
            key,
            label: date.toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' }),
            slots: [],
          })
        }
        map.get(key).slots.push(slot)
      }
      return [...map.values()]
    },
  },
  watch: {
    physicianId() { this.load() },
    fromUtc() { this.load() },
    toUtc() { this.load() },
    refreshKey() { this.load() },
  },
  methods: {
    formatTime(value) {
      return new Date(value).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })
    },
    async load() {
      if (!this.physicianId) {
        this.slots = []
        return
      }
      this.loading = true
      this.error = ''
      try {
        const data = await getPhysicianSlots(this.physicianId, this.fromUtc || undefined, this.toUtc || undefined)
        this.slots = Array.isArray(data) ? data : []
      } catch (e) {
        this.slots = []
        this.error = e.message || 'Could not load available slots.'
      } finally {
        this.loading = false
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>

<style scoped>
.slot-chip {
  @apply px-4 py-2 rounded-xl border border-slate-200 bg-white text-sm font-medium text-slate-700 hover:border-primary-400 hover:text-primary-600 transition-colors dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200;
}
.slot-chip-selected {
  @apply border-primary-500 bg-primary-500 text-white hover:text-white dark:bg-primary-500;
}
</style>
