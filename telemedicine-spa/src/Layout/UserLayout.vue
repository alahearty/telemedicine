<template>
  <!-- App -->
  <div class="flex bg-packed font-lexend dark:bg-gray-900 pt-16">
    <div class="flex-auto w-full overflow-auto h-screen" id="body-scroll">
      <Header />
      <slot />
      <Footer />
    </div>
  </div>
  <!-- end app -->
</template>

<script>
import Header from '@/components/account-dashboard/Header.vue'
import Footer from '@/components/account-dashboard/Footer.vue'
import Sidebar from '@/components/account-dashboard/Sidebar.vue'

export default {
  name: 'UserLayout',
  components: {
    Sidebar,
    Header,
    Footer,
  },
}
</script>
