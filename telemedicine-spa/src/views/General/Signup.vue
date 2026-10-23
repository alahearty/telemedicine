<template>
  <AuthLayout>
    <form @submit.prevent="handleSubmit" class="auth-form">
      <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100">
        Create account
      </h1>
      <p class="text-slate-600 dark:text-slate-400 mt-1 mb-8">
        Get started with telemedicine in minutes
      </p>

      <div class="space-y-5">
        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
            Account type
          </label>
          <select
            v-model="accountType"
            :class="['input-modern', { 'ring-2 ring-red-500 border-red-500': errors.accountType }]"
            @blur="validateAccountType"
          >
            <option value="">Select account type</option>
            <option value="Patient">Patient</option>
            <option value="Physician">Physician</option>
          </select>
          <span v-if="errors.accountType" class="block text-sm text-red-500 mt-1.5">{{ errors.accountType }}</span>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
            Email address
          </label>
          <input
            type="email"
            v-model="email"
            placeholder="you@example.com"
            :class="['input-modern', { 'ring-2 ring-red-500 border-red-500': errors.email }]"
            @blur="validateEmail"
          />
          <span v-if="errors.email" class="block text-sm text-red-500 mt-1.5">{{ errors.email }}</span>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
            Password
          </label>
          <div class="relative">
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              placeholder="••••••••"
              :class="['input-modern pr-12', { 'ring-2 ring-red-500 border-red-500': errors.password }]"
              @blur="validatePassword"
            />
            <button
              type="button"
              class="absolute right-3 top-1/2 -translate-y-1/2 p-2 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 rounded-lg"
              @click="showPassword = !showPassword"
            >
              <svg v-if="showPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.066 5.74m0 0L21 21" />
              </svg>
              <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            </button>
          </div>
          <span v-if="errors.password" class="block text-sm text-red-500 mt-1.5">{{ errors.password }}</span>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
            Confirm password
          </label>
          <div class="relative">
            <input
              :type="showConfirmPassword ? 'text' : 'password'"
              v-model="confirmPassword"
              placeholder="••••••••"
              :class="['input-modern pr-12', { 'ring-2 ring-red-500 border-red-500': errors.confirmPassword }]"
              @blur="validateConfirmPassword"
            />
            <button
              type="button"
              class="absolute right-3 top-1/2 -translate-y-1/2 p-2 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 rounded-lg"
              @click="showConfirmPassword = !showConfirmPassword"
            >
              <svg v-if="showConfirmPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.066 5.74m0 0L21 21" />
              </svg>
              <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            </button>
          </div>
          <span v-if="errors.confirmPassword" class="block text-sm text-red-500 mt-1.5">{{ errors.confirmPassword }}</span>
        </div>
      </div>

      <div class="flex gap-3 mt-8">
        <button
          type="button"
          @click="handleBack"
          :disabled="loading"
          class="btn-secondary flex-1"
        >
          Back
        </button>
        <button
          type="submit"
          :disabled="loading"
          class="btn-primary flex-1 inline-flex items-center justify-center gap-2"
        >
          <span v-if="loading">Creating...</span>
          <span v-else>Create account</span>
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3" />
          </svg>
        </button>
      </div>

      <div class="relative my-8">
        <div class="absolute inset-0 flex items-center">
          <div class="w-full border-t border-slate-200 dark:border-slate-600"></div>
        </div>
        <div class="relative flex justify-center text-sm">
          <span class="px-4 bg-white dark:bg-slate-800 text-slate-500">Or sign up with</span>
        </div>
      </div>

      <div class="flex gap-3">
        <button
          type="button"
          @click="handleSocialSignup('google')"
          class="flex-1 flex items-center justify-center gap-2 py-3 px-4 rounded-xl border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors font-medium text-slate-700 dark:text-slate-300"
        >
          <GoogleIcon class="w-5 h-5" />
          Google
        </button>
        <button
          type="button"
          @click="handleSocialSignup('facebook')"
          class="flex-1 flex items-center justify-center gap-2 py-3 px-4 rounded-xl border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors font-medium text-slate-700 dark:text-slate-300"
        >
          <FacebookIcon class="w-5 h-5" />
          Facebook
        </button>
      </div>

      <p class="text-center text-slate-600 dark:text-slate-400 mt-8">
        Already have an account?
        <router-link to="/login" class="font-semibold text-primary-600 hover:text-primary-700 dark:text-primary-400">
          Sign in
        </router-link>
      </p>
    </form>
  </AuthLayout>
</template>

<script>
import AuthLayout from '@/Layout/AuthLayout.vue'
import GoogleIcon from '@/components/icons/GoogleIcon.vue'
import FacebookIcon from '@/components/icons/FacebookIcon.vue'
import { register } from '@/api'

export default {
  name: 'Signup',
  components: { GoogleIcon, FacebookIcon, AuthLayout },
  data() {
    return {
      accountType: '',
      email: '',
      password: '',
      confirmPassword: '',
      showPassword: false,
      showConfirmPassword: false,
      errors: {},
      loading: false,
    }
  },
  methods: {
    validateAccountType() {
      if (!this.accountType) this.errors.accountType = 'Account type is required'
      else delete this.errors.accountType
    },
    validateEmail() {
      if (!this.email) this.errors.email = 'Email is required'
      else if (!/^[^\s@]+@[^\s@]+(\.[^\s@]+)*$/.test(this.email)) this.errors.email = 'Please enter a valid email address'
      else delete this.errors.email
    },
    validatePassword() {
      if (!this.password) this.errors.password = 'Password is required'
      else if (this.password.length < 6) this.errors.password = 'Password must be at least 6 characters'
      else delete this.errors.password
    },
    validateConfirmPassword() {
      if (!this.confirmPassword) this.errors.confirmPassword = 'Please confirm your password'
      else if (this.password !== this.confirmPassword) this.errors.confirmPassword = 'Passwords do not match'
      else delete this.errors.confirmPassword
    },
    validateForm() {
      this.validateAccountType()
      this.validateEmail()
      this.validatePassword()
      this.validateConfirmPassword()
      return Object.keys(this.errors).length === 0
    },
    async handleSubmit() {
      if (!this.validateForm()) {
        if (this.$notification) this.$notification.error('Validation Error', 'Please fix the errors in the form')
        return
      }
      this.loading = true
      try {
        await register(this.email, this.password, this.accountType)
        if (this.$notification) this.$notification.success('Account created', 'You can now sign in.')
        setTimeout(() => this.$router.push('/login'), 1500)
      } catch (error) {
        const msg = error.message || 'Registration failed. Please try again.'
        if (this.$notification) this.$notification.error('Registration failed', msg)
      } finally {
        this.loading = false
      }
    },
    handleBack() {
      this.$router.push('/')
    },
    handleSocialSignup(provider) {
      if (this.$notification) this.$notification.info('Coming soon', `${provider} signup will be available soon.`)
    },
  },
}
</script>
