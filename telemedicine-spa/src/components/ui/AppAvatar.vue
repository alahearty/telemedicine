<template>
  <span
    class="inline-flex shrink-0 items-center justify-center rounded-full font-semibold uppercase tracking-wide text-white ring-2 ring-white/70 dark:ring-white/10"
    :class="sizeClass"
    :style="{ backgroundImage: gradient }"
    :aria-label="name || 'User avatar'"
    role="img"
  >
    {{ initials }}
  </span>
</template>

<script>
const GRADIENTS = [
  'linear-gradient(135deg, #0d9488, #0891b2)',
  'linear-gradient(135deg, #0ea5e9, #6366f1)',
  'linear-gradient(135deg, #14b8a6, #34d399)',
  'linear-gradient(135deg, #8b5cf6, #ec4899)',
  'linear-gradient(135deg, #f59e0b, #f97316)',
  'linear-gradient(135deg, #06b6d4, #3b82f6)',
]

export default {
  name: 'AppAvatar',
  props: {
    name: { type: String, default: '' },
    size: { type: String, default: 'md' },
  },
  computed: {
    initials() {
      const parts = (this.name || '').trim().split(/\s+/).filter(Boolean)
      if (!parts.length) return 'U'
      if (parts.length === 1) return parts[0].slice(0, 2)
      return `${parts[0][0]}${parts[parts.length - 1][0]}`
    },
    sizeClass() {
      return {
        sm: 'h-8 w-8 text-[11px]',
        md: 'h-10 w-10 text-xs',
        lg: 'h-14 w-14 text-base',
        xl: 'h-20 w-20 text-2xl',
      }[this.size] || 'h-10 w-10 text-xs'
    },
    gradient() {
      const seed = (this.name || 'u').split('').reduce((sum, char) => sum + char.charCodeAt(0), 0)
      return GRADIENTS[seed % GRADIENTS.length]
    },
  },
}
</script>
