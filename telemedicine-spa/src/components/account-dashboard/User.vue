<template>
    <section>
        <Breadcrumb :home="home" :model="items" />
    </section>
    <slot></slot>
</template>
<script>
import Layout from '@/Layout/AdminLayout.vue'
export default {
  data() {
    return {
        home: {icon: 'pi pi-home', to: '/DashboardPatient'},
        items: [
            {label: 'Apply Services'},
            {label: 'Proceed Payment'},
            {label: 'Chat Session'},
            {label: 'Dcotor Feedback'},
        ]
    }
  },

 methods: {
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
    this.$emit(`update:layout`,Layout);
  },
}
</script>