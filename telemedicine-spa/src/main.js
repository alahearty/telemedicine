import { createApp } from 'vue'
import App from './App.vue'
import store from './store/admin'
import router from './router'
import Icon from '@/components/LocalIcon.vue'
import VueApexCharts from 'vue3-apexcharts'
import { initTheme } from '@/composables/useTheme'

import '@fontsource/sora/latin-500.css'
import '@fontsource/sora/latin-600.css'
import '@fontsource/sora/latin-700.css'
import '@fontsource/dm-sans/latin-400.css'
import '@fontsource/dm-sans/latin-500.css'
import '@fontsource/dm-sans/latin-600.css'
import '@fontsource/dm-sans/latin-700.css'

import './assets/tailwind.css'
import './assets/animate.css'

initTheme()

const app = createApp(App)
app.use(store)
app.use(router)
app.component('Icon', Icon)
app.use(VueApexCharts)
app.mount('#app')
