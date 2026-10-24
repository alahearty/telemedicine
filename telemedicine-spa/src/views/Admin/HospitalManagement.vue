<template>
  <div class="hospital-management p-6">
    <div class="flex items-center justify-between mb-8">
      <h2 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100">
        Hospital Management
      </h2>
      <button class="btn-primary" @click="openCreateModal">
        Add Hospital
      </button>
    </div>
    <div v-if="loading" class="card-modern p-12 text-center">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent mx-auto"></div>
      <p class="text-slate-500 dark:text-slate-400 mt-4">Loading hospitals...</p>
    </div>
    <div v-else class="card-modern overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead>
            <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-700">
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">ID</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Name</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Address</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="h in hospitals" :key="h.id" class="border-b border-slate-100 dark:border-slate-700/50 hover:bg-slate-50/50 dark:hover:bg-slate-800/30 transition-colors">
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ h.id }}</td>
              <td class="p-4 font-medium text-slate-800 dark:text-slate-100">{{ h.name }}</td>
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ h.address }}</td>
              <td class="p-4">
                <button class="text-primary-600 hover:text-primary-700 dark:text-primary-400 font-medium mr-4" @click="openEditModal(h)">
                  Edit
                </button>
                <button class="text-red-500 hover:text-red-600 font-medium" @click="confirmDelete(h)">
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="!hospitals.length && !loading" class="p-12 text-center text-slate-500 dark:text-slate-400">
        No hospitals yet. Add one to get started.
      </div>
    </div>
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal card-modern p-6 max-w-md animate-fade-in">
        <h3 class="font-display text-xl font-semibold mb-6 text-slate-800 dark:text-slate-100">
          {{ editingId ? 'Edit Hospital' : 'Add Hospital' }}
        </h3>
        <form @submit.prevent="saveHospital" class="space-y-4">
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Name</label>
            <input v-model="form.name" class="input-modern" required />
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Address</label>
            <input v-model="form.address" class="input-modern" required />
          </div>
          <div class="flex gap-3 pt-2">
            <button type="submit" class="btn-primary flex-1">Save</button>
            <button type="button" class="btn-secondary flex-1" @click="closeModal">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { getHospitals, createHospital, updateHospital, deleteHospital } from '@/api'

export default {
  name: 'HospitalManagement',
  data() {
    return {
      hospitals: [],
      loading: false,
      showModal: false,
      editingId: null,
      form: { name: '', address: '' },
    }
  },
  mounted() {
    this.loadHospitals()
  },
  methods: {
    async loadHospitals() {
      this.loading = true
      try {
        const data = await getHospitals()
        this.hospitals = Array.isArray(data) ? data : []
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      } finally {
        this.loading = false
      }
    },
    openCreateModal() {
      this.editingId = null
      this.form = { name: '', address: '' }
      this.showModal = true
    },
    openEditModal(h) {
      this.editingId = h.id
      this.form = { name: h.name, address: h.address }
      this.showModal = true
    },
    closeModal() {
      this.showModal = false
    },
    async saveHospital() {
      try {
        if (this.editingId) {
          await updateHospital(this.editingId, this.form)
          if (this.$notification) this.$notification.success('Updated', 'Hospital updated successfully')
        } else {
          await createHospital(this.form)
          if (this.$notification) this.$notification.success('Created', 'Hospital created successfully')
        }
        this.closeModal()
        this.loadHospitals()
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      }
    },
    async confirmDelete(h) {
      if (!confirm(`Delete ${h.name}?`)) return
      try {
        await deleteHospital(h.id)
        if (this.$notification) this.$notification.success('Deleted', 'Hospital removed')
        this.loadHospitals()
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      }
    },
  },
}
</script>

<style scoped>
@reference "../../assets/tailwind.css";
.modal-overlay {
  @apply fixed inset-0 bg-black/50 flex items-center justify-center z-50 backdrop-blur-sm;
}
</style>
