<template>
  <Teleport to="body">
    <Transition name="fade">
      <div v-if="open" class="fixed inset-0 z-[90] flex items-start justify-center px-4 pt-[12vh]">
        <div class="absolute inset-0 bg-slate-950/55 backdrop-blur-sm" @click="close" />
        <div
          class="relative w-full max-w-xl overflow-hidden rounded-[22px] border border-white/10 bg-white shadow-2xl dark:bg-midnight-850"
          role="dialog"
          aria-modal="true"
          aria-label="Quick navigation"
        >
          <div class="flex items-center gap-3 border-b border-slate-100 px-4 dark:border-white/[0.07]">
            <Icon icon="search" class="text-lg text-slate-400" />
            <input
              ref="input"
              v-model="query"
              type="text"
              class="w-full bg-transparent py-4 text-sm text-ink-900 outline-none placeholder:text-slate-400 dark:text-white dark:placeholder:text-slate-500"
              placeholder="Search pages and actions..."
              aria-label="Search pages and actions"
              @keydown.down.prevent="move(1)"
              @keydown.up.prevent="move(-1)"
              @keydown.enter.prevent="go(activeIndex)"
              @keydown.esc.prevent="close"
            />
            <kbd class="hidden rounded-md border border-slate-200 bg-slate-50 px-2 py-1 text-[10px] font-semibold text-slate-400 dark:border-white/[0.08] dark:bg-white/[0.04] dark:text-slate-500 sm:block">ESC</kbd>
          </div>

          <ul v-if="results.length" class="max-h-[52vh] overflow-y-auto p-2 scrollbar-slim">
            <li v-for="(item, index) in results" :key="`${item.group}-${item.to}`">
              <button
                type="button"
                class="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-left transition-colors"
                :class="index === activeIndex
                  ? 'bg-primary-50 text-primary-900 dark:bg-primary-400/10 dark:text-primary-100'
                  : 'text-slate-600 hover:bg-slate-50 dark:text-slate-300 dark:hover:bg-white/[0.04]'"
                @mouseenter="activeIndex = index"
                @click="go(index)"
              >
                <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-slate-100 text-slate-500 dark:bg-white/[0.06] dark:text-slate-400">
                  <Icon :icon="item.icon" class="text-base" />
                </span>
                <span class="min-w-0 flex-1">
                  <span class="block text-sm font-medium">{{ item.label }}</span>
                  <span class="block text-[11px] text-slate-400 dark:text-slate-500">{{ item.group }}</span>
                </span>
                <Icon icon="arrow-right" class="text-sm text-slate-300 dark:text-slate-600" />
              </button>
            </li>
          </ul>
          <div v-else class="px-4 py-10 text-center text-sm text-slate-500 dark:text-slate-400">
            No matching pages.
          </div>
          <div class="flex items-center justify-between border-t border-slate-100 px-4 py-2.5 text-[10px] text-slate-400 dark:border-white/[0.07] dark:text-slate-500">
            <span>Navigate with arrows, select with Enter</span>
            <span class="hidden sm:block">Telemedicine quick switch</span>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script>
export default {
  name: 'CommandPalette',
  props: {
    open: { type: Boolean, default: false },
    groups: { type: Array, default: () => [] },
  },
  emits: ['update:open'],
  data() {
    return { query: '', activeIndex: 0 }
  },
  computed: {
    allItems() {
      return this.groups.flatMap((group) =>
        group.items.map((item) => ({ ...item, group: group.label }))
      )
    },
    results() {
      const term = this.query.trim().toLowerCase()
      if (!term) return this.allItems
      return this.allItems.filter((item) =>
        `${item.label} ${item.group}`.toLowerCase().includes(term)
      )
    },
  },
  watch: {
    open(value) {
      if (value) {
        this.query = ''
        this.activeIndex = 0
        this.$nextTick(() => this.$refs.input?.focus())
      }
    },
    query() {
      this.activeIndex = 0
    },
  },
  mounted() {
    window.addEventListener('keydown', this.handleShortcut)
  },
  beforeUnmount() {
    window.removeEventListener('keydown', this.handleShortcut)
  },
  methods: {
    handleShortcut(event) {
      if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
        event.preventDefault()
        this.$emit('update:open', !this.open)
      }
      if (event.key === 'Escape' && this.open) this.close()
    },
    close() {
      this.$emit('update:open', false)
    },
    move(delta) {
      if (!this.results.length) return
      this.activeIndex = (this.activeIndex + delta + this.results.length) % this.results.length
    },
    go(index) {
      const item = this.results[index]
      if (!item) return
      this.close()
      if (this.$route.path !== item.to) this.$router.push(item.to)
    },
  },
}
</script>
