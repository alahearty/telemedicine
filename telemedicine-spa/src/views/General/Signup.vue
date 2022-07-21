<template>
  <AuthLayout>
    <template v-slot:form>
      <form>
        <h1>Sign up</h1>

        <div class="inputs-container">
          <input type="text" placeholder="Enter your name" />
          <input type="email" placeholder="Enter your email address" />
          <input type="password" placeholder="*****" />
        </div>

        <div class="buttons-container">
          <button class="submit-button">
            Create
            <span class="material-symbols-outlined"> arrow_right_alt </span>
          </button>
        </div>

        <span class="divider" />

        <p class="signin-options-title">Or signup with</p>

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
            Already have an account?
            <router-link :to="{ path: '/login' }">Login</router-link>
          </p>
        </div>
      </form>
    </template>
  </AuthLayout>
</template>

<script>
import AuthLayout from '@/components/layout/AuthLayout.vue'
import GoogleIcon from '@/components/icons/GoogleIcon.vue'
import FacebookIcon from '@/components/icons/FacebookIcon.vue'

export default {
  name: 'Signup',
  components: { GoogleIcon, FacebookIcon, AuthLayout },
}
</script>

<style></style>
