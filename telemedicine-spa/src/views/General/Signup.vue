<template>
  <AuthLayout>
    <form @submit.prevent="handleSubmit" class="auth-form">
      <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100">
        Create account
      </h1>
      <p class="text-slate-600 dark:text-slate-400 mt-1 mb-6">
        Register and complete your {{ isPhysician ? 'physician' : 'patient' }} profile in one step.
      </p>

      <div class="space-y-5">
        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Account type</label>
          <select v-model="accountType" data-testid="signup-account-type" class="input-modern" @change="onAccountTypeChange">
            <option value="">Select account type</option>
            <option value="Patient">Patient</option>
            <option value="Physician">Physician</option>
          </select>
          <span v-if="errors.accountType" class="block text-sm text-red-500 mt-1.5">{{ errors.accountType }}</span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">First name</label>
            <input v-model="form.firstName" data-testid="signup-first-name" type="text" class="input-modern" placeholder="Jane" />
            <span v-if="errors.firstName" class="block text-sm text-red-500 mt-1.5">{{ errors.firstName }}</span>
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Last name</label>
            <input v-model="form.lastName" data-testid="signup-last-name" type="text" class="input-modern" placeholder="Doe" />
            <span v-if="errors.lastName" class="block text-sm text-red-500 mt-1.5">{{ errors.lastName }}</span>
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Email address</label>
          <input v-model="form.email" data-testid="signup-email" type="email" class="input-modern" placeholder="you@example.com" />
          <span v-if="errors.email" class="block text-sm text-red-500 mt-1.5">{{ errors.email }}</span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Password</label>
            <input v-model="form.password" data-testid="signup-password" :type="showPassword ? 'text' : 'password'" class="input-modern" placeholder="At least 6 characters" />
            <span v-if="errors.password" class="block text-sm text-red-500 mt-1.5">{{ errors.password }}</span>
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Confirm password</label>
            <input v-model="form.confirmPassword" data-testid="signup-confirm-password" :type="showPassword ? 'text' : 'password'" class="input-modern" placeholder="Repeat password" />
            <span v-if="errors.confirmPassword" class="block text-sm text-red-500 mt-1.5">{{ errors.confirmPassword }}</span>
          </div>
        </div>
        <label class="inline-flex items-center gap-2 text-sm text-slate-600 dark:text-slate-400">
          <input v-model="showPassword" type="checkbox" class="rounded border-slate-300" />
          Show passwords
        </label>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Gender</label>
            <select v-model="form.gender" class="input-modern">
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
            <span v-if="errors.gender" class="block text-sm text-red-500 mt-1.5">{{ errors.gender }}</span>
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Date of birth</label>
            <input v-model="form.birth" data-testid="signup-birth" type="date" :max="today" class="input-modern" />
            <span v-if="errors.birth" class="block text-sm text-red-500 mt-1.5">{{ errors.birth }}</span>
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Phone</label>
          <input v-model="form.phone" data-testid="signup-phone" type="tel" class="input-modern" placeholder="+27 11 555 0100" />
          <span v-if="errors.phone" class="block text-sm text-red-500 mt-1.5">{{ errors.phone }}</span>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Address</label>
          <textarea v-model="form.address" rows="2" class="input-modern" placeholder="Street, city" />
        </div>

        <template v-if="isPhysician">
          <div class="rounded-xl bg-primary-50 dark:bg-primary-900/20 p-4 space-y-4 border border-primary-100 dark:border-primary-800">
            <h2 class="font-semibold text-slate-800 dark:text-slate-100">Professional information</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Licence number</label>
                <input v-model="form.license" type="text" class="input-modern" placeholder="DEMO-MP-003" />
                <span v-if="errors.license" class="block text-sm text-red-500 mt-1.5">{{ errors.license }}</span>
              </div>
              <div>
                <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Speciality</label>
                <input v-model="form.medicalSpecialization" type="text" class="input-modern" placeholder="General Practice" />
                <span v-if="errors.medicalSpecialization" class="block text-sm text-red-500 mt-1.5">{{ errors.medicalSpecialization }}</span>
              </div>
            </div>
            <div>
              <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Qualifications</label>
              <input v-model="form.qualifications" type="text" class="input-modern" placeholder="MBChB, diplomas" />
            </div>
            <div>
              <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Professional bio</label>
              <textarea v-model="form.bio" rows="3" class="input-modern" placeholder="Short professional summary" />
            </div>
          </div>
        </template>

        <template v-else-if="accountType === 'Patient'">
          <div class="rounded-xl bg-slate-50 dark:bg-slate-800/50 p-4 grid grid-cols-1 md:grid-cols-2 gap-4 border border-slate-100 dark:border-slate-700">
            <div>
              <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Emergency contact</label>
              <input v-model="form.emergencyContactName" type="text" class="input-modern" placeholder="Name" />
            </div>
            <div>
              <label class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">Emergency phone</label>
              <input v-model="form.emergencyContactPhone" type="tel" class="input-modern" placeholder="+27 11 555 0111" />
            </div>
          </div>
        </template>
      </div>

      <p v-if="error" class="mt-5 rounded-xl bg-red-50 border border-red-200 p-3 text-sm text-red-700 dark:bg-red-900/20 dark:border-red-800 dark:text-red-300">
        {{ error }}
      </p>

      <div class="flex gap-3 mt-8">
        <button type="button" @click="$router.push('/')" :disabled="loading" class="btn-secondary flex-1">Back</button>
        <button type="submit" data-testid="signup-submit" :disabled="loading" class="btn-primary flex-1">
          {{ loading ? 'Creating...' : 'Create account' }}
        </button>
      </div>

      <p class="text-center text-slate-600 dark:text-slate-400 mt-8">
        Already have an account?
        <router-link to="/login" class="font-semibold text-primary-600 hover:text-primary-700 dark:text-primary-400">Sign in</router-link>
      </p>
      <p class="text-center text-xs text-slate-400 mt-4">
        Demonstration platform. Use synthetic information only.
      </p>
    </form>
  </AuthLayout>
</template>

<script>
import AuthLayout from '@/Layout/AuthLayout.vue'
import { register } from '@/api'

export default {
  name: 'Signup',
  components: { AuthLayout },
  data() {
    return {
      accountType: '',
      showPassword: false,
      loading: false,
      error: '',
      errors: {},
      form: {
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
        gender: 'Female',
        birth: '',
        phone: '',
        address: '',
        emergencyContactName: '',
        emergencyContactPhone: '',
        license: '',
        medicalSpecialization: '',
        qualifications: '',
        bio: '',
      },
    }
  },
  computed: {
    isPhysician() {
      return this.accountType === 'Physician'
    },
    today() {
      return new Date().toISOString().split('T')[0]
    },
  },
  methods: {
    onAccountTypeChange() {
      this.errors.accountType = undefined
    },
    validate() {
      const errors = {}
      if (!this.accountType) errors.accountType = 'Account type is required'
      if (!this.form.firstName.trim()) errors.firstName = 'First name is required'
      if (!this.form.lastName.trim()) errors.lastName = 'Last name is required'
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.form.email)) errors.email = 'A valid email is required'
      if (!this.form.password || this.form.password.length < 6) errors.password = 'Password must be at least 6 characters'
      if (this.form.password !== this.form.confirmPassword) errors.confirmPassword = 'Passwords do not match'
      if (!this.form.phone.trim()) errors.phone = 'Phone is required'
      if (!this.form.birth) errors.birth = 'Date of birth is required'
      else if (this.form.birth > this.today) errors.birth = 'Date of birth cannot be in the future'
      if (this.isPhysician) {
        if (!this.form.license.trim()) errors.license = 'Licence number is required'
        if (!this.form.medicalSpecialization.trim()) errors.medicalSpecialization = 'Speciality is required'
      }
      this.errors = errors
      return Object.keys(errors).length === 0
    },
    async handleSubmit() {
      this.error = ''
      if (!this.validate()) {
        this.error = 'Please correct the highlighted fields.'
        return
      }
      this.loading = true
      try {
        await register({
          email: this.form.email.trim(),
          password: this.form.password,
          accountType: this.accountType,
          firstName: this.form.firstName.trim(),
          lastName: this.form.lastName.trim(),
          gender: this.form.gender,
          birth: this.form.birth,
          phone: this.form.phone.trim(),
          address: this.form.address.trim(),
          emergencyContactName: this.form.emergencyContactName.trim(),
          emergencyContactPhone: this.form.emergencyContactPhone.trim(),
          license: this.isPhysician ? this.form.license.trim() : null,
          medicalSpecialization: this.isPhysician ? this.form.medicalSpecialization.trim() : null,
          qualifications: this.isPhysician ? this.form.qualifications.trim() : null,
          bio: this.isPhysician ? this.form.bio.trim() : null,
        })
        if (this.$notification) this.$notification.success('Account created', 'You can now sign in.')
        this.$router.push('/login')
      } catch (e) {
        this.error = e.message || 'Registration failed. Please try again.'
        if (this.$notification) this.$notification.error('Registration failed', this.error)
      } finally {
        this.loading = false
      }
    },
  },
}
</script>
