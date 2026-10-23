<template>
  <div>
    <Notification />
    <router-view/>
  </div>
</template>

<script>
import Scrollbar from 'smooth-scrollbar'
import Notification from '@/components/common/Notification.vue'

export default {
  name: 'App',
  components: {
    Notification
  },

  data(){
    return{
      layout:`div`
    }
  },

  mounted() {
    this.initScrollbar()
    setTimeout(() => {
      var alert_dis = document.querySelectorAll('.alert-dismiss')
      alert_dis.forEach((x) =>
        x.addEventListener('click', function () {
          x.parentElement.classList.add('hidden')
        })
      )
    }, 100)
  },

  watch: {
    $route() {
      this.$nextTick(() => this.initScrollbar())
    },
  },

  methods: {
    initScrollbar() {
      const el = document.querySelector('#body-scroll')
      if (el) {
        const existing = Scrollbar.get(el)
        if (existing) existing.destroy()
        Scrollbar.init(el)
      }
    },
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
