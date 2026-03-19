<template>
  <AdminLayout>
    <router-view />
  </AdminLayout>
</template>

<script>
import AdminLayout from '@/Layout/AdminLayout.vue'
export default {
  components: { AdminLayout },
}
</script>
