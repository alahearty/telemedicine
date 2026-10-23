<template>
  <component :is="sidebarComponent" @close="sidebarToggle" />
</template>

<script>
import SidebarAdmin from './SidebarAdmin.vue'
import SidebarPatient from './SidebarPatient.vue'
import SidebarPhysician from './SidebarPhysician.vue'

export default {
  name: 'Sidebar',
  components: { SidebarAdmin, SidebarPatient, SidebarPhysician },
  computed: {
    sidebarComponent() {
      const path = this.$route?.path || ''
      if (path.startsWith('/physician')) return 'SidebarPhysician'
      const role = (localStorage.getItem('user-role') || '').toLowerCase()
      if (role.includes('admin') || role.includes('administrator')) return 'SidebarAdmin'
      return 'SidebarPatient'
    },
  },
  methods: {
    sidebarToggle() {
      document.querySelector('.flex-sidebar')?.classList.add('hidden')
    },
  },
}
</script>

