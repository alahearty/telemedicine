<template>
  <PhysicianLayout>
    <router-view />
  </PhysicianLayout>
</template>

<script>
import PhysicianLayout from '@/Layout/PhysicianLayout.vue'

export default {
  name: 'PhysicianDashboardLayout',
  components: { PhysicianLayout },
}
</script>
