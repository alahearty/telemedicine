<template>
  <div class="physician-profile p-6 max-w-3xl mx-auto">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-6">
      Profile Settings
    </h1>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <form v-else @submit.prevent="save" class="space-y-6">
      <div class="card-modern p-6 space-y-4">
        <h2 class="font-semibold text-slate-800 dark:text-slate-100">Personal Information</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">First Name</label>
            <input v-model="form.firstName" type="text" class="input-modern w-full" required />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Last Name</label>
            <input v-model="form.lastName" type="text" class="input-modern w-full" required />
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email</label>
            <input v-model="form.email" type="email" class="input-modern w-full" readonly />
            <p class="text-xs text-slate-500 mt-1">Email cannot be changed here</p>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone</label>
            <input v-model="form.phone" type="tel" class="input-modern w-full" />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Specialization</label>
            <input v-model="form.medicalSpecialization" type="text" class="input-modern w-full" placeholder="e.g. General Practice" />
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Address</label>
            <textarea v-model="form.address" rows="3" class="input-modern w-full" />
          </div>
        </div>
      </div>

      <div class="flex gap-3">
        <button type="submit" :disabled="saving" class="btn-primary">
          {{ saving ? 'Saving...' : 'Save Changes' }}
        </button>
        <router-link to="/physician" class="px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-600 text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors">
          Cancel
        </router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { getCurrentPhysician, updatePhysician } from '@/api'

export default {
  name: 'ProfilePhysician',
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
        medicalSpecialization: '',
        address: '',
      },
    }
  },
  methods: {
    async load() {
      this.loading = true
      try {
        const p = await getCurrentPhysician()
        this.form = {
          id: p.id,
          firstName: p.firstName || '',
          lastName: p.lastName || '',
          email: p.email || '',
          phone: p.phone || '',
          medicalSpecialization: p.medicalSpecialization || '',
          address: p.address || '',
        }
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      } finally {
        this.loading = false
      }
    },
    async save() {
      if (!this.form.id) return
      this.saving = true
      try {
        await updatePhysician(this.form.id, {
          firstName: this.form.firstName,
          lastName: this.form.lastName,
          phone: this.form.phone,
          medicalSpecialization: this.form.medicalSpecialization,
          address: this.form.address,
        })
        if (this.$notification) this.$notification.success('Saved', 'Profile updated.')
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
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
