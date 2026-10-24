<template>
  <div>
    <template v-if="isAdmin">
      <div class="space-y-6">
        <PageHeader
          eyebrow="Administration"
          title="Admin dashboard"
          subtitle="A calm, live view of platform activity, payments and the people using your care network."
        >
          <template #actions>
            <span class="badge badge-neutral">
              <Icon icon="activity" class="text-[11px]" />
              Demo dataset
            </span>
            <router-link to="/dashboard/hospital" class="btn-primary">
              <Icon icon="building" class="text-base" />
              Manage hospitals
            </router-link>
          </template>
        </PageHeader>

        <div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          <StatCard
            label="Total revenue"
            :value="formatCurrency(totalRevenue)"
            :hint="`${formatNumber(paidTransactions)} paid transactions`"
            icon="wallet"
            tone="teal"
            :loading="loading"
          />
          <StatCard
            label="Paid transactions"
            :value="formatNumber(paidTransactions)"
            :hint="unpaidTransactions ? `${formatNumber(unpaidTransactions)} awaiting payment` : 'All payments settled'"
            icon="receipt"
            tone="cyan"
            :loading="loading"
          />
          <StatCard
            label="Patients"
            :value="formatNumber(patients.length)"
            :hint="`${formatNumber(physicians.length)} physicians onboard`"
            icon="users"
            tone="violet"
            :loading="loading"
          />
          <StatCard
            label="Services"
            :value="formatNumber(services.length)"
            :hint="services.length ? `Average ${formatCurrency(averageServiceCost)}` : 'No services configured'"
            icon="stethoscope"
            tone="amber"
            :loading="loading"
          />
        </div>

        <div class="grid gap-6 xl:grid-cols-[minmax(0,1.65fr)_minmax(0,1fr)]">
          <section class="card p-5 sm:p-6">
            <div class="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Service catalogue</h2>
                <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Configured consultation prices across your care catalogue</p>
              </div>
              <span class="badge badge-info">
                <Icon icon="bar-chart" class="text-[11px]" />
                {{ formatNumber(services.length) }} services
              </span>
            </div>

            <div v-if="loading" class="mt-6 space-y-3">
              <div v-for="n in 3" :key="n" class="h-9 skeleton" :style="{ width: `${92 - n * 14}%` }" />
            </div>
            <div v-else-if="servicePricing.length" class="mt-4">
              <apexchart
                :key="isDark ? 'pricing-dark' : 'pricing-light'"
                type="bar"
                height="330"
                :options="pricingOptions"
                :series="pricingSeries"
              />
            </div>
            <EmptyState
              v-else
              icon="stethoscope"
              title="No services configured"
              description="Add consultation services to see their prices compared here."
            />
          </section>

          <section class="card p-5 sm:p-6">
            <div class="flex items-start justify-between gap-3">
              <div>
                <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">User mix</h2>
                <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Accounts by role</p>
              </div>
              <span class="badge badge-neutral">{{ formatNumber(totalUsers) }} total</span>
            </div>

            <div v-if="loading" class="mx-auto mt-8 h-44 w-44 rounded-full border-[18px] border-slate-100 skeleton dark:border-white/[0.05]" />
            <div v-else-if="userMix.length" class="mt-4">
              <div class="relative mx-auto h-[230px] max-w-[280px]">
                <apexchart
                  :key="isDark ? 'mix-dark' : 'mix-light'"
                  type="donut"
                  height="230"
                  :options="mixOptions"
                  :series="mixSeries"
                />
                <div class="pointer-events-none absolute inset-0 flex flex-col items-center justify-center">
                  <span class="font-display text-2xl font-semibold text-ink-900 dark:text-white">{{ formatNumber(totalUsers) }}</span>
                  <span class="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-400 dark:text-slate-500">accounts</span>
                </div>
              </div>
              <div class="mt-2 space-y-2">
                <div v-for="item in userMix" :key="item.label" class="flex items-center gap-2.5 text-xs">
                  <span class="h-2 w-2 rounded-full" :style="{ backgroundColor: item.color }" />
                  <span class="flex-1 text-slate-600 dark:text-slate-300">{{ item.label }}</span>
                  <span class="font-semibold text-ink-900 dark:text-slate-100">{{ formatNumber(item.value) }}</span>
                </div>
              </div>
            </div>
            <EmptyState v-else icon="users" title="No accounts yet" description="Registered patients and physicians will be summarised here." />
          </section>
        </div>

        <div class="grid gap-6 xl:grid-cols-[minmax(0,1.65fr)_minmax(0,1fr)]">
          <section class="card overflow-hidden">
            <div class="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-4 dark:border-white/[0.06] sm:px-6">
              <div>
                <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Latest payments</h2>
                <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Synthetic payment activity from this workspace</p>
              </div>
              <span class="badge badge-neutral">{{ formatNumber(payments.length) }} records</span>
            </div>

            <div v-if="loading" class="space-y-3 p-5 sm:p-6">
              <div v-for="n in 4" :key="n" class="h-10 skeleton" />
            </div>

            <div v-else-if="payments.length">
              <div class="hidden overflow-x-auto sm:block">
                <table class="w-full text-left text-xs">
                  <thead>
                    <tr class="border-b border-slate-100 text-[10px] uppercase tracking-[0.12em] text-slate-400 dark:border-white/[0.06] dark:text-slate-500">
                      <th class="px-6 py-3 font-semibold">Service</th>
                      <th class="px-6 py-3 font-semibold">Amount</th>
                      <th class="px-6 py-3 font-semibold">Status</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100 dark:divide-white/[0.05]">
                    <tr v-for="(payment, index) in paymentRows.slice(0, 6)" :key="index" class="transition-colors hover:bg-slate-50/70 dark:hover:bg-white/[0.02]">
                      <td class="px-6 py-3.5">
                        <div class="flex items-center gap-3">
                          <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-primary-50 text-primary-600 dark:bg-primary-400/10 dark:text-primary-300">
                            <Icon icon="receipt" class="text-sm" />
                          </span>
                          <span class="font-medium text-slate-700 dark:text-slate-200">{{ payment.serviceName }}</span>
                        </div>
                      </td>
                      <td class="px-6 py-3.5 font-semibold text-ink-900 dark:text-slate-100">{{ formatCurrency(payment.amountPaid) }}</td>
                      <td class="px-6 py-3.5">
                        <span class="badge" :class="payment.isPaid ? 'badge-success' : 'badge-warning'">
                          <span class="h-1.5 w-1.5 rounded-full" :class="payment.isPaid ? 'bg-emerald-500' : 'bg-amber-500'" />
                          {{ payment.isPaid ? 'Paid' : 'Unpaid' }}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="space-y-2 p-4 sm:hidden">
                <div v-for="(payment, index) in paymentRows.slice(0, 6)" :key="`m-${index}`" class="rounded-xl border border-slate-100 p-3 dark:border-white/[0.06]">
                  <div class="flex items-start justify-between gap-3">
                    <div>
                      <p class="text-sm font-semibold text-ink-900 dark:text-slate-100">{{ payment.serviceName }}</p>
                      <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">{{ formatCurrency(payment.amountPaid) }}</p>
                    </div>
                    <span class="badge" :class="payment.isPaid ? 'badge-success' : 'badge-warning'">{{ payment.isPaid ? 'Paid' : 'Unpaid' }}</span>
                  </div>
                </div>
              </div>
            </div>
            <EmptyState v-else icon="receipt" title="No payments yet" description="Simulated consultation payments will appear here." />
          </section>

          <section class="card overflow-hidden">
            <div class="flex items-center justify-between border-b border-slate-100 px-5 py-4 dark:border-white/[0.06] sm:px-6">
              <div>
                <h2 class="font-display text-base font-semibold text-ink-900 dark:text-slate-100">Patients</h2>
                <p class="mt-0.5 text-xs text-slate-500 dark:text-slate-400">Registered patient accounts</p>
              </div>
              <Icon icon="users" class="text-lg text-slate-300 dark:text-slate-600" />
            </div>

            <div v-if="loading" class="space-y-3 p-5">
              <div v-for="n in 4" :key="n" class="flex items-center gap-3">
                <div class="h-9 w-9 rounded-full skeleton" />
                <div class="flex-1 space-y-2"><div class="h-3 w-1/2 skeleton" /><div class="h-2.5 w-2/3 skeleton" /></div>
              </div>
            </div>
            <div v-else-if="patients.length" class="divide-y divide-slate-100 dark:divide-white/[0.05]">
              <div v-for="patient in patients.slice(0, 6)" :key="patient.email" class="flex items-center gap-3 px-5 py-3.5 sm:px-6">
                <AppAvatar :name="`${patient.firstName || ''} ${patient.lastName || ''}`.trim() || patient.email" size="sm" />
                <div class="min-w-0 flex-1">
                  <p class="truncate text-xs font-semibold text-ink-900 dark:text-slate-100">
                    {{ `${patient.firstName || ''} ${patient.lastName || ''}`.trim() || 'Patient' }}
                  </p>
                  <p class="truncate text-[11px] text-slate-500 dark:text-slate-400">{{ patient.email }}</p>
                </div>
                <span class="badge badge-info">Patient</span>
              </div>
            </div>
            <EmptyState v-else icon="users" title="No patients yet" description="Patient accounts will appear here as people register." />
          </section>
        </div>
      </div>
    </template>
    <DashboardPatient v-else />
  </div>
</template>

<script>
import AppAvatar from '@/components/ui/AppAvatar.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import PageHeader from '@/components/ui/PageHeader.vue'
import StatCard from '@/components/ui/StatCard.vue'
import DashboardPatient from '@/views/Patient/DashboardPatient.vue'
import { useTheme } from '@/composables/useTheme'
import { formatCurrency, formatNumber } from '@/utils/appointments'

const PALETTE = ['#14b8a6', '#06b6d4', '#38bdf8', '#818cf8', '#a78bfa', '#f59e0b']

export default {
  name: 'Dashboard',
  components: { AppAvatar, EmptyState, PageHeader, StatCard, DashboardPatient },
  setup() {
    const { isDark } = useTheme()
    return { isDark }
  },
  data() {
    return { loading: true }
  },
  computed: {
    isAdmin() {
      const role = (localStorage.getItem('user-role') || '').toLowerCase()
      return role.includes('admin') || role.includes('administrator')
    },
    users() {
      const users = this.$store.getters.getUser
      return Array.isArray(users) ? users : []
    },
    payments() {
      const revenue = this.$store.getters.getRevenue
      const list = revenue?.telemedicinePayments
      return Array.isArray(list) ? list : []
    },
    services() {
      const services = this.$store.getters.getServices
      return Array.isArray(services) ? services : []
    },
    paymentRows() {
      return this.payments.map((payment) => ({
        ...payment,
        serviceName: payment.service?.serviceName || this.serviceNameForAmount(payment.amountPaid),
      }))
    },
    patients() {
      return this.users.filter((user) => String(user.accountType || '').toLowerCase().includes('patient'))
    },
    physicians() {
      return this.users.filter((user) => String(user.accountType || '').toLowerCase().includes('physician'))
    },
    totalUsers() {
      return this.users.length
    },
    totalRevenue() {
      const revenue = this.$store.getters.getRevenue
      if (Number.isFinite(Number(revenue?.totalAmountPaid))) return Number(revenue.totalAmountPaid)
      return this.payments.reduce((sum, payment) => sum + Number(payment.amountPaid || 0), 0)
    },
    paidTransactions() {
      return this.payments.filter((payment) => payment.isPaid === true).length
    },
    unpaidTransactions() {
      return this.payments.length - this.paidTransactions
    },
    averageServiceCost() {
      if (!this.services.length) return 0
      const total = this.services.reduce((sum, service) => sum + Number(service.cost || 0), 0)
      return total / this.services.length
    },
    servicePricing() {
      return this.services
        .map((service) => {
          const name = service.serviceName || 'Consultation'
          return {
            name,
            label: name.replace(/\s*Consultation$/i, '') || name,
            value: Number(service.cost || 0),
          }
        })
        .sort((a, b) => b.value - a.value)
        .slice(0, 6)
    },
    pricingSeries() {
      return [{ name: 'Price', data: this.servicePricing.map((item) => item.value) }]
    },
    pricingOptions() {
      const dark = this.isDark
      return {
        chart: {
          type: 'bar',
          toolbar: { show: false },
          background: 'transparent',
          fontFamily: 'DM Sans, sans-serif',
          animations: { speed: 450 },
        },
        plotOptions: {
          bar: {
            horizontal: true,
            distributed: true,
            borderRadius: 7,
            barHeight: '48%',
          },
        },
        colors: PALETTE,
        dataLabels: {
          enabled: true,
          textAnchor: 'start',
          offsetX: 10,
          formatter: (value) => formatCurrency(value),
          style: { fontSize: '10px', fontWeight: 600, colors: [dark ? '#CBD5E1' : '#475569'] },
        },
        grid: {
          borderColor: dark ? 'rgba(148,163,184,0.12)' : '#E2E8F0',
          strokeDashArray: 4,
          xaxis: { lines: { show: true } },
          yaxis: { lines: { show: false } },
        },
        xaxis: {
          categories: this.servicePricing.map((item) => item.label),
          labels: { style: { colors: dark ? '#94A3B8' : '#64748B', fontSize: '11px' } },
          axisBorder: { show: false },
          axisTicks: { show: false },
        },
        yaxis: {
          labels: { style: { colors: dark ? '#CBD5E1' : '#475569', fontSize: '11px', fontWeight: 500 } },
        },
        legend: { show: false },
        tooltip: {
          theme: dark ? 'dark' : 'light',
          y: { formatter: (value) => formatCurrency(value) },
        },
      }
    },
    userMix() {
      const other = Math.max(0, this.users.length - this.patients.length - this.physicians.length)
      return [
        { label: 'Patients', value: this.patients.length, color: PALETTE[0] },
        { label: 'Physicians', value: this.physicians.length, color: PALETTE[2] },
        { label: 'Other accounts', value: other, color: PALETTE[4] },
      ].filter((item) => item.value > 0)
    },
    mixSeries() {
      return this.userMix.map((item) => item.value)
    },
    mixOptions() {
      const dark = this.isDark
      return {
        chart: { type: 'donut', background: 'transparent', fontFamily: 'DM Sans, sans-serif' },
        labels: this.userMix.map((item) => item.label),
        colors: this.userMix.map((item) => item.color),
        stroke: { width: 0 },
        dataLabels: { enabled: false },
        legend: { show: false },
        plotOptions: {
          pie: {
            donut: { size: '74%', labels: { show: false } },
          },
        },
        tooltip: {
          theme: dark ? 'dark' : 'light',
          y: { formatter: (value) => `${formatNumber(value)} accounts` },
        },
      }
    },
  },
  async mounted() {
    if (!this.isAdmin) {
      this.loading = false
      return
    }
    try {
      await Promise.all([
        this.$store.dispatch('fetchUsers'),
        this.$store.dispatch('fetchAllRevenues'),
        this.$store.dispatch('fetchServices'),
      ])
    } finally {
      this.loading = false
    }
  },
  methods: {
    formatCurrency,
    formatNumber,
    serviceNameForAmount(amount) {
      const match = this.services.find((service) => Number(service.cost) === Number(amount))
      return match?.serviceName || 'Consultation'
    },
  },
}
</script>
