<template>
  <div class="patient-record p-6 max-w-3xl mx-auto">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-2">
      Update Record
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-6">
      Complete or update your medical profile. This information helps doctors provide better care.
    </p>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <form v-else @submit.prevent="submit" class="space-y-6">
      <div class="card-modern p-6 space-y-4">
        <h2 class="font-semibold text-slate-800 dark:text-slate-100">Personal Information</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">First Name *</label>
            <input
              v-model="form.firstName"
              type="text"
              class="input-modern w-full"
              placeholder="Jane"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Last Name *</label>
            <input
              v-model="form.lastName"
              type="text"
              class="input-modern w-full"
              placeholder="Doe"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email *</label>
            <input
              v-model="form.email"
              type="email"
              class="input-modern w-full"
              placeholder="you@example.com"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone *</label>
            <input
              v-model="form.phone"
              type="tel"
              class="input-modern w-full"
              placeholder="+1 234 567 8900"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Gender *</label>
            <select v-model="form.gender" class="input-modern w-full" required>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Birth Date *</label>
            <input
              v-model="form.birth"
              type="date"
              class="input-modern w-full"
              required
            />
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Address</label>
            <textarea
              v-model="form.address"
              rows="3"
              class="input-modern w-full"
              placeholder="Street, City, State, ZIP"
            />
          </div>
        </div>
      </div>

      <div class="flex gap-3">
        <button
          type="submit"
          :disabled="saving"
          class="btn-primary"
        >
          {{ saving ? 'Saving...' : (form.id ? 'Update Profile' : 'Create Profile') }}
        </button>
        <router-link to="/dashboard" class="px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-600 text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors">
          Cancel
        </router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { getCurrentPatient, createPatient, updatePatient } from '@/api'

export default {
  name: 'Patient',
  data() {
    return {
      loading: true,
      saving: false,
      form: {
        id: null,
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
        gender: 'Male',
        birth: '',
        address: '',
      },
    }
  },
  methods: {
    async load() {
      this.loading = true
      try {
        const p = await getCurrentPatient()
        this.form = {
          id: p.id,
          firstName: p.firstName || '',
          lastName: p.lastName || '',
          email: p.email || localStorage.getItem('user-email') || '',
          phone: p.phone || '',
          gender: p.gender || 'Male',
          birth: p.birth ? p.birth.split('T')[0] : '',
          address: p.address || '',
        }
      } catch {
        this.form.email = localStorage.getItem('user-email') || ''
      } finally {
        this.loading = false
      }
    },
    async submit() {
      this.saving = true
      try {
        if (this.form.id) {
          await updatePatient(this.form.id, {
            firstName: this.form.firstName,
            lastName: this.form.lastName,
            email: this.form.email,
            phone: this.form.phone,
          })
          if (this.$notification) this.$notification.success('Updated', 'Your profile has been updated.')
        } else {
          const created = await createPatient({
            firstName: this.form.firstName,
            lastName: this.form.lastName,
            email: this.form.email,
            phone: this.form.phone,
            gender: this.form.gender,
            birth: this.form.birth,
            address: this.form.address,
            accountType: 'Patient',
          })
          this.form.id = created?.id ?? created
          localStorage.setItem('user-id', String(this.form.id))
          if (this.$notification) this.$notification.success('Created', 'Your profile has been created.')
        }
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message || 'Could not save.')
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
