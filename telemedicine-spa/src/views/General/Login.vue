<template>
    <layout>
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
            DOCTOR: "doctor",
            PATIENT: "patient",
            ADMIN: "admin",
        };
    },
    methods: {
        async loginLocal() {
            try {
                const result = await this.login({
                    email: this.email,
                    password: this.password,
                });

                if (!result.role) throw new Error();

                localStorage.setItem("access-token", result.token);

                if (result.role === PATIENT) {
                    this.routeTo(this.PATIENT);
                } else if (result.role === DOCTOR) {
                    this.routeTo(this.DOCTOR);
                } else if (result.role === ADMIN) {
                    this.routeTo(this.ADMIN);
                } else {
                    throw new Error();
                }
            } catch {
                console.log("An error occurred!!!");
            }
        },

        async login(payload) {
            const init = {
                method: "POST",
                headers: {
                    "content-Type": "application/json",
                },
                body: JSON.stringify(payload),
            };

            const result = {};

            try {
                const response = await fetch(
                    "https://localhost:7231/api/auth/login",
                    init
                );
                localStorage.setItem("at", response.token);

                result.token = response.token;
                result.role = response.role;
            } catch (e) {
                console.log({ e });
            } finally {
                return result;
            }
        },

        routeTo(route) {
            this.$router.push(`/${route}`);
        },

        handleSubmit(e) {
            e.preventDefault();

            if (this.email.trim() && this.email.includes("admin")) {
                this.$router.push("/dashboard");
            } else if (this.email.trim() && this.email.includes("doctor")) {
                this.$router.push("/dashboard");
            } else {
                // this.$router.push('/dashboard')
            }
            // const isUserAuthenticated = false
            // if (!isUserAuthenticated) this.$router.push('/login')
            // },
        },
    },
};
</script>

<style scoped></style>
