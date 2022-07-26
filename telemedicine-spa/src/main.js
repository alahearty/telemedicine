import { createApp } from 'vue'
import App from './App.vue'
import store from './store'
import router from './router'
import { Icon } from '@iconify/vue'
import VueApexCharts from 'vue3-apexcharts'
import PerfectScrollbar from 'vue3-perfect-scrollbar'
import PrimeVue from 'primevue/config'
import Breadcrumb from 'primevue/breadcrumb'
import Dialog from 'primevue/dialog'

import 'primevue/resources/themes/saga-blue/theme.css' // theme
import 'primevue/resources/primevue.min.css' // core
import 'primeicons/primeicons.css' // icons

import 'vue3-perfect-scrollbar/dist/vue3-perfect-scrollbar.css'
import './assets/tailwind.css'
import './assets/animate.css'
import './assets/sass/css/style.css'

const app = createApp(App)
app.use(store)
app.use(router, Icon)
app.use(VueApexCharts)
app.use(PrimeVue)
app.component('Dialog', Dialog)
app.component('Breadcrumb', Breadcrumb)
app.use(PerfectScrollbar)
app.mount('#app')

router.beforeEach((to, from, next) => {
  document.querySelector('.flex-sidebar').classList.add('hidden')
  next()
})
