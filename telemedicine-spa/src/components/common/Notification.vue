<template>
  <transition-group name="notification" tag="div" class="notification-container">
    <div
      v-for="notification in notifications"
      :key="notification.id"
      :class="['notification', `notification-${notification.type}`]"
    >
      <div class="notification-content">
        <div class="notification-icon">
          <svg v-if="notification.type === 'success'" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <svg v-else-if="notification.type === 'error'" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <svg v-else-if="notification.type === 'warning'" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <div class="notification-text">
          <p class="notification-title">{{ notification.title }}</p>
          <p v-if="notification.message" class="notification-message">{{ notification.message }}</p>
        </div>
        <button class="notification-close" @click="removeNotification(notification.id)">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
      <div v-if="notification.autoClose" class="notification-progress" :style="{ animationDuration: `${notification.duration}ms` }"></div>
    </div>
  </transition-group>
</template>

<script>
import { getCurrentInstance } from 'vue'

export default {
  name: 'Notification',
  data() {
    return {
      notifications: []
    }
  },
  mounted() {
    const api = {
      show: this.show.bind(this),
      success: (title, message) => this.show(title, message, 'success'),
      error: (title, message) => this.show(title, message, 'error'),
      warning: (title, message) => this.show(title, message, 'warning'),
      info: (title, message) => this.show(title, message, 'info')
    }
    const instance = getCurrentInstance()
    if (instance?.appContext?.app) {
      instance.appContext.app.config.globalProperties.$notification = api
    }
    this.$root.$notification = api
  },
  methods: {
    show(title, message = '', type = 'info', duration = 5000) {
      const id = Date.now() + Math.random()
      const notification = {
        id,
        title,
        message,
        type,
        duration,
        autoClose: duration > 0
      }
      this.notifications.push(notification)
      
      if (duration > 0) {
        setTimeout(() => {
          this.removeNotification(id)
        }, duration)
      }
    },
    removeNotification(id) {
      const index = this.notifications.findIndex(n => n.id === id)
      if (index > -1) {
        this.notifications.splice(index, 1)
      }
    }
  }
}
</script>

<style scoped>
.notification-container {
  @apply fixed top-4 right-4 z-50 space-y-2 max-w-md;
}

.notification {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-lg border-l-4 overflow-hidden;
  animation: slideIn 0.3s ease-out;
}

.notification-success {
  @apply border-green-500;
}

.notification-error {
  @apply border-red-500;
}

.notification-warning {
  @apply border-yellow-500;
}

.notification-info {
  @apply border-blue-500;
}

.notification-content {
  @apply flex items-start p-4;
}

.notification-icon {
  @apply flex-shrink-0 mr-3;
}

.notification-success .notification-icon {
  @apply text-green-500;
}

.notification-error .notification-icon {
  @apply text-red-500;
}

.notification-warning .notification-icon {
  @apply text-yellow-500;
}

.notification-info .notification-icon {
  @apply text-blue-500;
}

.notification-text {
  @apply flex-1 min-w-0;
}

.notification-title {
  @apply font-semibold text-gray-800 dark:text-gray-200;
}

.notification-message {
  @apply text-sm text-gray-600 dark:text-gray-400 mt-1;
}

.notification-close {
  @apply ml-3 flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors;
}

.notification-progress {
  @apply h-1 bg-current opacity-30;
  animation: progress linear;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes progress {
  from {
    width: 100%;
  }
  to {
    width: 0;
  }
}

.notification-leave-active {
  animation: slideOut 0.3s ease-in;
}

@keyframes slideOut {
  from {
    transform: translateX(0);
    opacity: 1;
  }
  to {
    transform: translateX(100%);
    opacity: 0;
  }
}
</style>

