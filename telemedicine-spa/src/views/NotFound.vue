<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-slate-50 dark:bg-slate-900 p-6">
    <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100 mb-2">Page not found</h1>
    <p class="text-slate-600 dark:text-slate-400 mb-6">The page you're looking for doesn't exist.</p>
    <router-link to="/" class="btn-primary">Back to home</router-link>
  </div>
</template>

<script>
export default { name: 'NotFound' }
</script>
