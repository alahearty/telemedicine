<template>
  <div class="flex flex-col items-center justify-center px-6 py-12 text-center">
    <div class="icon-tile mb-4 bg-slate-50 text-slate-400 dark:bg-white/[0.04] dark:text-slate-500">
      <Icon :icon="icon" class="text-xl" />
    </div>
    <h3 class="font-display text-base font-semibold text-slate-800 dark:text-slate-100">{{ title }}</h3>
    <p v-if="description" class="mt-1.5 max-w-sm text-sm leading-6 text-slate-500 dark:text-slate-400">
      {{ description }}
    </p>
    <div v-if="$slots.default" class="mt-4">
      <slot />
    </div>
  </div>
</template>

<script>
export default {
  name: 'EmptyState',
  props: {
    icon: { type: String, default: 'sparkles' },
    title: { type: String, required: true },
    description: { type: String, default: '' },
  },
}
</script>
