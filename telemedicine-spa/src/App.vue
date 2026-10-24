<template>
  <div class="min-h-screen">
    <Notification />
    <router-view />
  </div>
</template>

<script>
import Notification from '@/components/common/Notification.vue'

export default {
  name: 'App',
  components: {
    Notification,
  },
}
</script>

<style>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 48;
}
</style>
