<template>
  <header class="sticky top-0 z-30 border-b border-slate-200/70 bg-white/75 backdrop-blur-xl dark:border-white/[0.06] dark:bg-midnight-950/70">
    <div class="flex h-[72px] items-center gap-3 px-4 sm:px-6 lg:px-8">
      <button class="btn-icon h-10 w-10 lg:hidden" aria-label="Open menu" @click="$emit('toggle-sidebar')">
        <Icon icon="menu" class="text-lg" />
      </button>

      <div class="hidden min-w-0 sm:block">
        <p class="eyebrow">Workspace</p>
        <p class="truncate font-display text-[13px] font-semibold text-ink-900 dark:text-slate-100">{{ pageLabel }}</p>
      </div>

      <div class="ml-auto flex items-center gap-2">
        <button
          type="button"
          class="hidden items-center gap-2 rounded-xl border border-slate-200/90 bg-white/70 px-3 py-2 text-xs font-medium text-slate-500 shadow-sm transition-colors hover:border-primary-200 hover:text-primary-700 dark:border-white/[0.08] dark:bg-white/[0.03] dark:text-slate-400 dark:hover:border-primary-400/25 dark:hover:text-primary-300 sm:flex"
          aria-label="Open quick search"
          @click="paletteOpen = true"
        >
          <Icon icon="search" class="text-sm" />
          <span class="pr-6">Search</span>
          <kbd class="rounded-md border border-slate-200 bg-slate-50 px-1.5 py-0.5 text-[10px] font-semibold text-slate-400 dark:border-white/[0.08] dark:bg-white/[0.05] dark:text-slate-500">
            {{ shortcutLabel }}
          </kbd>
        </button>
        <button class="btn-icon sm:hidden" aria-label="Open quick search" @click="paletteOpen = true">
          <Icon icon="search" class="text-lg" />
        </button>

        <button
          type="button"
          class="btn-icon"
          :aria-label="isDark ? 'Switch to light mode' : 'Switch to dark mode'"
          @click="toggleTheme"
        >
          <Icon :icon="isDark ? 'sun' : 'moon'" class="text-lg" />
        </button>

        <div class="relative" @click.stop>
          <button
            type="button"
            class="flex items-center gap-2 rounded-xl p-1 pr-2 transition-colors hover:bg-slate-100/80 dark:hover:bg-white/[0.06]"
            :aria-expanded="menuOpen"
            aria-haspopup="menu"
            @click="menuOpen = !menuOpen"
          >
            <AppAvatar :name="displayName" size="sm" />
            <span class="hidden text-left md:block">
              <span class="block max-w-[130px] truncate text-xs font-semibold text-ink-900 dark:text-slate-100">{{ displayName }}</span>
              <span class="block text-[10px] font-medium uppercase tracking-[0.12em] text-slate-400 dark:text-slate-500">{{ roleLabel }}</span>
            </span>
            <Icon icon="chevron-down" class="text-xs text-slate-400" />
          </button>

          <Transition name="fade">
            <div
              v-if="menuOpen"
              class="absolute right-0 mt-2 w-60 overflow-hidden rounded-2xl border border-slate-200/90 bg-white p-2 shadow-2xl dark:border-white/[0.08] dark:bg-midnight-850"
              role="menu"
            >
              <div class="flex items-center gap-3 rounded-xl bg-slate-50/80 p-2.5 dark:bg-white/[0.04]">
                <AppAvatar :name="displayName" size="md" />
                <div class="min-w-0">
                  <p class="truncate text-sm font-semibold text-ink-900 dark:text-white">{{ displayName }}</p>
                  <p class="truncate text-[11px] text-slate-500 dark:text-slate-400">{{ email || roleLabel }}</p>
                </div>
              </div>
              <div class="mt-1.5 space-y-0.5">
                <router-link
                  :to="profileLink"
                  class="flex items-center gap-2.5 rounded-lg px-2.5 py-2 text-xs font-medium text-slate-600 transition-colors hover:bg-slate-100 hover:text-slate-900 dark:text-slate-300 dark:hover:bg-white/[0.06] dark:hover:text-white"
                  role="menuitem"
                  @click="menuOpen = false"
                >
                  <Icon :icon="isPhysician ? 'user' : 'settings'" class="text-sm" />
                  {{ isPhysician ? 'Physician profile' : 'Account settings' }}
                </router-link>
                <router-link
                  :to="isPhysician ? '/physician' : '/dashboard'"
                  class="flex items-center gap-2.5 rounded-lg px-2.5 py-2 text-xs font-medium text-slate-600 transition-colors hover:bg-slate-100 hover:text-slate-900 dark:text-slate-300 dark:hover:bg-white/[0.06] dark:hover:text-white"
                  role="menuitem"
                  @click="menuOpen = false"
                >
                  <Icon icon="dashboard" class="text-sm" />
                  Dashboard
                </router-link>
              </div>
              <div class="mt-1 border-t border-slate-100 pt-1 dark:border-white/[0.07]">
                <button
                  type="button"
                  class="flex w-full items-center gap-2.5 rounded-lg px-2.5 py-2 text-xs font-medium text-rose-600 transition-colors hover:bg-rose-50 dark:text-rose-300 dark:hover:bg-rose-400/10"
                  role="menuitem"
                  @click="signOut"
                >
                  <Icon icon="log-out" class="text-sm" />
                  Sign out
                </button>
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </div>

    <CommandPalette v-model:open="paletteOpen" :groups="groups" />
  </header>
</template>

<script>
import AppAvatar from '@/components/ui/AppAvatar.vue'
import CommandPalette from '@/components/layout/CommandPalette.vue'
import { useTheme } from '@/composables/useTheme'
import { clearSession, useSession } from '@/composables/useSession'
import { findPageLabel, groupsFor, roleKey } from '@/config/navigation'

export default {
  name: 'AppHeader',
  components: { AppAvatar, CommandPalette },
  emits: ['toggle-sidebar'],
  setup() {
    const { isDark, setTheme } = useTheme()
    return { isDark, setTheme, session: useSession() }
  },
  data() {
    return { menuOpen: false, paletteOpen: false }
  },
  computed: {
    role() {
      return roleKey(this.$route.path, this.session.role)
    },
    groups() {
      return groupsFor(this.$route.path, this.session.role)
    },
    pageLabel() {
      return findPageLabel(this.$route.path, this.groups)
    },
    roleLabel() {
      return { patient: 'Patient', admin: 'Administrator', physician: 'Physician' }[this.role]
    },
    isPhysician() {
      return this.role === 'physician'
    },
    displayName() {
      if (this.session.name) return this.session.name
      const local = (this.session.email || '').split('@')[0]
      if (!local) return this.roleLabel
      return local.charAt(0).toUpperCase() + local.slice(1)
    },
    email() {
      return this.session.email || ''
    },
    profileLink() {
      return this.isPhysician ? '/physician/profile' : '/dashboard/account'
    },
    shortcutLabel() {
      return /Mac|iPhone|iPad/i.test(navigator.platform || '') ? 'Cmd K' : 'Ctrl K'
    },
  },
  mounted() {
    document.addEventListener('click', this.closeMenu)
    document.addEventListener('keydown', this.handleEscape)
  },
  beforeUnmount() {
    document.removeEventListener('click', this.closeMenu)
    document.removeEventListener('keydown', this.handleEscape)
  },
  methods: {
    closeMenu() {
      this.menuOpen = false
    },
    handleEscape(event) {
      if (event.key === 'Escape') this.menuOpen = false
    },
    toggleTheme() {
      this.setTheme(this.isDark ? 'light' : 'dark')
    },
    signOut() {
      this.menuOpen = false
      clearSession()
      this.$router.push('/login')
    },
  },
}
</script>
