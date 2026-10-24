<template>
  <div v-if="!isAdmin && !isClosed">
    <div v-if="compact" class="flex flex-wrap items-center gap-2">
      <button
        v-if="video.canJoin"
        type="button"
        class="inline-flex items-center gap-1.5 rounded-lg bg-gradient-to-r from-teal-500 to-cyan-500 px-2.5 py-1.5 text-[11px] font-semibold text-white shadow-sm transition hover:from-teal-400 hover:to-cyan-400 disabled:opacity-60"
        :disabled="busy"
        @click="joinVideo"
      >
        <Icon icon="video" class="text-xs" />
        {{ busy ? 'Opening...' : 'Join video' }}
      </button>
      <router-link
        v-if="chat.canChat"
        :to="chatLink"
        class="inline-flex items-center gap-1.5 rounded-lg border border-slate-200 bg-white px-2.5 py-1.5 text-[11px] font-semibold text-slate-600 transition hover:border-primary-200 hover:text-primary-700 dark:border-white/[0.1] dark:bg-white/[0.03] dark:text-slate-300 dark:hover:border-primary-400/25 dark:hover:text-primary-300"
      >
        <Icon icon="message-circle" class="text-xs" />
        Message
      </router-link>
      <span v-if="showStatusHint" class="text-[11px] font-medium text-slate-400 dark:text-slate-500">
        {{ statusHint }}
      </span>
      <span v-else-if="!video.canJoin && (video.state === 'waiting' || video.state === 'closed')" class="text-[11px] font-medium text-slate-400 dark:text-slate-500">
        {{ video.detail }}
      </span>
    </div>

    <div v-else class="grid gap-3 sm:grid-cols-2">
      <div class="rounded-xl border border-slate-200/80 bg-slate-50/50 p-4 dark:border-white/[0.07] dark:bg-white/[0.02]">
        <div class="flex items-center gap-2">
          <span class="icon-tile-sm bg-teal-50 text-teal-600 dark:bg-teal-400/10 dark:text-teal-300">
            <Icon icon="video" class="text-base" />
          </span>
          <div>
            <p class="text-xs font-semibold text-ink-900 dark:text-slate-100">Video consultation</p>
            <p class="text-[11px] text-slate-500 dark:text-slate-400">{{ video.detail || 'Secure room for this appointment.' }}</p>
          </div>
        </div>
        <button
          v-if="video.canJoin"
          type="button"
          data-testid="join-video"
          class="btn-primary mt-4 w-full py-2 text-xs"
          :disabled="busy"
          @click="joinVideo"
        >
          <Icon icon="video" class="text-sm" />
          {{ busy ? 'Opening room...' : 'Join video' }}
        </button>
        <p v-else class="mt-4 rounded-lg bg-white/70 px-3 py-2 text-[11px] font-medium text-slate-500 dark:bg-white/[0.03] dark:text-slate-400">
          {{ video.label }}
        </p>
      </div>

      <div class="rounded-xl border border-slate-200/80 bg-slate-50/50 p-4 dark:border-white/[0.07] dark:bg-white/[0.02]">
        <div class="flex items-center gap-2">
          <span class="icon-tile-sm bg-cyan-50 text-cyan-600 dark:bg-cyan-400/10 dark:text-cyan-300">
            <Icon icon="message-circle" class="text-base" />
          </span>
          <div>
            <p class="text-xs font-semibold text-ink-900 dark:text-slate-100">Secure chat</p>
            <p class="text-[11px] text-slate-500 dark:text-slate-400">{{ chat.detail }}</p>
          </div>
        </div>
        <router-link
          v-if="chat.canChat"
          :to="chatLink"
          class="btn-secondary mt-4 w-full py-2 text-xs"
        >
          <Icon icon="message-circle" class="text-sm" />
          Message {{ isPhysician ? 'patient' : 'care team' }}
        </router-link>
        <p v-else class="mt-4 rounded-lg bg-white/70 px-3 py-2 text-[11px] font-medium text-slate-500 dark:bg-white/[0.03] dark:text-slate-400">
          {{ chat.label }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import { createOrGetConsultation } from '@/api'
import { chatAccessFor, ensureConsultationPolicy, useConsultationPolicy, videoAccessFor } from '@/composables/useConsultationPolicy'
import { statusLabel } from '@/utils/appointments'

export default {
  name: 'AppointmentActions',
  props: {
    appointment: { type: Object, required: true },
    compact: { type: Boolean, default: false },
  },
  setup() {
    const { state } = useConsultationPolicy()
    return { policyState: state }
  },
  data() {
    return { busy: false }
  },
  computed: {
    role() {
      const value = (localStorage.getItem('user-role') || '').toLowerCase()
      if (this.$route.path.startsWith('/physician')) return 'physician'
      if (value.includes('admin') || value.includes('administrator')) return 'admin'
      return 'patient'
    },
    isAdmin() {
      return this.role === 'admin'
    },
    isPhysician() {
      return this.role === 'physician'
    },
    status() {
      return statusLabel(this.appointment?.status).toLowerCase()
    },
    isClosed() {
      return this.status === 'cancelled' || this.status === 'declined'
    },
    chat() {
      return chatAccessFor(this.appointment)
    },
    video() {
      return videoAccessFor(this.appointment, this.policyState.policy)
    },
    chatLink() {
      return {
        path: this.isPhysician ? '/physician/chat' : '/dashboard/chat',
        query: { appointmentId: String(this.appointment?.id ?? '') },
      }
    },
    showStatusHint() {
      if (this.status !== 'pending') return false
      return true
    },
    statusHint() {
      return this.status === 'pending' ? 'Awaiting confirmation' : ''
    },
  },
  mounted() {
    if (!this.isAdmin) ensureConsultationPolicy()
  },
  methods: {
    async joinVideo() {
      if (!this.appointment?.id || this.busy) return
      this.busy = true
      try {
        const session = await createOrGetConsultation(this.appointment.id)
        this.$router.push({
          name: this.isPhysician ? 'PhysicianConsultation' : 'PatientConsultation',
          params: { sessionId: session.sessionId },
        })
      } catch (error) {
        if (this.$notification) this.$notification.error('Video unavailable', error.message || 'The consultation room is not available.')
      } finally {
        this.busy = false
      }
    },
  },
}
</script>
