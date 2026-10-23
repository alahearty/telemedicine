<template>
  <div>
    <template v-if="user.accountType === 'admin'">
    <div class="dashboard p-4">
      <nav class="flex mt-5" aria-label="Breadcrumb">
        <ol class="inline-flex items-center space-x-1 md:space-x-3">
          <li class="inline-flex items-center">
            <a
              href="#"
              class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
            >
              <svg class="mr-2 w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path
                  d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"
                ></path>
              </svg>
              Dashboard
            </a>
          </li>
        </ol>
      </nav>
      <!-- end nav -->
      <div class="mt-5 w-full">
        <h1 class="text-2xl text-gray-900 dark:text-gray-200 font-medium">
          Admin Dashboard
        </h1>
      </div>
      <!-- grid wrapper card -->
      <div
        class="wrapper-card grid lg:grid-cols-4 grid-cols-1 md:grid-cols-2 gap-2 mt-5"
      >
        <!-- card  -->
        <Card total="Rp.2 300 908" totalPayout="Total Payouts">
          <div class="bg-primary-100 dark:bg-primary-900/30 rounded-2xl w-14 h-14 flex items-center justify-center text-primary-600 dark:text-primary-400">
            <wallet-icon />
          </div>
        </Card>
        <Card total="256" totalPayout="Total Payins">
          <div class="bg-amber-100 dark:bg-amber-900/30 rounded-2xl w-14 h-14 flex items-center justify-center text-amber-600 dark:text-amber-400">
            <bag-icon />
          </div>
        </Card>
        <Card total="3569" totalPayout="Total Patients">
          <div class="bg-emerald-100 dark:bg-emerald-900/30 rounded-2xl w-14 h-14 flex items-center justify-center text-emerald-600 dark:text-emerald-400">
            <user-icon />
          </div>
        </Card>
        <Card total="7230" totalPayout="Total Visit">
          <div class="bg-violet-100 dark:bg-violet-900/30 rounded-2xl w-14 h-14 flex items-center justify-center text-violet-600 dark:text-violet-400">
            <trend-icon />
          </div>
        </Card>
        <!-- end card -->
      </div>
      <!-- end wrapper card -->
      <div class="mt-2 lg:flex block lg:gap-2">
        <div
          class="bg-white dark:bg-gray-800 p-5 w-full rounded-md box-border shadow"
        >
          <chart-component
            title="Total Services"
            subtitle=" your services chart per-year"
            total="27.9%"
            report=" Services Report"
          >
            <span class="float-right">
              <h2 class="text-green-500 -mt-12 flex">
                <span class="mr-2"> 87.9% </span
                ><span>
                  <Icon icon="akar-icons:arrow-up" />
                </span>
              </h2>
            </span>
            <br />
            <apexchart
              width="100%"
              height="380"
              type="area"
              :options="optionsArea"
              :series="seriesArea"
            ></apexchart>
          </chart-component>
        </div>

        <div class="card-modern p-6 lg:w-96 lg:mt-0 mt-4 w-full">
          <Partners />
        </div>
      </div>
      <div class="mt-2 lg:flex block lg:gap-2">
        <div class="chart">
          <chart-component
            title="5,780"
            subtitle="This Week New Patients "
            total="27.9%"
            report="Patient Report"
          >
            <apexchart
              width="100%"
              height="380"
              type="bar"
              :options="optionsBar"
              :series="seriesBar"
            ></apexchart>
          </chart-component>
        </div>
        <div class="chart">
          <chart-component
            title=" 355"
            subtitle="This Week Doctors"
            total="47.9%"
            report="Doctor Report"
          >
            <apexchart
              width="100%"
              height="380"
              type="area"
              :options="optionsBar"
              :series="seriesBar"
            ></apexchart>
          </chart-component>
        </div>
        <div class="chart">
          <chart-component
            title="475"
            subtitle="This Week User Signups"
            total=""
            report="User Report"
          >
            <apexchart
              width="100%"
              height="380"
              type="pie"
              :options="optionsDonut"
              :series="seriesDonut"
              class="p-3"
            ></apexchart>
          </chart-component>
        </div>
      </div>
      <div>
        <data-table
          :headers="transactionHeaders"
          :items="transactions"
          title="Latest Transactions"
          subtitle="This is a list of latest transactions"
        >
          <template #cell(statusTransaction)="{ data }">
            <span
              class="status"
              :class="{
                completed: data === 'completed',
                progress: data === 'progress',
              }"
            >
              {{ data }}
            </span>
          </template>
        </data-table>
      </div>
    </div>
    </template>
    <DashboardPatient v-else />
  </div>
</template>

<script>
import { Icon } from '@iconify/vue'
import Partners from '@/components/account-dashboard/Partners.vue'
import WalletIcon from '@/components/icons/WalletIcon.vue'
import BagIcon from '../../components/icons/BagIcon.vue'
import UserIcon from '@/components/icons/UserIcon.vue'
import TrendIcon from '@/components/icons/TrendIcon.vue'
import DataTable from '@/components/account-dashboard/DataTable.vue'
import ChartComponent from '@/components/account-dashboard/ChartComponent.vue'
import Card from '@/components/account-dashboard/Card.vue'
import DashboardPatient from '@/views/Patient/DashboardPatient.vue'

export default {
  name: 'Dashboard',
  data() {
    return {
      user: {
        accountType: (() => {
          const role = localStorage.getItem('user-role') || ''
          const r = role.toLowerCase()
          if (r.includes('admin') || r.includes('administrator')) return 'admin'
          if (r.includes('physician') || r.includes('doctor')) return 'physician'
          return 'user'
        })(),
      },
      optionsArea: {
        xaxis: {
          categories: [2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022],
        },
        fontFamily: 'Segoe UI, sans-serif',
        stroke: {
          curve: 'straight',
        },
        markers: {
          size: 0,
        },
        yaxis: {
          show: false,
        },
        fill: {
          type: 'gradient',
          gradient: {
            shadeIntensity: 1,
            opacityFrom: 0.7,
            opacityTo: 0.9,
            stops: [0, 90, 100],
          },
        },
      },

      chart: {
        fontFamily: 'lexend, sans-serif',
      },

      seriesArea: [],

      optionsBar: {
        chart: {
          toolbar: {
            show: false,
          },
          zoom: {
            enabled: false,
          },
          sparkline: {
            enabled: true,
          },
        },
        legend: {
          show: false,
        },
        xaxis: {
          show: false,
        },
        yaxis: {
          show: false,
        },
        colors: ['#059682', '#DC2626'],
        dataLabels: {
          enabled: false,
        },
        stroke: {
          curve: 'straight',
        },
      },

      seriesBar: [],

      optionsVisitor: {
        chart: {
          toolbar: {
            show: false,
          },
          zoom: {
            enabled: false,
          },
          sparkline: {
            enabled: true,
          },
        },
        legend: {
          show: false,
        },
        xaxis: {
          show: false,
        },
        yaxis: {
          show: false,
        },
        colors: ['#4f46e5'],
        dataLabels: {
          enabled: false,
        },
        fill: {
          type: 'gradient',
          gradient: {
            shadeIntensity: 1,
            opacityFrom: 0.7,
            opacityTo: 0.9,
            stops: [0, 90, 100],
          },
        },
        stroke: {
          curve: 'smooth',
        },
      },

      seriesVisitor: [],

      optionsDonut: {
        chart: {
          type: 'donut',
        },
        legend: false,
        dataLabels: {
          enabled: false,
        },
        labels: [],
      },
      seriesDonut: [20, 15, 63, 83],

      transactionHeaders: [
        {
          label: 'Transaction',
          key: 'transaction',
        },
        {
          label: 'Date & Time',
          key: 'datetime',
        },
        {
          label: 'Amount',
          key: 'amount',
        },
        {
          label: 'Status',
          key: 'statusTransaction',
        },
      ],

      transactions: [],
    }
  },
  components: {
    Icon,
    Partners,
    WalletIcon,
    BagIcon,
    UserIcon,
    TrendIcon,
    DataTable,
    ChartComponent,
    Card,
    DashboardPatient,
  },
  computed: {
    seriesBar() {
      const services = this.$store.getters.getServices
      return Array.isArray(services) && services.length ? [{ name: 'Services', data: services.map(s => s.id || 0) }] : [{ name: 'Services', data: [0] }]
    },
    seriesArea() {
      const revenue = this.$store.getters.getRevenue
      return Array.isArray(revenue) && revenue.length ? [{ name: 'Revenue', data: revenue.map(r => r.amountPaid ?? r.amount ?? 0) }] : [{ name: 'Revenue', data: [30, 40, 45, 50, 49, 60, 70, 91] }]
    },
    seriesVisitor() {
      const v = this.$store.getters.getVisitor
      return v?.data ? [{ name: v.name || 'Visitor', data: v.data }] : [{ name: 'Visitor', data: [30, 40, 45, 50, 49, 60, 70, 91] }]
    },
  },

  methods: {
    fetchServices() {
      this.$store.dispatch('fetchServices')
    },

    fetchRevenues() {
      this.$store.dispatch('fetchAllRevenues')
    },

    fetchVisitors() {
      this.$store.dispatch('fetchAllVisitor')
    },

    fetchTransactions() {
      this.$store.dispatch('fetchAllTransaction').then(() => {
        this.transactions = this.$store.state.transaction
      })
    },
    fetchUSERSs() {
      this.$store.dispatch('fetchUsers').then(() => {
        this.labels = this.$store.state.user
      })
    },
  },

  mounted() {
    if (this.user.accountType !== 'admin') return
    this.fetchServices()
    this.fetchRevenues()
    this.fetchTransactions()
    this.fetchVisitors()
    this.fetchUSERSs()
  },
  created() {
    // this.$emit(`update:layout`, AdminLayout)
  },
}
</script>

<style scoped>
.chart {
  @apply mt-2 bg-white dark:bg-slate-800 rounded-2xl shadow-soft border border-slate-100 dark:border-slate-700/50 p-6 w-full;
}
.status {
  @apply text-red-800 bg-red-300 px-3 py-1 rounded-md;
}

.status.completed {
  @apply text-green-800 bg-green-300;
}

.status.progress {
  @apply text-purple-800 bg-purple-300;
}
</style>
