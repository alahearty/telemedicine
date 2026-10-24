<template>
  <div class="space-y-6">
    <PageHeader
      eyebrow="Physician workspace"
      :title="displayName ? `Welcome back, ${displayName}` : 'Welcome back'"
      subtitle="Manage your consultations, availability and patient care from one calm workspace."
    >
      <template #actions>
        <router-link to="/physician/availability" class="btn-secondary">
          <Icon icon="clock" class="text-base" />
          Manage availability
        </router-link>
        <router-link to="/physician/appointments" class="btn-primary">
          <Icon icon="calendar-check" class="text-base" />
          View appointments
        </router-link>
      </template>
    </PageHeader>

    <div class="grid grid-cols-2 gap-4 xl:grid-cols-4">
      <StatCard label="Total appointments" :value="stats.totalAppointments" hint="All assigned visits" icon="calendar" tone="teal" />
      <StatCard label="Unattended" :value="stats.unattended" hint="Requests needing attention" icon="bell" tone="amber" />
      <StatCard label="Completed" :value="stats.completed" hint="Attended consultations" icon="user-check" tone="emerald" />
      <StatCard label="Active" :value="stats.active" hint="In progress or confirmed" icon="calendar-check" tone="violet" />
    </div>

    <div class="grid gap-6 xl:grid-cols-2">
      <section class="card overflow-hidden">
        <div class="flex items-center justify-between border-b border-slate-100 px-5 py-4 dark:border-white/[0.06] sm:px-6">
          <div>
            <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Upcoming appointments</h2>
            <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Your next patient visits</p>
          </div>
          <router-link to="/physician/appointments" class="inline-flex items-center gap-1.5 text-xs font-semibold text-primary-700 hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200">
            View all
            <Icon icon="arrow-right" class="text-xs" />
          </router-link>
        </div>

        <div v-if="upcomingAppointments.length" class="divide-y divide-slate-100 dark:divide-white/[0.06]">
          <article
            v-for="appointment in upcomingAppointments"
            :key="appointment.id"
            class="flex flex-col gap-3 px-5 py-4 transition-colors hover:bg-slate-50/70 dark:hover:bg-white/[0.02] sm:flex-row sm:items-center sm:px-6"
          >
            <div class="flex h-14 w-14 shrink-0 flex-col items-center justify-center rounded-xl bg-primary-50 text-primary-800 dark:bg-primary-400/10 dark:text-primary-300">
              <span class="font-display text-lg font-bold leading-none">{{ appointment.dateParts.day }}</span>
              <span class="mt-0.5 text-[10px] font-semibold uppercase tracking-wide">{{ appointment.dateParts.month }}</span>
            </div>
            <div class="min-w-0 flex-1">
              <p class="truncate text-sm font-semibold text-ink-900 dark:text-slate-100">{{ appointment.serviceName }}</p>
              <p class="mt-1 truncate text-xs text-slate-500 dark:text-slate-400">
                {{ appointment.patientName || 'Patient' }}
              </p>
              <p class="mt-1 text-xs text-slate-400 dark:text-slate-500">{{ appointment.dateParts.weekday }} &middot; {{ appointment.dateParts.time }}</p>
            </div>
            <div class="flex flex-wrap items-center gap-2 sm:justify-end">
              <StatusBadge :status="appointment.rawStatus" />
              <AppointmentActions :appointment="appointment.source" compact />
              <router-link :to="`/physician/appointments/${appointment.id}`" class="btn-ghost px-2.5 py-1.5 text-xs">
                Details
              </router-link>
            </div>
          </article>
        </div>
        <EmptyState
          v-else
          icon="calendar-check"
          title="No upcoming appointments"
          description="New requests and confirmed visits will appear here."
        />
      </section>

      <section class="card p-5 sm:p-6">
        <div class="flex items-center justify-between">
          <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Quick actions</h2>
          <Icon icon="sparkles" class="text-lg text-primary-400" />
        </div>
        <div class="mt-4 space-y-1">
          <router-link
            v-for="action in quickActions"
            :key="action.to"
            :to="action.to"
            class="group flex items-center gap-3 rounded-xl px-2.5 py-3 transition-colors hover:bg-slate-50 dark:hover:bg-white/[0.04]"
          >
            <span class="icon-tile-sm" :class="action.tone">
              <Icon :icon="action.icon" class="text-base" />
            </span>
            <span class="min-w-0 flex-1">
              <span class="block text-xs font-semibold text-slate-700 dark:text-slate-300">{{ action.label }}</span>
              <span class="mt-0.5 block text-[11px] text-slate-400 dark:text-slate-500">{{ action.description }}</span>
            </span>
            <Icon icon="chevron-right" class="text-xs text-slate-300 transition-transform group-hover:translate-x-0.5 dark:text-slate-600" />
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import AppointmentActions from '@/components/appointments/AppointmentActions.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import PageHeader from '@/components/ui/PageHeader.vue'
import StatCard from '@/components/ui/StatCard.vue'
import StatusBadge from '@/components/ui/StatusBadge.vue'
import { setSessionName } from '@/composables/useSession'
import { getCurrentPhysician, getPhysicianAppointments, getUnattendedAppointments } from '@/api'
import { formatDateParts, isUpcoming, statusLabel as labelForStatus } from '@/utils/appointments'

const QUICK_ACTIONS = [
  { label: 'Appointments', description: 'Review and respond to requests', to: '/physician/appointments', icon: 'calendar-check', tone: 'bg-teal-50 text-teal-600 dark:bg-teal-400/10 dark:text-teal-300' },
  { label: 'Unattended requests', description: 'Requests waiting for a response', to: '/physician/unattended', icon: 'bell', tone: 'bg-amber-50 text-amber-600 dark:bg-amber-400/10 dark:text-amber-300' },
  { label: 'Patient list', description: 'People in your care', to: '/physician/patients', icon: 'users', tone: 'bg-violet-50 text-violet-600 dark:bg-violet-400/10 dark:text-violet-300' },
  { label: 'Availability', description: 'Shape your weekly schedule', to: '/physician/availability', icon: 'clock', tone: 'bg-cyan-50 text-cyan-600 dark:bg-cyan-400/10 dark:text-cyan-300' },
  { label: 'Profile', description: 'Professional details and bio', to: '/physician/profile', icon: 'user', tone: 'bg-emerald-50 text-emerald-600 dark:bg-emerald-400/10 dark:text-emerald-300' },
]

export default {
  name: 'DashboardPhysician',
  components: { AppointmentActions, EmptyState, PageHeader, StatCard, StatusBadge },
  data() {
    return {
      profile: null,
      appointments: [],
      unattended: [],
      quickActions: QUICK_ACTIONS,
    }
  },
  computed: {
    displayName() {
      if (this.profile?.firstName) {
        return `Dr. ${this.profile.firstName}${this.profile.lastName ? ` ${this.profile.lastName}` : ''}`
      }
      const stored = localStorage.getItem('user-name')
      if (stored) return stored.startsWith('Dr.') ? stored : `Dr. ${stored}`
      return null
    },
    stats() {
      const completed = this.appointments.filter((appointment) => labelForStatus(appointment.status) === 'Attended').length
      const active = this.appointments.filter((appointment) => labelForStatus(appointment.status) === 'Active').length
      return {
        totalAppointments: this.appointments.length,
        unattended: this.unattended.length,
        completed,
        active,
      }
    },
    upcomingAppointments() {
      return this.appointments
        .filter((appointment) => isUpcoming(appointment.status))
        .slice(0, 5)
        .map((appointment) => {
          const when = appointment.appointmentDate || appointment.scheduleStartTime
          return {
            id: appointment.id,
            serviceName: appointment.serviceName || appointment.telemedicineService?.serviceName || 'Consultation',
            patientName: appointment.patientName || '',
            rawStatus: appointment.status,
            dateParts: formatDateParts(when),
            source: appointment,
            sortValue: when ? new Date(when).getTime() : Number.MAX_SAFE_INTEGER,
          }
        })
        .sort((a, b) => a.sortValue - b.sortValue)
    },
  },
  methods: {
    async load() {
      try {
        const profile = await getCurrentPhysician()
        this.profile = profile
        if (profile?.id) localStorage.setItem('physician-id', String(profile.id))
        if (profile?.firstName) {
          setSessionName(`Dr. ${profile.firstName}${profile.lastName ? ` ${profile.lastName}` : ''}`)
        }
      } catch (error) {
        console.warn('Could not load physician:', error)
      }
      try {
        const id = localStorage.getItem('physician-id') || this.profile?.id
        if (id) {
          const data = await getPhysicianAppointments(id)
          this.appointments = Array.isArray(data) ? data : (data?.data ?? [])
        }
      } catch (error) {
        console.warn('Could not load appointments:', error)
      }
      try {
        const data = await getUnattendedAppointments()
        this.unattended = Array.isArray(data) ? data : (data?.data ?? [])
      } catch (error) {
        console.warn('Could not load unattended:', error)
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>
