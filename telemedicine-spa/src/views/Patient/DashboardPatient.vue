<template>
  <div class="patient-dashboard">
    <section>
      <Breadcrumb :home="home" :model="items">
        <template #item="{ item }">
          <a @click="handleRouteClick(item.url)">{{ item.label }}</a>
        </template>
      </Breadcrumb>
    </section>
    <main>
      <router-view />
    </main>
  </div>
</template>

<script>
import Layout from '@/Layout/AdminLayout.vue'
export default {
  name: 'DashboardPatient',
  data() {
    return {
      home: { icon: 'pi pi-home', to: '/DashboardPatient' },
      items: [
        { label: 'Update Record', url: '/update-record' },
        { label: 'Book service', url: '/book-service' },
        { label: 'Proceed Payment', url: '/payment' },
        { label: 'Chat Session', url: '/chat' },
        { label: 'Doctor Feedback', url: '/feedback' },
      ],
    }
  },
  components: {
    Layout,
  },
  methods: {
    handleRouteClick(link) {
      this.$router.push(link)
    },
    fetchServices() {
      this.$store.dispatch('fetchfetchServices')
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

  //   mounted() {
  //     this.fetchServices()
  //     this.fetchRevenues()
  //     this.fetchTransactions()
  //     this.fetchVisitors()
  //     this.fetchUSERSs()
  //   },

  created() {
    this.$emit(`update:layout`, Layout)
  },
}
</script>

<style scoped>
.patient-dashboard {
  display: flex;
  flex-direction: column;
}
main {
  padding: 1rem 1.5rem;
  flex: 1;
}
a {
  cursor: pointer;
}
</style>
