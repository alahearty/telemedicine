<template>
  <!-- <AppLayout /> -->
  <router-view />
</template>

<script>
// vue Components
import AppLayout from '@/components/layout/AppLayout.vue'
import AuthLayout from '@/components/layout/AuthLayout.vue'
// npm-js
import Scrollbar from 'smooth-scrollbar'

export default {
  name: 'App',
  components: {
    AppLayout,
    AuthLayout,
  },
  mounted() {
    Scrollbar.init(document.querySelector('#body-scroll'))

    setTimeout(() => {
      var alert_dis = document.querySelectorAll('.alert-dismiss')
      alert_dis.forEach((x) =>
        x.addEventListener('click', function () {
          x.parentElement.classList.add('hidden')
        })
      )
    }, 100)
  },
}
</script>

<style>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 48;
}
* {
  scroll-behavior: smooth;
}
</style>
