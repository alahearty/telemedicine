import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiTarget = env.VITE_API_TARGET || 'http://127.0.0.1:5231'

  return {
    plugins: [vue()],
    resolve: {
      alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) },
    },
    server: {
      host: '127.0.0.1',
      port: 8080,
      strictPort: true,
      watch: {
        ignored: ['**/test-results/**', '**/playwright-report/**', '**/artifacts/**'],
      },
      proxy: {
        '/api': { target: apiTarget, changeOrigin: true },
        '/chat-hub': { target: apiTarget, changeOrigin: true, ws: true },
        '/consultation-hub': { target: apiTarget, changeOrigin: true, ws: true },
      },
    },
    preview: {
      host: '127.0.0.1',
      port: 8080,
      strictPort: true,
      proxy: {
        '/api': { target: apiTarget, changeOrigin: true },
        '/chat-hub': { target: apiTarget, changeOrigin: true, ws: true },
        '/consultation-hub': { target: apiTarget, changeOrigin: true, ws: true },
      },
    },
    build: {
      sourcemap: false,
      chunkSizeWarningLimit: 600,
      rollupOptions: {
        output: {
          manualChunks: {
            vue: ['vue', 'vue-router', 'vuex'],
            charts: ['apexcharts', 'vue3-apexcharts'],
            primevue: ['primevue/config', 'primevue/dialog', 'primevue/breadcrumb'],
          },
        },
      },
    },
    test: {
      environment: 'jsdom',
      globals: true,
      include: ['src/**/*.spec.js'],
    },
  }
})
