<template>
  <div class="hospital-management p-6">
    <h2 class="text-2xl font-bold mb-6">Hospital Management</h2>
    <div class="mb-4">
      <button class="btn-primary px-4 py-2 rounded" @click="openCreateModal">
        Add Hospital
      </button>
    </div>
    <div class="table-container overflow-x-auto">
      <table class="w-full border-collapse">
        <thead>
          <tr class="bg-gray-100 dark:bg-gray-700">
            <th class="p-3 text-left">ID</th>
            <th class="p-3 text-left">Name</th>
            <th class="p-3 text-left">Address</th>
            <th class="p-3 text-left">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="h in hospitals" :key="h.id" class="border-b dark:border-gray-600">
            <td class="p-3">{{ h.id }}</td>
            <td class="p-3">{{ h.name }}</td>
            <td class="p-3">{{ h.address }}</td>
            <td class="p-3">
              <button class="text-blue-600 mr-2" @click="openEditModal(h)">Edit</button>
              <button class="text-red-600" @click="confirmDelete(h)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal bg-white dark:bg-gray-800 p-6 rounded-lg max-w-md">
        <h3 class="text-xl mb-4">{{ editingId ? 'Edit Hospital' : 'Add Hospital' }}</h3>
        <form @submit.prevent="saveHospital">
          <div class="mb-4">
            <label class="block mb-2">Name</label>
            <input v-model="form.name" class="w-full p-2 border rounded dark:bg-gray-700" required />
          </div>
          <div class="mb-4">
            <label class="block mb-2">Address</label>
            <input v-model="form.address" class="w-full p-2 border rounded dark:bg-gray-700" required />
          </div>
          <div class="flex gap-2">
            <button type="submit" class="btn-primary px-4 py-2 rounded">Save</button>
            <button type="button" class="px-4 py-2 border rounded" @click="closeModal">Cancel</button>
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
      try {
        const data = await getHospitals()
        this.hospitals = Array.isArray(data) ? data : []
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
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
.modal-overlay {
  @apply fixed inset-0 bg-black/50 flex items-center justify-center z-50;
}
.btn-primary {
  @apply bg-indigo-500 text-white hover:bg-indigo-600;
}
</style>
