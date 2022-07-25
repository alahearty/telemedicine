<template>
      <form @submit="handleSubmit">
        <h1>Log in</h1>
        <p class="subtitle">Log in to your account</p>

        <div class="inputs-container">
          <input
            type="email"
            v-model="email"
            placeholder="Enter your email address"
          />
          <input type="password" v-model="password" placeholder="*****" />
        </div>

        <div class="buttons-container">
          <a href="#">Forgot password?</a>

          <button class="submit-button">
            Next
            <span class="material-symbols-outlined"> arrow_right_alt </span>
          </button>
        </div>

        <span class="divider" />

        <p class="signin-options-title">Or login with</p>

        <div class="signin-options-container">
          <button>
            <FacebookIcon />
            Facebook
          </button>
          <button>
            <GoogleIcon />
            Google
          </button>
        </div>

        <div class="alt-sign-in">
          <p>
            Don’t have an account yet?
            <router-link :to="{ path: '/signup' }">Signup</router-link>
          </p>
        </div>
      </form>
</template>

<script>
import AuthLayout from '@/Layout/AuthLayout.vue'
import GoogleIcon from '@/components/icons/GoogleIcon.vue'
import FacebookIcon from '@/components/icons/FacebookIcon.vue'
import Layout from '@/Layout/AuthLayout.vue'

export default {
  name: 'Login',
  components: { GoogleIcon, FacebookIcon, AuthLayout },
  data() {
    return {
      email: '',
      password: '',
    }
  },
  methods: {
    handleSubmit(e) {
      e.preventDefault()
      if (this.email.trim()) this.$router.push('/dashboard')
    // const isUserAuthenticated = false
    // if (!isUserAuthenticated) this.$router.push('/login')
    // },
    }
  },
  created() {
    this.$emit(`update:layout`,Layout);
  },
}
</script>

<style></style>
