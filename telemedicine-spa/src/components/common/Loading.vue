<template>
  <div v-if="show" class="loading-overlay">
    <div class="loading-spinner">
      <div class="spinner-ring"></div>
      <div class="spinner-ring"></div>
      <div class="spinner-ring"></div>
      <p v-if="message" class="loading-message">{{ message }}</p>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Loading',
  props: {
    show: {
      type: Boolean,
      default: false
    },
    message: {
      type: String,
      default: ''
    }
  }
}
</script>

<style scoped>
.loading-overlay {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50;
}

.loading-spinner {
  @apply flex flex-col items-center;
}

.spinner-ring {
  @apply border-4 border-indigo-200 border-t-indigo-500 rounded-full;
  width: 50px;
  height: 50px;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

.spinner-ring:nth-child(2) {
  width: 40px;
  height: 40px;
  animation-delay: 0.1s;
  border-color: transparent transparent #6366f1 #6366f1;
}

.spinner-ring:nth-child(3) {
  width: 30px;
  height: 30px;
  animation-delay: 0.2s;
  border-color: #818cf8 transparent transparent transparent;
}

.loading-message {
  @apply text-white font-semibold text-lg mt-4;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>

