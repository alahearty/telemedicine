<template>
  <div class="account-page p-6 max-w-3xl mx-auto">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-6">
      Account Settings
    </h1>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <div v-else-if="error === 'admin'" class="card-modern p-6 text-center">
      <p class="text-slate-600 dark:text-slate-400 mb-4">
        Account settings are for patient profiles. As an administrator, you can manage the system from the Dashboard.
      </p>
      <router-link to="/dashboard" class="text-primary-600 dark:text-primary-400 font-medium hover:underline">
        Go to Dashboard →
      </router-link>
    </div>
    <div v-else-if="error" class="card-modern p-6 text-center">
      <p class="text-red-500 dark:text-red-400 mb-4">{{ error }}</p>
      <router-link to="/dashboard/update-record" class="text-primary-600 dark:text-primary-400 font-medium hover:underline">
        Complete your profile in Update Record →
      </router-link>
    </div>

    <form v-else @submit.prevent="save" class="space-y-6">
      <div class="card-modern p-6 space-y-4">
        <h2 class="font-semibold text-slate-800 dark:text-slate-100">Personal Information</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">First Name</label>
            <input
              v-model="form.firstName"
              type="text"
              class="input-modern w-full"
              placeholder="First name"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Last Name</label>
            <input
              v-model="form.lastName"
              type="text"
              class="input-modern w-full"
              placeholder="Last name"
            />
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email</label>
            <input
              v-model="form.email"
              type="email"
              class="input-modern w-full"
              placeholder="you@example.com"
              readonly
            />
            <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">Email cannot be changed here</p>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone</label>
            <input
              v-model="form.phone"
              type="tel"
              class="input-modern w-full"
              placeholder="+1 234 567 8900"
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
          {{ saving ? 'Saving...' : 'Save Changes' }}
        </button>
        <router-link to="/dashboard" class="px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-600 text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors">
          Cancel
        </router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { getCurrentPatient, updatePatient } from '@/api'

export default {
  name: 'Account',
  data() {
    return {
      loading: true,
      saving: false,
      error: null,
      form: {
        id: null,
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
      },
    }
  },
  computed: {
    isAdmin() {
      const role = (localStorage.getItem('user-role') || '').toLowerCase()
      return role.includes('admin') || role.includes('administrator')
    },
  },
  methods: {
    async load() {
      this.loading = true
      this.error = null
      if (this.isAdmin) {
        this.loading = false
        this.error = 'admin'
        return
      }
      try {
        const p = await getCurrentPatient()
        this.form = {
          id: p.id,
          firstName: p.firstName || '',
          lastName: p.lastName || '',
          email: p.email || '',
          phone: p.phone || '',
        }
      } catch (e) {
        this.error = e.message || 'Could not load your profile. Please complete your profile in Update Record first.'
      } finally {
        this.loading = false
      }
    },
    async save() {
      if (!this.form.id) {
        this.error = 'Please complete your profile in Update Record first.'
        return
      }
      this.saving = true
      try {
        await updatePatient(this.form.id, {
          firstName: this.form.firstName,
          lastName: this.form.lastName,
          phone: this.form.phone,
        })
        if (this.$notification) this.$notification.success('Saved', 'Your profile has been updated.')
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
