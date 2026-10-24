<template>
  <Teleport to="body">
    <Transition name="fade">
      <div v-if="open" class="fixed inset-0 z-[80] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-slate-950/50 backdrop-blur-sm" @click="$emit('cancel')" />
        <div
          class="relative w-full max-w-md rounded-[22px] border border-white/10 bg-white p-6 shadow-2xl dark:bg-midnight-850"
          role="dialog"
          aria-modal="true"
          :aria-labelledby="titleId"
        >
          <div class="flex items-start gap-4">
            <div
              class="icon-tile shrink-0"
              :class="danger ? 'bg-rose-50 text-rose-600 dark:bg-rose-400/10 dark:text-rose-300' : 'bg-amber-50 text-amber-600 dark:bg-amber-400/10 dark:text-amber-300'"
            >
              <Icon :icon="danger ? 'alert-circle' : 'info'" class="text-xl" />
            </div>
            <div class="min-w-0">
              <h2 :id="titleId" class="font-display text-lg font-semibold text-ink-900 dark:text-white">
                {{ title }}
              </h2>
              <p v-if="message" class="mt-1.5 text-sm leading-6 text-slate-500 dark:text-slate-400">
                {{ message }}
              </p>
            </div>
          </div>
          <div class="mt-6 flex justify-end gap-2">
            <button type="button" class="btn-secondary" @click="$emit('cancel')">{{ cancelLabel }}</button>
            <button type="button" :class="danger ? 'btn-danger' : 'btn-primary'" @click="$emit('confirm')">
              {{ confirmLabel }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script>
export default {
  name: 'ConfirmDialog',
  props: {
    open: { type: Boolean, default: false },
    title: { type: String, default: 'Are you sure?' },
    message: { type: String, default: '' },
    confirmLabel: { type: String, default: 'Confirm' },
    cancelLabel: { type: String, default: 'Cancel' },
    danger: { type: Boolean, default: false },
  },
  emits: ['confirm', 'cancel'],
  data() {
    return { titleId: `confirm-title-${Math.random().toString(36).slice(2, 8)}` }
  },
  watch: {
    open(value) {
      document.body.style.overflow = value ? 'hidden' : ''
    },
  },
  mounted() {
    document.addEventListener('keydown', this.handleKeydown)
  },
  beforeUnmount() {
    document.body.style.overflow = ''
    document.removeEventListener('keydown', this.handleKeydown)
  },
  methods: {
    handleKeydown(event) {
      if (event.key === 'Escape' && this.open) this.$emit('cancel')
    },
  },
}
</script>
