<template>
  <div class="min-h-dvh">
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:fixed focus:left-4 focus:top-4 focus:z-[100] focus:rounded-xl focus:bg-white focus:px-4 focus:py-2 focus:text-sm focus:font-semibold focus:text-primary-700 focus:shadow-xl dark:focus:bg-midnight-850 dark:focus:text-primary-300"
    >
      Skip to content
    </a>

    <AppSidebar :open="sidebarOpen" @close="sidebarOpen = false" />

    <Transition name="fade">
      <div
        v-if="sidebarOpen"
        class="fixed inset-0 z-40 bg-slate-950/45 backdrop-blur-sm lg:hidden"
        aria-hidden="true"
        @click="sidebarOpen = false"
      />
    </Transition>

    <div class="lg:pl-[264px]">
      <AppHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />

      <main id="main-content" class="mx-auto w-full max-w-[1400px] px-4 pb-10 pt-6 sm:px-6 lg:px-8 lg:pt-8">
        <slot />
      </main>

      <footer class="border-t border-slate-200/70 px-4 py-5 dark:border-white/[0.06] sm:px-6 lg:px-8">
        <div class="mx-auto flex max-w-[1400px] flex-col gap-2 text-[11px] text-slate-400 dark:text-slate-500 sm:flex-row sm:items-center sm:justify-between">
          <p>&copy; {{ year }} Telemedicine Platform</p>
          <p class="flex items-center gap-2">
            <span class="h-1.5 w-1.5 rounded-full bg-emerald-500" />
            Local demo &middot; synthetic data only
          </p>
        </div>
      </footer>
    </div>
  </div>
</template>

<script>
import AppSidebar from '@/components/layout/AppSidebar.vue'
import AppHeader from '@/components/layout/AppHeader.vue'

export default {
  name: 'AppShell',
  components: { AppSidebar, AppHeader },
  data() {
    return { sidebarOpen: false, year: new Date().getFullYear() }
  },
  watch: {
    sidebarOpen(value) {
      document.body.style.overflow = value ? 'hidden' : ''
    },
    $route() {
      this.sidebarOpen = false
    },
  },
  beforeUnmount() {
    document.body.style.overflow = ''
  },
}
</script>
