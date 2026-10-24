<template>
  <div class="card-modern p-5 flex items-center gap-4">
    <div class="icon-wrap">
      <slot />
    </div>
    <div>
      <p class="font-bold text-slate-800 dark:text-slate-100 text-xl">
        {{ total }}
      </p>
      <p class="text-slate-500 dark:text-slate-400 text-sm mt-0.5">{{ totalPayout }}</p>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Card',
  props: {
    total: { type: String, required: true },
    totalPayout: { type: String, required: true },
  },
}
</script>

<style scoped>
@reference "../../assets/tailwind.css";
.icon-wrap {
  @apply flex-shrink-0 w-14 h-14 rounded-2xl flex items-center justify-center;
}
</style>
