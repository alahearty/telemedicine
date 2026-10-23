<template>
  <div class="physician-patients p-6">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-1">
      Patients
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-8">
      View patients from your appointments
    </p>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <div v-else class="card-modern overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead>
            <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-700">
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">ID</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Name</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Email</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Phone</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in patients" :key="p.id" class="border-b border-slate-100 dark:border-slate-700/50 hover:bg-slate-50/50">
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ p.id }}</td>
              <td class="p-4 font-medium text-slate-800 dark:text-slate-100">{{ p.firstName }} {{ p.lastName }}</td>
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ p.email }}</td>
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ p.phone || '-' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="!patients.length && !loading" class="p-12 text-center text-slate-500">
        No patients yet. Patients will appear when they book appointments with you.
      </div>
    </div>
  </div>
</template>

<script>
import { getPhysicianAppointments, getCurrentPhysician, getPatient } from '@/api'

export default {
  name: 'PatientsPhysician',
  data() {
    return {
      patients: [],
      loading: false,
    }
  },
  methods: {
    async load() {
      this.loading = true
      try {
        const p = await getCurrentPhysician()
        const physicianId = p?.id || localStorage.getItem('physician-id')
        if (!physicianId) return
        const appointments = await getPhysicianAppointments(physicianId)
        const list = Array.isArray(appointments) ? appointments : (appointments?.data ?? [])
        const patientIds = [...new Set(list.map(a => a.patientId).filter(Boolean))]
        const patients = []
        for (const id of patientIds) {
          try {
            const patient = await getPatient(id)
            if (patient && !patients.find(p => p.id === patient.id)) patients.push(patient)
          } catch (_) {}
        }
        this.patients = patients
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      } finally {
        this.loading = false
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>
