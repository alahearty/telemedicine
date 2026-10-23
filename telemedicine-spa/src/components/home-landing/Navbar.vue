<template>
  <header :class="['navbar', { 'navbar-scrolled': isScrolled }]">
    <nav class="nav-container">
      <router-link to="/" class="logo">
        <TelemedicineLogo />
      </router-link>

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
        <li><a href="#about" @click="closeMobileMenu">About</a></li>
        <li><a href="#doctors" @click="closeMobileMenu">Doctors</a></li>
        <li><a href="#service" @click="closeMobileMenu">Services</a></li>
      </ul>

      <ul :class="['nav-auth', { 'nav-auth-open': mobileMenuOpen }]">
        <li><button class="register-btn" @click="pushToRegister">Sign up</button></li>
        <li><button class="login-btn" @click="pushToLogin">Sign in</button></li>
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
  padding-inline: var(--bg-padding-inline, 1.5rem);
  transition: all 0.3s ease;
  background: transparent;
}

.navbar-scrolled {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(12px);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}

.nav-container {
  display: flex;
  padding: 1rem 0;
  align-items: center;
  max-width: 1200px;
  margin-inline: auto;
}

.logo {
  flex-shrink: 0;
}

.logo svg {
  height: 32px;
}

.mobile-menu-toggle {
  display: none;
  margin-left: auto;
  padding: 0.5rem;
  color: #04786a;
  background: transparent;
  border: none;
  cursor: pointer;
  z-index: 1001;
}

nav a {
  color: #065f55;
  font-weight: 600;
  font-size: 0.9375rem;
  transition: color 0.2s ease;
}

nav a:hover {
  color: #059682;
}

.nav-links {
  display: flex;
  gap: 2rem;
  margin-left: auto;
  list-style: none;
  padding: 0;
}

.nav-auth {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-left: 2rem;
  list-style: none;
  padding: 0;
}

.nav-auth button {
  font-weight: 600;
  font-size: 0.9375rem;
  border-radius: 12px;
  padding: 0.5rem 1.25rem;
  transition: all 0.2s ease;
  cursor: pointer;
}

.register-btn {
  color: #04786a;
  background: transparent;
  border: 2px solid #10b99a;
}

.register-btn:hover {
  background: #ecfdf8;
  color: #059682;
}

.login-btn {
  background: linear-gradient(135deg, #059682 0%, #04786a 100%);
  color: white;
  border: none;
}

.login-btn:hover {
  background: linear-gradient(135deg, #04786a 0%, #065f55 100%);
  box-shadow: 0 4px 14px rgba(5, 150, 130, 0.4);
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
    backdrop-filter: blur(12px);
    flex-direction: column;
    justify-content: center;
    gap: 2rem;
    margin: 0;
    transform: translateX(-100%);
    transition: transform 0.3s ease;
    z-index: 1000;
  }

  .nav-links a {
    font-size: 1.25rem;
  }

  .nav-links-open {
    transform: translateX(0);
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
</style>
