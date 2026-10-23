<template>
  <div class="max-w-6xl mx-auto p-6">
    <div class="flex flex-wrap items-end justify-between gap-4 mb-6">
      <div>
        <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100">Find a doctor</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Browse physicians, their specialities and live availability.</p>
      </div>
      <input v-model="search" type="search" class="input-modern max-w-sm" placeholder="Search name, speciality or bio" />
    </div>

    <div v-if="loading" class="flex items-center gap-3 py-16 text-slate-500">
      <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading doctors...</span>
    </div>

    <div v-else-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
      {{ error }}
    </div>

    <div v-else-if="!filtered.length" class="card-modern p-12 text-center text-slate-500">
      No doctors match your search.
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
      <article v-for="doctor in filtered" :key="doctor.id" class="card-modern p-6 flex flex-col">
        <div class="flex items-center gap-4">
          <div class="w-14 h-14 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-lg font-bold dark:bg-primary-900/40 dark:text-primary-300">
            {{ initials(doctor) }}
          </div>
          <div class="min-w-0">
            <h2 class="font-semibold text-slate-800 dark:text-slate-100">Dr. {{ doctor.firstName }} {{ doctor.lastName }}</h2>
            <p class="text-sm text-primary-600 dark:text-primary-300">{{ doctor.medicalSpecialization || 'General Practice' }}</p>
            <p class="text-xs text-slate-400">Licence {{ doctor.license || 'not recorded' }}</p>
          </div>
        </div>
        <p class="text-sm text-slate-600 dark:text-slate-400 mt-4 flex-1">
          {{ doctor.bio || doctor.qualifications || 'No professional biography has been added yet.' }}
        </p>
        <div class="mt-5 flex gap-2">
          <router-link :to="`/dashboard/doctors/${doctor.id}`" class="btn-secondary flex-1 text-center">View profile</router-link>
          <router-link :to="`/dashboard/book-service?physicianId=${doctor.id}`" class="btn-primary flex-1 text-center">Book</router-link>
        </div>
      </article>
    </div>
  </div>
</template>

<script>
import { getPhysicians } from '@/api'

export default {
  name: 'DoctorDirectory',
  data() {
    return {
      doctors: [],
      search: '',
      loading: false,
      error: '',
    }
  },
  computed: {
    filtered() {
      const term = this.search.trim().toLowerCase()
      if (!term) return this.doctors
      return this.doctors.filter(d =>
        `${d.firstName} ${d.lastName}`.toLowerCase().includes(term)
        || (d.medicalSpecialization || '').toLowerCase().includes(term)
        || (d.bio || '').toLowerCase().includes(term)
        || (d.qualifications || '').toLowerCase().includes(term)
      )
    },
  },
  methods: {
    initials(doctor) {
      return `${(doctor.firstName || '?')[0]}${(doctor.lastName || '?')[0]}`.toUpperCase()
    },
    async load() {
      this.loading = true
      this.error = ''
      try {
        const data = await getPhysicians()
        this.doctors = Array.isArray(data) ? data : (data?.data ?? [])
      } catch (e) {
        this.error = e.message || 'Could not load the doctor directory.'
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
