<template>
  <AuthLayout>
    <form @submit.prevent="handleSubmit">
      <h1>Sign up</h1>
      <p class="subtitle">Create your account to get started</p>

      <div class="inputs-container">
        <div class="input-group">
          <label class="input-label">Account Type</label>
          <select 
            v-model="accountType" 
            :class="['form-select', { 'input-error': errors.accountType }]"
            @blur="validateAccountType"
          >
            <option value="">Select account type</option>
            <option value="Patient">Patient</option>
            <option value="Physician">Physician</option>
          </select>
          <span v-if="errors.accountType" class="error-message">{{ errors.accountType }}</span>
        </div>

        <div class="input-group">
          <label class="input-label">Email Address</label>
          <input 
            type="email" 
            v-model="email"
            placeholder="Enter your email address"
            :class="['form-input', { 'input-error': errors.email }]"
            @blur="validateEmail"
          />
          <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
        </div>

        <div class="input-group">
          <label class="input-label">Password</label>
          <div class="password-input-wrapper">
            <input 
              :type="showPassword ? 'text' : 'password'" 
              v-model="password"
              placeholder="Enter your password"
              :class="['form-input', { 'input-error': errors.password }]"
              @blur="validatePassword"
            />
            <button 
              type="button"
              class="password-toggle"
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
          <span v-if="errors.password" class="error-message">{{ errors.password }}</span>
        </div>

        <div class="input-group">
          <label class="input-label">Confirm Password</label>
          <div class="password-input-wrapper">
            <input 
              :type="showConfirmPassword ? 'text' : 'password'" 
              v-model="confirmPassword"
              placeholder="Confirm your password"
              :class="['form-input', { 'input-error': errors.confirmPassword }]"
              @blur="validateConfirmPassword"
            />
            <button 
              type="button"
              class="password-toggle"
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
          <span v-if="errors.confirmPassword" class="error-message">{{ errors.confirmPassword }}</span>
        </div>
      </div>

      <div class="buttons-container">
        <button 
          type="submit" 
          class="submit-button"
          :disabled="loading"
        >
          <span v-if="loading">Creating...</span>
          <span v-else>Create Account</span>
          <span class="material-symbols-outlined"> arrow_forward </span>
        </button>
        <button 
          type="button" 
          class="submit-button secondary" 
          @click="handleBack"
          :disabled="loading"
        >
          <span class="material-symbols-outlined"> arrow_back </span>
          Back
        </button>
      </div>

      <span class="divider" />

      <p class="signin-options-title">Or signup with</p>

      <div class="signin-options-container">
        <button type="button" @click="handleSocialSignup('facebook')">
          <FacebookIcon />
          Facebook
        </button>
        <button type="button" @click="handleSocialSignup('google')">
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
  </AuthLayout>
</template>

<script>
import AuthLayout from '@/Layout/AuthLayout.vue'
import GoogleIcon from '@/components/icons/GoogleIcon.vue'
import FacebookIcon from '@/components/icons/FacebookIcon.vue'
import { postAsync } from '@/api-services'

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
      if (!this.accountType) {
        this.errors.accountType = 'Account type is required'
      } else {
        delete this.errors.accountType
      }
    },
    validateEmail() {
      if (!this.email) {
        this.errors.email = 'Email is required'
      } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
        this.errors.email = 'Please enter a valid email address'
      } else {
        delete this.errors.email
      }
    },
    validatePassword() {
      if (!this.password) {
        this.errors.password = 'Password is required'
      } else if (this.password.length < 6) {
        this.errors.password = 'Password must be at least 6 characters'
      } else {
        delete this.errors.password
      }
    },
    validateConfirmPassword() {
      if (!this.confirmPassword) {
        this.errors.confirmPassword = 'Please confirm your password'
      } else if (this.password !== this.confirmPassword) {
        this.errors.confirmPassword = 'Passwords do not match'
      } else {
        delete this.errors.confirmPassword
      }
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
        if (this.$notification) {
          this.$notification.error('Validation Error', 'Please fix the errors in the form')
        }
        return
      }

      this.loading = true
      try {
        const response = await postAsync('/auth/register', {
          email: this.email,
          password: this.password,
          accountType: this.accountType
        })

        if (response.wasSuccesful || response.success || !response.notSuccesful) {
          if (this.$notification) {
            this.$notification.success('Registration Successful', 'Your account has been created successfully!')
          }
          // Redirect to login after successful registration
          setTimeout(() => {
            this.$router.push('/login')
          }, 1500)
        } else {
          const errorMessage = response.error || response.message || 'Registration failed'
          throw new Error(errorMessage)
        }
      } catch (error) {
        console.error('Registration error:', error)
        const errorMessage = error.message || 'Registration failed. Please try again.'
        if (this.$notification) {
          this.$notification.error('Registration Failed', errorMessage)
        }
      } finally {
        this.loading = false
      }
    },
    handleBack() {
      this.$router.push('/')
    },
    handleSocialSignup(provider) {
      // TODO: Implement social signup
      console.log(`Social signup with ${provider}`)
      if (this.$notification) {
        this.$notification.info('Coming Soon', `Social signup with ${provider} will be available soon.`)
      }
    },
  },
}
</script>

<style scoped>
.subtitle {
  @apply text-gray-600 dark:text-gray-400 mb-6 text-center;
}

.input-group {
  @apply mb-4;
}

.input-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input,
.form-select {
  @apply w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:bg-gray-700 dark:text-gray-200 transition-all;
}

.form-select {
  @apply appearance-none bg-white dark:bg-gray-700;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3E%3Cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3E%3C/svg%3E");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}

.form-input.input-error,
.form-select.input-error {
  @apply border-red-500 focus:ring-red-500;
}

.password-input-wrapper {
  @apply relative;
}

.password-toggle {
  @apply absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-gray-700 dark:hover:text-gray-300;
}

.error-message {
  @apply block text-sm text-red-500 mt-1;
}

.submit-button.secondary {
  @apply bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-gray-600;
}

.submit-button:disabled {
  @apply opacity-50 cursor-not-allowed;
}
</style>
