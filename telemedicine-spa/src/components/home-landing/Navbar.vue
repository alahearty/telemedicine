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
      </ul>

      <ul class="nav-auth">
        <li><button @click="pushToRegister">Register</button></li>
        <li><button class="login-btn" @click="pushToLogin">Login</button></li>
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
  position: absolute;
  width: 100%;
  z-index: 2;
  padding-inline: var(--bg-padding-inline);
}
nav {
  display: flex;
  padding: 1rem 0;
  color: #1b71a1;
  align-items: center;
  max-width: var(--max-width);
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
  width: 450px;
  justify-content: space-between;
  align-items: center;
  gap: 0.1rem;
  margin-left: auto;
}

.nav-auth {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-left: 2rem;
}
.nav-auth button {
  font-weight: 600;
  border: 1px solid #283779;
  border-radius: 50px;
  /* height: 45px; */
  padding: 0.4rem 1rem;
  transition: all 300ms ease;
  color: #283779;
}
.nav-auth button:hover {
  background-color: #283779;
  color: #fff;
}

.nav-auth button.login-btn {
  background-color: #283779;
  color: #fff;
}

@media (max-width: 970px) {
  .nav-links {
    display: none;
  }
  .nav-auth {
    /* display: none; */
    margin-left: auto;
  }
}

@media (max-width: 450px) {
  .nav-auth {
    display: none;
  }
}
</style>
