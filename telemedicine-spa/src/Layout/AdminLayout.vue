<template>
  <!-- App -->
  <div class="flex bg-packed font-lexend dark:bg-gray-900 pt-16">
    <div
      id="sidebar-scroll"
      class="flex-sidebar lg:flex-auto w-sidebar lg:block hidden bg-white dark:bg-gray-800 border-r-2 dark:border-gray-700 h-screen lg:z-0 z-40 overflow-auto lg:relative fixed"
    >
      <Sidebar />
    </div>
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
  name: 'Layout',
  components: {
    Sidebar,
    Header,
    Footer,
  },
}
</script>

<style></style>
