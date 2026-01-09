<template>
  <header :class="['navbar', { 'navbar-scrolled': isScrolled }]">
    <nav class="nav-container">
      <div class="logo">
        <TelemedicineLogo />
      </div>

      <button class="mobile-menu-toggle" @click="toggleMobileMenu" aria-label="Toggle menu">
        <svg v-if="!mobileMenuOpen" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
        </svg>
        <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>

      <ul :class="['nav-links', { 'nav-links-open': mobileMenuOpen }]">
        <li><a href="#departments" @click="closeMobileMenu">Departments</a></li>
        <li><a href="#about" @click="closeMobileMenu">About us</a></li>
        <li><a href="#doctors" @click="closeMobileMenu">Our Doctors</a></li>
        <li><a href="#service" @click="closeMobileMenu">Our Services</a></li>
      </ul>

      <ul :class="['nav-auth', { 'nav-auth-open': mobileMenuOpen }]">
        <li><button class="register-btn" @click="pushToRegister">Register</button></li>
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

  data() {
    return {
      isScrolled: false,
      mobileMenuOpen: false
    }
  },

  methods: {
    pushToRegister() {
      this.$router.push('/signup')
      this.closeMobileMenu()
    },
    pushToLogin() {
      this.$router.push('/login')
      this.closeMobileMenu()
    },
    toggleMobileMenu() {
      this.mobileMenuOpen = !this.mobileMenuOpen
    },
    closeMobileMenu() {
      this.mobileMenuOpen = false
    },
    handleScroll() {
      this.isScrolled = window.scrollY > 50
    }
  },
  mounted() {
    window.addEventListener('scroll', this.handleScroll)
    this.handleScroll()
  },
  beforeUnmount() {
    window.removeEventListener('scroll', this.handleScroll)
  },
}
</script>

<style scoped>
.navbar {
  position: fixed;
  top: 0;
  width: 100%;
  z-index: 1000;
  padding-inline: var(--bg-padding-inline);
  transition: all 0.3s ease;
  background: transparent;
}

.navbar-scrolled {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.nav-container {
  display: flex;
  padding: 1rem 0;
  color: #1b71a1;
  align-items: center;
  max-width: var(--max-width);
  margin-inline: auto;
  position: relative;
}

.logo {
  flex-shrink: 0;
}

.logo svg {
  height: 30px;
}

.mobile-menu-toggle {
  display: none;
  margin-left: auto;
  padding: 0.5rem;
  color: #283779;
  background: transparent;
  border: none;
  cursor: pointer;
  z-index: 1001;
}

nav a {
  color: #283779;
  font-weight: 600;
  transition: all 300ms ease;
  position: relative;
}

nav a::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 0;
  height: 2px;
  background: #283779;
  transition: width 300ms ease;
}

nav a:hover::after {
  width: 100%;
}

nav a:hover {
  color: #1b71a1;
}

.nav-links {
  display: flex;
  width: 450px;
  justify-content: space-between;
  align-items: center;
  gap: 0.1rem;
  margin-left: auto;
  list-style: none;
  padding: 0;
}

.nav-auth {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-left: 2rem;
  list-style: none;
  padding: 0;
}

.nav-auth button {
  font-weight: 600;
  border: 1px solid #283779;
  border-radius: 50px;
  padding: 0.4rem 1rem;
  transition: all 300ms ease;
  color: #283779;
  background: transparent;
  cursor: pointer;
}

.nav-auth button:hover {
  background-color: #283779;
  color: #fff;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(40, 55, 121, 0.3);
}

.nav-auth button.register-btn {
  background-color: transparent;
}

.nav-auth button.login-btn {
  background-color: #283779;
  color: #fff;
}

.nav-auth button.login-btn:hover {
  background-color: #1b71a1;
}

@media (max-width: 970px) {
  .mobile-menu-toggle {
    display: block;
  }

  .nav-links {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100vh;
    background: rgba(255, 255, 255, 0.98);
    backdrop-filter: blur(10px);
    flex-direction: column;
    justify-content: center;
    gap: 2rem;
    margin: 0;
    transform: translateX(-100%);
    transition: transform 0.3s ease;
    z-index: 1000;
  }

  .nav-links-open {
    transform: translateX(0);
  }

  .nav-links li {
    opacity: 0;
    animation: fadeIn 0.3s ease forwards;
  }

  .nav-links-open li {
    animation-delay: 0.1s;
  }

  .nav-auth {
    position: fixed;
    bottom: 2rem;
    left: 50%;
    transform: translateX(-50%) translateY(100%);
    flex-direction: column;
    gap: 1rem;
    margin: 0;
    transition: transform 0.3s ease;
    z-index: 1000;
  }

  .nav-auth-open {
    transform: translateX(-50%) translateY(0);
  }

  .nav-auth button {
    width: 200px;
    padding: 0.75rem 1.5rem;
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 450px) {
  .nav-container {
    padding: 0.75rem 0;
  }
}
</style>
