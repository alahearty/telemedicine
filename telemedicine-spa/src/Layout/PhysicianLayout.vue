<template>
  <div class="flex bg-slate-50 dark:bg-slate-900 font-sans pt-16 min-h-screen">
    <div
      id="sidebar-scroll"
      class="flex-sidebar lg:flex-auto w-sidebar lg:block hidden h-screen lg:z-0 z-40 overflow-auto lg:relative fixed"
    >
      <Sidebar />
    </div>
    <div class="flex-auto w-full overflow-auto min-h-screen" id="body-scroll">
      <Header />
      <slot />
      <Footer />
    </div>
  </div>
</template>

<script>
import Header from '@/components/account-dashboard/Header.vue'
import Footer from '@/components/account-dashboard/Footer.vue'
import Sidebar from '@/components/account-dashboard/Sidebar.vue'

export default {
  name: 'PhysicianLayout',
  components: { Sidebar, Header, Footer },
}
</script>
