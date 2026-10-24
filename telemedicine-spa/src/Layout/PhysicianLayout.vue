<template>
  <AppShell>
    <slot />
  </AppShell>
</template>

<script>
import AppShell from '@/components/layout/AppShell.vue'

export default {
  name: 'PhysicianLayout',
  components: { AppShell },
}
</script>
