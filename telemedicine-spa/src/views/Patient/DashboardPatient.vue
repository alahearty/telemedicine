<template>
  <div class="space-y-6">
    <section class="relative overflow-hidden rounded-[26px] bg-[linear-gradient(125deg,#0f766e_0%,#0d9488_45%,#0891b2_100%)] p-6 text-white shadow-glow sm:p-8">
      <div class="pointer-events-none absolute -right-16 -top-24 h-72 w-72 rounded-full bg-white/15 blur-2xl" />
      <div class="pointer-events-none absolute -bottom-28 left-1/3 h-64 w-64 rounded-full bg-cyan-200/20 blur-3xl" />
      <div class="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_85%_20%,rgba(255,255,255,0.18),transparent_45%)]" />

      <div class="relative grid items-center gap-8 lg:grid-cols-[1.25fr_0.75fr]">
        <div>
          <p class="text-[11px] font-semibold uppercase tracking-[0.18em] text-white/70">Patient overview</p>
          <h1 class="mt-3 font-display text-[30px] font-semibold leading-tight tracking-[-0.03em] text-white text-balance sm:text-[38px]">
            Welcome back{{ displayName ? `, ${displayName}` : '' }}
          </h1>
          <p class="mt-3 max-w-xl text-sm leading-6 text-white/80 sm:text-base">
            Your care is all in one place. Review upcoming visits, reach a specialist, or keep your health record current.
          </p>
          <div class="mt-6 flex flex-wrap items-center gap-3">
            <router-link to="/dashboard/book-service" class="inline-flex items-center gap-2 rounded-xl bg-white px-5 py-2.5 text-sm font-semibold text-teal-800 shadow-lg shadow-teal-950/20 transition-transform hover:-translate-y-0.5">
              <Icon icon="calendar-plus" class="text-base" />
              Book appointment
            </router-link>
            <router-link to="/dashboard/doctors" class="inline-flex items-center gap-2 rounded-xl border border-white/25 bg-white/10 px-5 py-2.5 text-sm font-semibold text-white backdrop-blur transition-colors hover:bg-white/20">
              <Icon icon="stethoscope" class="text-base" />
              Find a doctor
            </router-link>
          </div>
        </div>

        <div class="rounded-[20px] border border-white/20 bg-white/10 p-4 backdrop-blur-md">
          <div class="flex items-center justify-between">
            <p class="text-[11px] font-semibold uppercase tracking-[0.16em] text-white/70">Next appointment</p>
            <span v-if="nextAppointment" class="inline-flex items-center gap-1.5 rounded-full bg-white/15 px-2.5 py-1 text-[10px] font-semibold uppercase tracking-wide text-white/90">
              <span class="h-1.5 w-1.5 rounded-full bg-emerald-300" />
              {{ nextAppointment.status }}
            </span>
          </div>

          <div v-if="loading" class="mt-4 space-y-2">
            <div class="h-6 w-2/3 animate-pulse rounded-lg bg-white/15" />
            <div class="h-4 w-1/2 animate-pulse rounded-lg bg-white/10" />
            <div class="mt-4 h-12 w-full animate-pulse rounded-xl bg-white/10" />
          </div>

          <div v-else-if="nextAppointment" class="mt-4">
            <div class="flex items-start gap-3">
              <div class="flex h-14 w-14 shrink-0 flex-col items-center justify-center rounded-xl bg-white text-teal-800 shadow-sm">
                <span class="font-display text-lg font-bold leading-none">{{ nextAppointment.dateParts.day }}</span>
                <span class="mt-0.5 text-[10px] font-semibold uppercase tracking-wide">{{ nextAppointment.dateParts.month }}</span>
              </div>
              <div class="min-w-0">
                <p class="truncate font-display text-base font-semibold text-white">{{ nextAppointment.serviceName }}</p>
                <p class="mt-1 truncate text-xs text-white/75">
                  {{ nextAppointment.physicianName ? `Dr. ${nextAppointment.physicianName}` : 'Care team' }}
                  <span v-if="nextAppointment.physicianSpecialization"> &middot; {{ nextAppointment.physicianSpecialization }}</span>
                </p>
                <p class="mt-1 text-xs font-medium text-white/85">{{ nextAppointment.dateParts.full }} &middot; {{ nextAppointment.dateParts.time }}</p>
              </div>
            </div>
            <router-link :to="`/dashboard/appointments/${nextAppointment.id}`" class="mt-4 inline-flex w-full items-center justify-center gap-2 rounded-xl bg-white/15 px-4 py-2.5 text-xs font-semibold text-white transition-colors hover:bg-white/25">
              View appointment
              <Icon icon="arrow-right" class="text-sm" />
            </router-link>
            <AppointmentActions :appointment="nextAppointment.source" compact class="mt-3" />
          </div>

          <div v-else class="mt-4">
            <div class="flex items-center gap-3">
              <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-white/15">
                <Icon icon="calendar-check" class="text-xl" />
              </div>
              <div>
                <p class="font-display text-sm font-semibold text-white">All clear</p>
                <p class="text-xs leading-5 text-white/70">No upcoming appointments. Book a visit when you are ready.</p>
              </div>
            </div>
            <router-link to="/dashboard/book-service" class="mt-4 inline-flex w-full items-center justify-center gap-2 rounded-xl bg-white/15 px-4 py-2.5 text-xs font-semibold text-white transition-colors hover:bg-white/25">
              Book a consultation
              <Icon icon="arrow-right" class="text-sm" />
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <div class="grid grid-cols-2 gap-4 xl:grid-cols-4">
      <StatCard label="Upcoming" :value="stats.upcoming" hint="Scheduled visits" icon="calendar-check" tone="teal" :loading="loading" />
      <StatCard label="Pending" :value="stats.pending" hint="Awaiting confirmation" icon="clock" tone="amber" :loading="loading" />
      <StatCard label="Completed" :value="stats.completed" hint="Attended consultations" icon="user-check" tone="emerald" :loading="loading" />
      <StatCard label="Cancelled" :value="stats.cancelled" hint="Cancelled or declined" icon="calendar-x" tone="rose" :loading="loading" />
    </div>

    <div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <router-link
        v-for="action in quickActions"
        :key="action.to"
        :to="action.to"
        class="card card-hover group flex items-start gap-4 p-5"
      >
        <span class="icon-tile" :class="action.tone">
          <Icon :icon="action.icon" class="text-xl" />
        </span>
        <span class="min-w-0 flex-1">
          <span class="flex items-center gap-1.5 font-display text-sm font-semibold text-ink-900 dark:text-slate-100">
            {{ action.title }}
            <Icon icon="arrow-right" class="text-xs text-slate-300 transition-transform group-hover:translate-x-0.5 group-hover:text-primary-500 dark:text-slate-600" />
          </span>
          <span class="mt-1 block text-xs leading-5 text-slate-500 dark:text-slate-400">{{ action.description }}</span>
        </span>
      </router-link>
    </div>

    <div class="grid gap-6 xl:grid-cols-[minmax(0,1.65fr)_minmax(0,1fr)]">
      <section class="card overflow-hidden">
        <div class="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-4 dark:border-white/[0.06] sm:px-6">
          <div>
            <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Upcoming appointments</h2>
            <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Your next visits at a glance</p>
          </div>
          <router-link to="/dashboard/appointments" class="inline-flex items-center gap-1.5 text-xs font-semibold text-primary-700 hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200">
            View all
            <Icon icon="arrow-right" class="text-xs" />
          </router-link>
        </div>

        <div v-if="loading" class="space-y-3 p-5 sm:p-6">
          <div v-for="n in 3" :key="n" class="flex items-center gap-4 rounded-2xl border border-slate-100 p-4 dark:border-white/[0.06]">
            <div class="h-14 w-14 skeleton" />
            <div class="flex-1 space-y-2">
              <div class="h-4 w-1/3 skeleton" />
              <div class="h-3 w-1/2 skeleton" />
            </div>
          </div>
        </div>

        <div v-else-if="upcomingAppointments.length" class="divide-y divide-slate-100 dark:divide-white/[0.06]">
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
                {{ appointment.physicianName ? `Dr. ${appointment.physicianName}` : 'Care team' }}
                <span v-if="appointment.physicianSpecialization"> &middot; {{ appointment.physicianSpecialization }}</span>
              </p>
              <p class="mt-1 text-xs text-slate-400 dark:text-slate-500">{{ appointment.dateParts.weekday }} &middot; {{ appointment.dateParts.time }}</p>
            </div>

            <div class="flex flex-wrap items-center gap-2 sm:justify-end sm:pl-3">
              <StatusBadge :status="appointment.rawStatus" />
              <AppointmentActions :appointment="appointment.source" compact />
              <router-link
                :to="`/dashboard/appointments/${appointment.id}`"
                class="btn-ghost px-2.5 py-1.5 text-xs"
              >
                Details
              </router-link>
              <button
                type="button"
                class="btn-ghost px-2.5 py-1.5 text-xs text-slate-400 hover:bg-rose-50 hover:text-rose-600 dark:text-slate-500 dark:hover:bg-rose-400/10 dark:hover:text-rose-300"
                @click="askCancel(appointment)"
              >
                Cancel
              </button>
            </div>
          </article>
        </div>

        <EmptyState
          v-else
          icon="calendar-check"
          title="No upcoming appointments"
          description="When you book a consultation, your next visit will appear here with all the details."
        >
          <router-link to="/dashboard/book-service" class="btn-primary">
            <Icon icon="calendar-plus" class="text-base" />
            Book appointment
          </router-link>
        </EmptyState>
      </section>

      <div class="space-y-6">
        <section class="card p-5 sm:p-6">
          <div class="flex items-center justify-between">
            <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Shortcuts</h2>
            <Icon icon="sparkles" class="text-lg text-primary-400" />
          </div>
          <div class="mt-4 space-y-1">
            <router-link
              v-for="link in shortcuts"
              :key="link.to"
              :to="link.to"
              class="group flex items-center gap-3 rounded-xl px-2.5 py-2.5 transition-colors hover:bg-slate-50 dark:hover:bg-white/[0.04]"
            >
              <span class="icon-tile-sm bg-slate-100/80 text-slate-500 dark:bg-white/[0.05] dark:text-slate-400">
                <Icon :icon="link.icon" class="text-base" />
              </span>
              <span class="flex-1 text-xs font-semibold text-slate-700 group-hover:text-ink-900 dark:text-slate-300 dark:group-hover:text-white">{{ link.label }}</span>
              <Icon icon="chevron-right" class="text-xs text-slate-300 dark:text-slate-600" />
            </router-link>
          </div>
        </section>

        <section class="relative overflow-hidden rounded-[20px] border border-primary-100/80 bg-gradient-to-br from-primary-50 via-white to-cyan-50/70 p-5 dark:border-primary-400/15 dark:from-primary-400/[0.08] dark:via-midnight-850 dark:to-cyan-400/[0.04] sm:p-6">
          <div class="pointer-events-none absolute -right-8 -top-8 h-28 w-28 rounded-full bg-primary-200/40 blur-2xl dark:bg-primary-400/10" />
          <div class="relative">
            <span class="icon-tile-sm bg-white text-primary-600 shadow-sm dark:bg-white/[0.06] dark:text-primary-300">
              <Icon icon="help-circle" class="text-lg" />
            </span>
            <h2 class="mt-4 font-display text-base font-semibold text-ink-900 dark:text-slate-100">Need a hand?</h2>
            <p class="mt-1.5 text-xs leading-5 text-slate-600 dark:text-slate-400">
              Browse trusted physicians and choose a time that fits your day. Your care team is one click away.
            </p>
            <router-link to="/dashboard/doctors" class="mt-4 inline-flex items-center gap-1.5 text-xs font-semibold text-primary-700 hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200">
              Find a doctor
              <Icon icon="arrow-right" class="text-xs" />
            </router-link>
          </div>
        </section>
      </div>
    </div>

    <ConfirmDialog
      :open="Boolean(cancelTarget)"
      title="Cancel this appointment?"
      :message="cancelTarget ? `${cancelTarget.serviceName} on ${cancelTarget.dateParts.full} at ${cancelTarget.dateParts.time} will be released.` : ''"
      confirm-label="Cancel appointment"
      cancel-label="Keep appointment"
      danger
      @confirm="confirmCancel"
      @cancel="cancelTarget = null"
    />
  </div>
</template>

<script>
import AppointmentActions from '@/components/appointments/AppointmentActions.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import StatCard from '@/components/ui/StatCard.vue'
import StatusBadge from '@/components/ui/StatusBadge.vue'
import { cancelAppointment, getCurrentPatient, getPatientAppointments } from '@/api'
import { setSessionName } from '@/composables/useSession'
import { formatDateParts, formatTime, isUpcoming, statusLabel } from '@/utils/appointments'

const QUICK_ACTIONS = [
  {
    title: 'Book appointment',
    description: 'Schedule a consultation in a few taps',
    to: '/dashboard/book-service',
    icon: 'calendar-plus',
    tone: 'bg-teal-50 text-teal-600 dark:bg-teal-400/10 dark:text-teal-300',
  },
  {
    title: 'Find a doctor',
    description: 'Browse specialists and availability',
    to: '/dashboard/doctors',
    icon: 'stethoscope',
    tone: 'bg-cyan-50 text-cyan-600 dark:bg-cyan-400/10 dark:text-cyan-300',
  },
  {
    title: 'My appointments',
    description: 'Reschedule, review or cancel visits',
    to: '/dashboard/appointments',
    icon: 'calendar-check',
    tone: 'bg-violet-50 text-violet-600 dark:bg-violet-400/10 dark:text-violet-300',
  },
  {
    title: 'Update record',
    description: 'Keep your medical profile current',
    to: '/dashboard/update-record',
    icon: 'file-text',
    tone: 'bg-amber-50 text-amber-600 dark:bg-amber-400/10 dark:text-amber-300',
  },
]

const SHORTCUTS = [
  { label: 'Manage appointments', to: '/dashboard/appointments', icon: 'calendar-check' },
  { label: 'Find doctors', to: '/dashboard/doctors', icon: 'stethoscope' },
  { label: 'Update medical record', to: '/dashboard/update-record', icon: 'file-text' },
  { label: 'Account settings', to: '/dashboard/account', icon: 'settings' },
]

export default {
  name: 'DashboardPatient',
  components: { AppointmentActions, ConfirmDialog, EmptyState, StatCard, StatusBadge },
  data() {
    return {
      loading: true,
      appointments: [],
      profile: null,
      cancelTarget: null,
      quickActions: QUICK_ACTIONS,
      shortcuts: SHORTCUTS,
    }
  },
  computed: {
    displayName() {
      if (this.profile?.firstName) {
        return `${this.profile.firstName} ${this.profile.lastName || ''}`.trim()
      }
      const stored = localStorage.getItem('user-name')
      if (stored) return stored
      const email = localStorage.getItem('user-email') || ''
      return email.split('@')[0] || ''
    },
    mappedAppointments() {
      return this.appointments
        .map((appointment) => {
          const when = appointment.appointmentDate || appointment.scheduleStartTime
          const parts = formatDateParts(when)
          return {
            id: appointment.id,
            serviceName: appointment.serviceName || appointment.telemedicineService?.serviceName || 'Consultation',
            physicianName: appointment.physicianName || '',
            physicianSpecialization: appointment.physicianSpecialization || '',
            rawStatus: appointment.status,
            status: statusLabel(appointment.status),
            dateParts: { ...parts, time: parts.time || formatTime(when) },
            upcoming: isUpcoming(appointment.status),
            source: appointment,
            sortValue: when ? new Date(when).getTime() : Number.MAX_SAFE_INTEGER,
          }
        })
        .sort((a, b) => a.sortValue - b.sortValue)
    },
    upcomingAppointments() {
      return this.mappedAppointments.filter((appointment) => appointment.upcoming)
    },
    nextAppointment() {
      return this.upcomingAppointments[0] || null
    },
    stats() {
      const counts = { upcoming: 0, pending: 0, completed: 0, cancelled: 0 }
      for (const appointment of this.mappedAppointments) {
        const status = appointment.status.toLowerCase()
        if (appointment.upcoming) counts.upcoming += 1
        if (status === 'pending') counts.pending += 1
        if (status === 'attended' || status === 'completed') counts.completed += 1
        if (status === 'cancelled' || status === 'declined') counts.cancelled += 1
      }
      return counts
    },
  },
  async mounted() {
    await this.load()
  },
  methods: {
    async load() {
      this.loading = true
      try {
        try {
          const profile = await getCurrentPatient()
          this.profile = profile
          if (profile?.firstName) {
            const name = `${profile.firstName} ${profile.lastName || ''}`.trim()
            localStorage.setItem('user-name', name)
            setSessionName(name)
          }
          if (profile?.id) localStorage.setItem('user-id', String(profile.id))
        } catch (error) {
          console.warn('Could not load patient profile:', error)
        }

        try {
          const patientId = this.profile?.id || localStorage.getItem('user-id')
          if (patientId) {
            const data = await getPatientAppointments(patientId)
            this.appointments = Array.isArray(data) ? data : (data?.data ?? [])
          }
        } catch (error) {
          console.warn('Could not load appointments:', error)
        }
      } finally {
        this.loading = false
      }
    },
    askCancel(appointment) {
      this.cancelTarget = appointment
    },
    async confirmCancel() {
      const target = this.cancelTarget
      if (!target) return
      try {
        await cancelAppointment(target.id)
        if (this.$notification) this.$notification.success('Cancelled', 'The appointment was cancelled.')
        this.cancelTarget = null
        await this.load()
      } catch (error) {
        if (this.$notification) this.$notification.error('Error', error.message || 'Could not cancel the appointment.')
      }
    },
  },
}
</script>
