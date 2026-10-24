<template>
  <aside
    class="fixed inset-y-0 left-0 z-50 flex w-[264px] flex-col border-r border-slate-200/80 bg-white/95 backdrop-blur-xl transition-transform duration-300 ease-out dark:border-white/[0.07] dark:bg-midnight-900/95 lg:translate-x-0"
    :class="open ? 'translate-x-0 shadow-2xl' : '-translate-x-full'"
    aria-label="Main navigation"
  >
    <div class="flex items-center justify-between px-5 pb-5 pt-6">
      <router-link :to="homeLink" class="group flex items-center gap-3">
        <span class="grid h-10 w-10 place-items-center rounded-[14px] bg-gradient-to-br from-teal-500 to-cyan-500 text-white shadow-glow-sm transition-transform group-hover:scale-105">
          <Icon icon="heart-pulse" class="text-xl" />
        </span>
        <span class="leading-tight">
          <span class="block font-display text-[17px] font-semibold tracking-[-0.02em] text-ink-900 dark:text-white">Telemedicine</span>
          <span class="block text-[10px] font-medium uppercase tracking-[0.16em] text-slate-400 dark:text-slate-500">Care workspace</span>
        </span>
      </router-link>
      <button class="btn-icon h-9 w-9 lg:hidden" aria-label="Close menu" @click="$emit('close')">
        <Icon icon="x" class="text-lg" />
      </button>
    </div>

    <div class="scrollbar-slim flex-1 overflow-y-auto px-3 pb-4">
      <div v-for="group in groups" :key="group.label" class="mb-5 last:mb-0">
        <p class="eyebrow px-3 pb-2">{{ group.label }}</p>
        <nav class="space-y-1">
          <router-link
            v-for="item in group.items"
            :key="item.to"
            :to="item.to"
            class="group relative flex items-center gap-3 rounded-xl px-3 py-2.5 text-[13px] font-medium transition-all duration-200"
            :class="itemActive(item)
              ? 'bg-gradient-to-r from-primary-50 to-cyan-50/60 text-primary-900 ring-1 ring-primary-100/80 dark:from-primary-400/[0.1] dark:to-cyan-400/[0.05] dark:text-primary-100 dark:ring-primary-400/15'
              : 'text-slate-600 hover:bg-slate-100/70 hover:text-slate-900 dark:text-slate-400 dark:hover:bg-white/[0.05] dark:hover:text-slate-100'"
            :aria-current="itemActive(item) ? 'page' : undefined"
            @click="$emit('close')"
          >
            <span
              class="flex h-8 w-8 items-center justify-center rounded-lg border transition-colors"
              :class="itemActive(item)
                ? 'border-primary-200/80 bg-white/80 text-primary-600 shadow-sm dark:border-primary-400/20 dark:bg-primary-400/10 dark:text-primary-300'
                : 'border-transparent bg-slate-100/80 text-slate-500 group-hover:bg-white group-hover:text-slate-700 dark:bg-white/[0.04] dark:text-slate-500 dark:group-hover:bg-white/[0.08] dark:group-hover:text-slate-300'"
            >
              <Icon :icon="item.icon" class="text-[17px]" />
            </span>
            <span class="flex-1">{{ item.label }}</span>
            <span v-if="isChatItem(item) && unreadCount" class="grid h-5 min-w-5 place-items-center rounded-full bg-primary-500 px-1.5 text-[10px] font-bold text-white">
              {{ unreadCount > 9 ? '9+' : unreadCount }}
            </span>
            <span v-else-if="itemActive(item)" class="h-1.5 w-1.5 rounded-full bg-primary-500 shadow-[0_0_0_3px_rgba(16,185,154,0.15)]" />
          </router-link>
        </nav>
      </div>

    </div>

    <div class="border-t border-slate-200/80 px-3 py-3 dark:border-white/[0.07]">
      <router-link
        :to="profileLink"
        class="flex items-center gap-3 rounded-xl p-2 transition-colors hover:bg-slate-100/70 dark:hover:bg-white/[0.05]"
        @click="$emit('close')"
      >
        <AppAvatar :name="displayName" size="sm" />
        <div class="min-w-0 flex-1">
          <p class="truncate text-[13px] font-semibold text-ink-900 dark:text-slate-100">{{ displayName }}</p>
          <p class="text-[10px] font-semibold uppercase tracking-[0.12em] text-slate-400 dark:text-slate-500">{{ roleLabel }}</p>
        </div>
        <Icon icon="chevron-right" class="text-sm text-slate-400" />
      </router-link>
      <button
        type="button"
        class="mt-1 flex w-full items-center gap-2.5 rounded-xl px-2 py-2 text-xs font-medium text-slate-500 transition-colors hover:bg-rose-50 hover:text-rose-600 dark:text-slate-400 dark:hover:bg-rose-400/10 dark:hover:text-rose-300"
        @click="signOut"
      >
        <Icon icon="log-out" class="text-sm" />
        Sign out
      </button>
    </div>
  </aside>
</template>

<script>
import AppAvatar from '@/components/ui/AppAvatar.vue'
import { getChatThreads } from '@/api'
import { clearSession, useSession } from '@/composables/useSession'
import { groupsFor, isItemActive, roleKey } from '@/config/navigation'

export default {
  name: 'AppSidebar',
  components: { AppAvatar },
  props: {
    open: { type: Boolean, default: false },
  },
  emits: ['close'],
  setup() {
    return { session: useSession() }
  },
  data() {
    return { unreadCount: 0, unreadTimer: null }
  },
  computed: {
    role() {
      return roleKey(this.$route.path, this.session.role)
    },
    groups() {
      return groupsFor(this.$route.path, this.session.role)
    },
    roleLabel() {
      return { patient: 'Patient', admin: 'Administrator', physician: 'Physician' }[this.role]
    },
    displayName() {
      if (this.session.name) return this.session.name
      const local = (this.session.email || '').split('@')[0]
      if (!local) return this.roleLabel
      return local.charAt(0).toUpperCase() + local.slice(1)
    },
    homeLink() {
      return this.role === 'physician' ? '/physician' : '/dashboard'
    },
    profileLink() {
      return this.role === 'physician' ? '/physician/profile' : '/dashboard/account'
    },
  },
  methods: {
    itemActive(item) {
      return isItemActive(item, this.$route.path)
    },
    isChatItem(item) {
      return item.to === '/dashboard/chat' || item.to === '/physician/chat'
    },
    async refreshUnread() {
      if (this.role === 'admin') return
      try {
        const data = await getChatThreads()
        const threads = Array.isArray(data) ? data : (data?.data ?? [])
        this.unreadCount = threads.reduce((sum, thread) => sum + Number(thread.unreadCount || 0), 0)
      } catch {
        this.unreadCount = 0
      }
    },
    signOut() {
      clearSession()
      this.$router.push('/login')
    },
  },
  watch: {
    '$route.fullPath'() {
      this.refreshUnread()
    },
  },
  mounted() {
    this.refreshUnread()
    this.unreadTimer = setInterval(this.refreshUnread, 45000)
    window.addEventListener('telemedicine:chat-read', this.refreshUnread)
  },
  beforeUnmount() {
    if (this.unreadTimer) clearInterval(this.unreadTimer)
    window.removeEventListener('telemedicine:chat-read', this.refreshUnread)
  },
}
</script>
