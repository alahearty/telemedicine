<template>
  <header>
    <nav>
      <div class="logo">
        <TelemedicineLogo />
      </div>

      <ul class="nav-links">
        <li><a href="#departments">Departments</a></li>
        <li><a href="#about">About us</a></li>
        <li><a href="#doctors">Our Doctors</a></li>
        <li><a href="#service">Our Services</a></li>

        <li><button @click="pushToRegister">Register</button></li>
        <li><button @click="pushToLogin">Login</button></li>
      </ul>
    </nav>
  </header>
</template>

<script>
import TelemedicineLogo from '@/components/icons/TelemedicineHomeLogo.vue'

export default {
  name: 'Navbar',

  components: { TelemedicineLogo },

  methods: {
    pushToRegister() {
      this.$router.push('/signup')
    },
    pushToLogin() {
      this.$router.push('/login')
    },
  },
  mounted() {
    // window.addEventListener('scroll', () => {
    //   console.log('Scroll')
    // })
    console.log('Mounted')
  },
}
</script>

<style scoped>
header {
  position: relative;
  /* box-shadow: 0 1.5rem 2.8rem rgb(47 46 65 / 8%); */
  /* background-color: #fff; */
  position: fixed;
  width: 100%;
  z-index: 2;
  padding-inline: var(--bg-padding-inline);
}
/* header::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: -1;
  filter: blur(4px);
} */

nav {
  display: flex;
  justify-content: space-between;
  padding: 1rem 0;
  color: #1b71a1;
  align-items: center;
  max-width: 1080px;
  margin-inline: auto;
}

nav a {
  color: #283779;
  font-weight: 600;
  transition: font-weight 300ms ease;
}

nav a:hover {
  font-weight: 700;
}

nav svg {
  height: 30px;
}

.nav-links {
  display: flex;
  width: 720px;
  justify-content: space-between;
  align-items: center;
  gap: 0.1rem;
}

.nav-links button {
  font-weight: 600;
  border: 1px solid #283779;
  border-radius: 50px;
  height: 45px;
  width: 80px;
  transition: all 300ms ease;
  color: #283779;
}
.nav-links button:hover {
  background-color: #283779;
  color: #fff;
}

@media (max-width: 920px) {
  .nav-links {
    display: none;
  }
}
</style>
