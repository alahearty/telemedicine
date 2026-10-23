<template>
  <div class="feedback-page p-6 max-w-4xl mx-auto">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-2">
      Doctor Feedback
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-6">
      Share your experience to help us improve care quality
    </p>

    <div class="card-modern p-6">
      <form @submit.prevent="submit" class="space-y-6">
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Appointment *</label>
          <select v-model="form.appointmentId" class="input-modern w-full" required>
            <option value="">Select an appointment</option>
            <option v-for="apt in completedAppointments" :key="apt.id" :value="apt.id">
              {{ apt.serviceName }} — {{ apt.date }}
            </option>
          </select>
          <p v-if="!completedAppointments.length" class="text-sm text-slate-500 dark:text-slate-400 mt-1">
            No completed appointments yet. Complete a consultation to leave feedback.
          </p>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Rating *</label>
          <div class="flex gap-2">
            <button
              v-for="n in 5"
              :key="n"
              type="button"
              :class="['w-12 h-12 rounded-xl flex items-center justify-center text-xl transition-colors', form.rating >= n ? 'bg-amber-400 text-amber-900' : 'bg-slate-100 dark:bg-slate-700 text-slate-400 hover:bg-slate-200 dark:hover:bg-slate-600']"
              @click="form.rating = n"
            >
              ★
            </button>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Comments</label>
          <textarea
            v-model="form.comments"
            rows="4"
            class="input-modern w-full"
            placeholder="How was your experience? Any suggestions?"
          />
        </div>

        <div class="flex gap-3">
          <button type="submit" :disabled="saving || !completedAppointments.length" class="btn-primary">
            {{ saving ? 'Submitting...' : 'Submit Feedback' }}
          </button>
          <router-link to="/dashboard" class="px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-600 text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors">
            Cancel
          </router-link>
        </div>
      </form>
    </div>

    <div v-if="submitted" class="card-modern p-6 mt-6 text-center text-green-600 dark:text-green-400">
      <p class="font-medium">Thank you for your feedback!</p>
      <p class="text-sm mt-1">Your input helps us improve our services.</p>
    </div>
  </div>
</template>

<script>
import { getPatientAppointments, getCurrentPatient } from '@/api'

export default {
  name: 'DoctorFeedback',
  data() {
    return {
      completedAppointments: [],
      form: {
        appointmentId: '',
        rating: 0,
        comments: '',
      },
      saving: false,
      submitted: false,
    }
  },
  methods: {
    async loadAppointments() {
      try {
        const userId = localStorage.getItem('user-id')
        if (!userId) {
          const p = await getCurrentPatient()
          if (p?.id) localStorage.setItem('user-id', String(p.id))
        }
        const id = localStorage.getItem('user-id')
        if (id) {
          const data = await getPatientAppointments(id)
          const list = Array.isArray(data) ? data : (data?.data ?? [])
          this.completedAppointments = list
            .filter(a => a.status === 'Completed' || a.status === 'Active')
            .map(a => ({
              id: a.id,
              serviceName: a.serviceName || a.telemedicineService?.serviceName || 'Consultation',
              date: a.appointmentDate?.split('T')[0] || a.scheduleStartTime?.split('T')[0] || 'N/A',
            }))
        }
      } catch (e) {
        console.warn('Could not load appointments:', e)
      }
    },
    async submit() {
      if (!this.form.appointmentId || !this.form.rating) return
      this.saving = true
      try {
        // TODO: Wire to feedback API when backend endpoint exists
        await new Promise(r => setTimeout(r, 500))
        if (this.$notification) this.$notification.success('Thank you', 'Your feedback has been submitted.')
        this.submitted = true
        this.form = { appointmentId: '', rating: 0, comments: '' }
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message || 'Could not submit.')
      } finally {
        this.saving = false
      }
    },
  },
  mounted() {
    this.loadAppointments()
  },
}
</script>
