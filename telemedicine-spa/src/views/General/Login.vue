<template>
    <layout>
        <form @submit="handleSubmit">
            <h1>Log in</h1>
            <p class="subtitle">Log in to your account</p>

            <div class="inputs-container">
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
            </div>

            <div class="buttons-container">
                <a href="#">Forgot password?</a>

                <button @click="loginLocal" class="submit-button">
                    Next
                    <span class="material-symbols-outlined">
                        arrow_right_alt
                    </span>
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
    </layout>
</template>

<script>
import GoogleIcon from "@/components/icons/GoogleIcon.vue";
import FacebookIcon from "@/components/icons/FacebookIcon.vue";
import Layout from "@/Layout/AuthLayout.vue";

export default {
    name: "Login",
    components: { GoogleIcon, FacebookIcon, Layout },
    data() {
        return {
            email: "",
            password: "",
            showPassword: false,
            errors: {},
            loading: false,
            DOCTOR: "doctor",
            PATIENT: "patient",
            ADMIN: "admin",
        };
    },
    methods: {
        validateEmail() {
            if (!this.email) {
                this.errors.email = 'Email is required';
            } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
                this.errors.email = 'Please enter a valid email address';
            } else {
                delete this.errors.email;
            }
        },
        validatePassword() {
            if (!this.password) {
                this.errors.password = 'Password is required';
            } else if (this.password.length < 6) {
                this.errors.password = 'Password must be at least 6 characters';
            } else {
                delete this.errors.password;
            }
        },
        async loginLocal() {
            this.validateEmail();
            this.validatePassword();
            
            if (Object.keys(this.errors).length > 0) {
                if (this.$notification) {
                    this.$notification.error('Validation Error', 'Please fix the errors in the form');
                }
                return;
            }

            this.loading = true;
            try {
                const result = await this.login({
                    email: this.email,
                    password: this.password,
                });

                if (!result.role) throw new Error();

                localStorage.setItem("access-token", result.token);

                if (this.$notification) {
                    this.$notification.success('Login Successful', 'Welcome back!');
                }

                // Normalize role names (handle different role formats from API)
                const role = result.role?.toLowerCase();
                
                if (role === 'patient' || role === 'patientrole') {
                    this.$router.push('/dashboard');
                } else if (role === 'physician' || role === 'doctor' || role === 'physicianrole') {
                    this.$router.push('/doctors/dashboard');
                } else if (role === 'administrator' || role === 'admin' || role === 'administratorrole') {
                    this.$router.push('/dashboard');
                } else {
                    throw new Error('Unknown role');
                }
            } catch {
                if (this.$notification) {
                    this.$notification.error('Login Failed', 'Invalid email or password. Please try again.');
                }
            } finally {
                this.loading = false;
            }
        },

        async login(payload) {
            const { login } = await import('@/api');
            return login(payload.email, payload.password);
        },
    },
};
</script>

<style scoped>
.input-group {
  @apply mb-4;
}

.input-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:bg-gray-700 dark:text-gray-200 transition-all;
}

.form-input.input-error {
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
</style>
