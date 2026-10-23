<template>
  <div class="max-w-4xl mx-auto p-6">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100">Working schedule</h1>
    <p class="text-slate-600 dark:text-slate-400 mt-1 mb-6">
      Publish the recurring hours patients can book. All times are interpreted as UTC.
    </p>

    <div v-if="loading" class="flex items-center gap-3 py-16 text-slate-500">
      <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading schedule...</span>
    </div>

    <form v-else @submit.prevent="save" class="card-modern p-6 space-y-4">
      <div v-for="(row, index) in rows" :key="row.day" class="grid grid-cols-1 md:grid-cols-12 gap-3 items-center border-b border-slate-100 dark:border-slate-700 pb-4 last:border-0">
        <label class="md:col-span-3 inline-flex items-center gap-2 text-sm font-medium text-slate-700 dark:text-slate-200">
          <input v-model="row.enabled" type="checkbox" class="rounded border-slate-300" />
          {{ row.dayName }}
        </label>
        <div class="md:col-span-3">
          <label class="sr-only">Start time</label>
          <input v-model="row.start" :disabled="!row.enabled" type="time" class="input-modern w-full" />
        </div>
        <div class="md:col-span-3">
          <label class="sr-only">End time</label>
          <input v-model="row.end" :disabled="!row.enabled" type="time" class="input-modern w-full" />
        </div>
        <div class="md:col-span-2">
          <label class="sr-only">Slot duration</label>
          <select v-model.number="row.duration" :disabled="!row.enabled" class="input-modern w-full">
            <option :value="15">15 min</option>
            <option :value="30">30 min</option>
            <option :value="45">45 min</option>
            <option :value="60">60 min</option>
          </select>
        </div>
        <div class="md:col-span-1 text-xs text-slate-400">
          <span v-if="row.enabled">UTC</span>
        </div>
      </div>

      <p v-if="error" class="rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">{{ error }}</p>
      <p v-if="saved" class="rounded-xl border border-green-200 bg-green-50 p-3 text-sm text-green-700 dark:border-green-800 dark:bg-green-900/20 dark:text-green-300">
        Schedule saved. Patients will see the new slots immediately.
      </p>

      <div class="flex flex-wrap gap-3 pt-2">
        <button type="submit" class="btn-primary" :disabled="saving">{{ saving ? 'Saving...' : 'Save schedule' }}</button>
        <router-link to="/physician" class="btn-secondary">Back to dashboard</router-link>
        <router-link to="/physician/appointments" class="btn-secondary">View appointments</router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { getCurrentPhysician, getPhysicianAvailability, replacePhysicianAvailability } from '@/api'

const DAYS = [
  { day: 0, name: 'Sunday' },
  { day: 1, name: 'Monday' },
  { day: 2, name: 'Tuesday' },
  { day: 3, name: 'Wednesday' },
  { day: 4, name: 'Thursday' },
  { day: 5, name: 'Friday' },
  { day: 6, name: 'Saturday' },
]

export default {
  name: 'AvailabilityPhysician',
  data() {
    return {
      rows: DAYS.map(d => ({
        day: d.day,
        dayName: d.name,
        enabled: false,
        start: '08:00',
        end: '16:00',
        duration: 30,
      })),
      loading: true,
      saving: false,
      error: '',
      saved: false,
    }
  },
  methods: {
    normalizeTime(value, fallback) {
      if (!value) return fallback
      return String(value).slice(0, 5)
    },
    async load() {
      this.loading = true
      this.error = ''
      try {
        const physician = await getCurrentPhysician()
        const id = physician?.id ?? physician?.data?.id
        if (!id) throw new Error('Physician profile not found.')
        localStorage.setItem('physician-id', String(id))
        const rules = await getPhysicianAvailability(id)
        for (const row of this.rows) {
          const rule = (Array.isArray(rules) ? rules : []).find(r => Number(r.dayOfWeek) === row.day && r.isActive)
          if (rule) {
            row.enabled = true
            row.start = this.normalizeTime(rule.startTime, row.start)
            row.end = this.normalizeTime(rule.endTime, row.end)
            row.duration = Number(rule.slotDurationMinutes) || 30
          }
        }
      } catch (e) {
        this.error = e.message || 'Could not load your schedule.'
      } finally {
        this.loading = false
      }
    },
    async save() {
      this.error = ''
      this.saved = false
      const enabled = this.rows.filter(r => r.enabled)
      for (const row of enabled) {
        if (!row.start || !row.end || row.start >= row.end) {
          this.error = `${row.dayName}: end time must be after start time.`
          return
        }
      }
      this.saving = true
      try {
        await replacePhysicianAvailability(enabled.map(row => ({
          dayOfWeek: row.day,
          startTime: `${row.start}:00`,
          endTime: `${row.end}:00`,
          slotDurationMinutes: row.duration,
        })))
        this.saved = true
        if (this.$notification) this.$notification.success('Schedule saved', 'Availability has been updated.')
      } catch (e) {
        this.error = e.message || 'Could not save your schedule.'
      } finally {
        this.saving = false
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>
