<template>
  <div class="space-y-6">
    <PageHeader
      eyebrow="Account"
      title="Account settings"
      :subtitle="subtitle"
    />

    <div class="grid items-start gap-6 lg:grid-cols-[minmax(0,320px)_minmax(0,1fr)]">
      <div class="space-y-6">
        <section class="card overflow-hidden">
          <div class="h-24 bg-[linear-gradient(125deg,#0f766e,#0d9488_55%,#0891b2)]">
            <div class="h-full w-full bg-[radial-gradient(circle_at_80%_20%,rgba(255,255,255,0.25),transparent_50%)]" />
          </div>
          <div class="px-5 pb-5">
            <div class="-mt-10 flex items-end justify-between">
              <AppAvatar :name="profileName" size="xl" class="ring-4 ring-white dark:ring-midnight-850" />
              <span class="badge badge-info mb-1">{{ roleLabel }}</span>
            </div>
            <h2 class="mt-3 font-display text-lg font-semibold text-ink-900 dark:text-slate-100">
              {{ profileName || roleLabel }}
            </h2>
            <p class="mt-1 flex items-center gap-1.5 truncate text-xs text-slate-500 dark:text-slate-400">
              <Icon icon="mail" class="text-sm" />
              {{ email || 'No email on record' }}
            </p>
            <div class="mt-4 rounded-xl border border-slate-100 bg-slate-50/80 p-3 dark:border-white/[0.06] dark:bg-white/[0.03]">
              <p class="flex items-center gap-2 text-[11px] font-semibold text-slate-600 dark:text-slate-300">
                <Icon icon="shield" class="text-sm text-primary-500" />
                Demo account
              </p>
              <p class="mt-1 text-[11px] leading-5 text-slate-500 dark:text-slate-400">
                This workspace uses synthetic data for safe demonstrations. No real patient information is stored.
              </p>
            </div>
          </div>
        </section>

        <section class="card p-5">
          <div class="flex items-center gap-3">
            <span class="icon-tile-sm bg-violet-50 text-violet-600 dark:bg-violet-400/10 dark:text-violet-300">
              <Icon icon="palette" class="text-base" />
            </span>
            <div>
              <h2 class="font-display text-sm font-semibold text-ink-900 dark:text-slate-100">Appearance</h2>
              <p class="text-[11px] text-slate-500 dark:text-slate-400">Choose your preferred theme</p>
            </div>
          </div>
          <div class="mt-4 grid grid-cols-3 gap-2">
            <button
              v-for="option in themeOptions"
              :key="option.value"
              type="button"
              class="flex flex-col items-center gap-2 rounded-xl border px-2 py-3 text-[11px] font-semibold transition-all"
              :class="mode === option.value
                ? 'border-primary-300 bg-primary-50 text-primary-800 shadow-sm dark:border-primary-400/30 dark:bg-primary-400/10 dark:text-primary-200'
                : 'border-slate-200 bg-white text-slate-500 hover:border-primary-200 hover:text-primary-700 dark:border-white/[0.08] dark:bg-white/[0.02] dark:text-slate-400 dark:hover:border-primary-400/25 dark:hover:text-primary-300'"
              :aria-pressed="mode === option.value"
              @click="setTheme(option.value)"
            >
              <Icon :icon="option.icon" class="text-lg" />
              {{ option.label }}
            </button>
          </div>
        </section>
      </div>

      <div class="space-y-6">
        <div v-if="loading" class="card space-y-5 p-6">
          <div class="h-5 w-40 skeleton" />
          <div class="grid gap-4 sm:grid-cols-2">
            <div class="h-11 skeleton" />
            <div class="h-11 skeleton" />
            <div class="h-11 skeleton sm:col-span-2" />
            <div class="h-11 skeleton" />
          </div>
        </div>

        <section v-else-if="isAdmin" class="card p-6">
          <div class="flex items-start gap-4">
            <span class="icon-tile bg-teal-50 text-teal-600 dark:bg-teal-400/10 dark:text-teal-300">
              <Icon icon="shield" class="text-xl" />
            </span>
            <div class="min-w-0">
              <h2 class="font-display text-lg font-semibold text-ink-900 dark:text-slate-100">Administrator profile</h2>
              <p class="mt-1.5 text-sm leading-6 text-slate-500 dark:text-slate-400">
                Your administrator identity is managed by the platform. Patient profile fields do not apply to this account.
              </p>
            </div>
          </div>

          <dl class="mt-6 grid gap-4 sm:grid-cols-2">
            <div class="rounded-xl border border-slate-100 bg-slate-50/70 p-4 dark:border-white/[0.06] dark:bg-white/[0.03]">
              <dt class="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-400 dark:text-slate-500">Display name</dt>
              <dd class="mt-1 text-sm font-semibold text-ink-900 dark:text-slate-100">{{ profileName }}</dd>
            </div>
            <div class="rounded-xl border border-slate-100 bg-slate-50/70 p-4 dark:border-white/[0.06] dark:bg-white/[0.03]">
              <dt class="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-400 dark:text-slate-500">Email</dt>
              <dd class="mt-1 truncate text-sm font-semibold text-ink-900 dark:text-slate-100">{{ email || 'Not set' }}</dd>
            </div>
            <div class="rounded-xl border border-slate-100 bg-slate-50/70 p-4 dark:border-white/[0.06] dark:bg-white/[0.03]">
              <dt class="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-400 dark:text-slate-500">Role</dt>
              <dd class="mt-1 text-sm font-semibold text-ink-900 dark:text-slate-100">Administrator</dd>
            </div>
            <div class="rounded-xl border border-slate-100 bg-slate-50/70 p-4 dark:border-white/[0.06] dark:bg-white/[0.03]">
              <dt class="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-400 dark:text-slate-500">Workspace</dt>
              <dd class="mt-1 text-sm font-semibold text-ink-900 dark:text-slate-100">Telemedicine demo</dd>
            </div>
          </dl>

          <div class="mt-6 flex flex-wrap gap-2 border-t border-slate-100 pt-5 dark:border-white/[0.06]">
            <router-link to="/dashboard" class="btn-primary">
              <Icon icon="dashboard" class="text-base" />
              Open dashboard
            </router-link>
            <router-link to="/dashboard/hospital" class="btn-secondary">
              <Icon icon="building" class="text-base" />
              Manage hospitals
            </router-link>
          </div>
        </section>

        <section v-else-if="error" class="card p-8 text-center">
          <span class="icon-tile mx-auto bg-amber-50 text-amber-600 dark:bg-amber-400/10 dark:text-amber-300">
            <Icon icon="alert-circle" class="text-xl" />
          </span>
          <h2 class="mt-4 font-display text-lg font-semibold text-ink-900 dark:text-slate-100">Profile not available</h2>
          <p class="mx-auto mt-1.5 max-w-md text-sm leading-6 text-slate-500 dark:text-slate-400">{{ error }}</p>
          <router-link to="/dashboard/update-record" class="btn-primary mt-5">
            <Icon icon="file-text" class="text-base" />
            Complete your profile
          </router-link>
        </section>

        <form v-else class="card p-6" @submit.prevent="save">
          <div class="flex items-start justify-between gap-4">
            <div>
              <h2 class="font-display text-lg font-semibold text-ink-900 dark:text-slate-100">Personal information</h2>
              <p class="mt-1 text-xs text-slate-500 dark:text-slate-400">These details appear on your appointments and care records.</p>
            </div>
            <span class="badge badge-neutral">{{ roleLabel }}</span>
          </div>

          <div class="mt-6 grid gap-5 sm:grid-cols-2">
            <div>
              <label for="first-name" class="label">First name</label>
              <input id="first-name" v-model="form.firstName" type="text" class="input-modern" placeholder="First name" autocomplete="given-name" />
            </div>
            <div>
              <label for="last-name" class="label">Last name</label>
              <input id="last-name" v-model="form.lastName" type="text" class="input-modern" placeholder="Last name" autocomplete="family-name" />
            </div>
            <div class="sm:col-span-2">
              <label for="email" class="label flex items-center gap-1.5">
                Email
                <Icon icon="lock" class="text-xs text-slate-400" />
              </label>
              <input id="email" v-model="form.email" type="email" class="input-modern" readonly aria-describedby="email-help" />
              <p id="email-help" class="helper">Email is linked to your sign-in and cannot be changed here.</p>
            </div>
            <div>
              <label for="phone" class="label">Phone number</label>
              <input id="phone" v-model="form.phone" type="tel" class="input-modern" placeholder="+27 82 123 4567" autocomplete="tel" />
              <p class="helper">Include your country code so your care team can reach you.</p>
            </div>
          </div>

          <div class="sticky bottom-4 z-10 mt-6 flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-slate-200/80 bg-white/90 p-3 shadow-soft backdrop-blur-xl dark:border-white/[0.08] dark:bg-midnight-900/90">
            <p class="flex items-center gap-2 text-xs font-medium" :class="dirty ? 'text-amber-600 dark:text-amber-300' : 'text-slate-500 dark:text-slate-400'">
              <span class="h-1.5 w-1.5 rounded-full" :class="dirty ? 'bg-amber-500' : 'bg-emerald-500'" />
              {{ dirty ? 'You have unsaved changes' : 'All changes saved' }}
            </p>
            <div class="flex gap-2">
              <button type="button" class="btn-secondary" :disabled="!dirty || saving" @click="reset">Cancel</button>
              <button type="submit" class="btn-primary" :disabled="!dirty || saving">
                <Icon icon="check" class="text-base" />
                {{ saving ? 'Saving...' : 'Save changes' }}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import AppAvatar from '@/components/ui/AppAvatar.vue'
import PageHeader from '@/components/ui/PageHeader.vue'
import { useTheme } from '@/composables/useTheme'
import { setSessionName, useSession } from '@/composables/useSession'
import { getCurrentPatient, updatePatient } from '@/api'

export default {
  name: 'Account',
  components: { AppAvatar, PageHeader },
  setup() {
    const { mode, setTheme } = useTheme()
    return { mode, setTheme, session: useSession() }
  },
  data() {
    return {
      loading: true,
      saving: false,
      error: null,
      initial: null,
      form: { id: null, firstName: '', lastName: '', email: '', phone: '' },
      themeOptions: [
        { value: 'light', label: 'Light', icon: 'sun' },
        { value: 'dark', label: 'Dark', icon: 'moon' },
        { value: 'system', label: 'System', icon: 'monitor' },
      ],
    }
  },
  computed: {
    role() {
      const role = (localStorage.getItem('user-role') || '').toLowerCase()
      if (role.includes('admin') || role.includes('administrator')) return 'admin'
      if (role.includes('physician') || role.includes('doctor')) return 'physician'
      return 'patient'
    },
    isAdmin() {
      return this.role === 'admin'
    },
    roleLabel() {
      return { admin: 'Administrator', physician: 'Physician', patient: 'Patient' }[this.role]
    },
    subtitle() {
      return this.isAdmin
        ? 'Review your administrator identity, workspace shortcuts and preferred appearance.'
        : 'Manage your personal details and choose how the workspace looks for you.'
    },
    storedName() {
      if (this.session.name) return this.session.name
      const local = (this.session.email || '').split('@')[0]
      if (!local) return this.roleLabel
      return local.charAt(0).toUpperCase() + local.slice(1)
    },
    profileName() {
      if (this.form.firstName) return `${this.form.firstName} ${this.form.lastName || ''}`.trim()
      return this.storedName
    },
    email() {
      return this.form.email || this.session.email || ''
    },
    dirty() {
      if (!this.initial) return false
      return ['firstName', 'lastName', 'phone']
        .some((key) => (this.form[key] || '') !== (this.initial[key] || ''))
    },
  },
  mounted() {
    this.load()
  },
  methods: {
    async load() {
      this.loading = true
      this.error = null
      if (this.isAdmin) {
        this.form = {
          id: null,
          firstName: this.storedName,
          lastName: '',
          email: this.session.email || '',
          phone: '',
        }
        this.initial = { ...this.form }
        this.loading = false
        return
      }
      try {
        const patient = await getCurrentPatient()
        this.form = {
          id: patient.id,
          firstName: patient.firstName || '',
          lastName: patient.lastName || '',
          email: patient.email || this.session.email || '',
          phone: patient.phone || '',
        }
        this.initial = { ...this.form }
        if (this.form.firstName) {
          setSessionName(`${this.form.firstName} ${this.form.lastName || ''}`.trim())
        }
      } catch (error) {
        this.error = error.message || 'We could not load your patient profile. Complete your record to continue.'
      } finally {
        this.loading = false
      }
    },
    reset() {
      if (this.initial) this.form = { ...this.initial }
    },
    async save() {
      if (!this.dirty || this.saving) return
      if (!this.form.id) {
        this.error = 'Please complete your profile in Update Record first.'
        return
      }
      this.saving = true
      try {
        await updatePatient(this.form.id, {
          firstName: this.form.firstName,
          lastName: this.form.lastName,
          phone: this.form.phone,
        })
        this.initial = { ...this.form }
        setSessionName(`${this.form.firstName} ${this.form.lastName || ''}`.trim())
        if (this.$notification) this.$notification.success('Saved', 'Your profile has been updated.')
      } catch (error) {
        if (this.$notification) this.$notification.error('Error', error.message || 'Could not save your changes.')
      } finally {
        this.saving = false
      }
    },
  },
}
</script>
