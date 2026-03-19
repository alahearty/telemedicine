import { createStore } from 'vuex'
import {
  getAnalyticsUsers,
  getAnalyticsServices,
  getAnalyticsPayments,
  getAnalyticsVisitors,
  getAnalyticsTransactions,
  getAnalyticsReports,
} from '@/api'

export default createStore({
  state: {
    services: [],
    revenue: [],
    visitor: [],
    transaction: [],
    report: [],
    user: [],
  },
  getters: {
    getServices: (state) => state.services,
    getRevenue: (state) => state.revenue,
    getVisitor: (state) => state.visitor,
    getTransaction: (state) => state.transaction,
    getReport: (state) => state.report,
    getUser: (state) => state.user,
  },
  actions: {
    async fetchUsers({ commit }) {
      try {
        const data = await getAnalyticsUsers()
        commit('SET_USERS', Array.isArray(data) ? data : (data || []))
      } catch (error) {
        console.error('fetchUsers error:', error)
        commit('SET_USERS', [])
      }
    },
    async fetchServices({ commit }) {
      try {
        const data = await getAnalyticsServices()
        commit('SET_SERVICE', Array.isArray(data) ? data : (data || []))
      } catch (error) {
        console.error('fetchServices error:', error)
        commit('SET_SERVICE', [])
      }
    },
    async fetchAllRevenues({ commit }) {
      try {
        const data = await getAnalyticsPayments()
        commit('SET_REVENUE', Array.isArray(data) ? data : (data || []))
      } catch (error) {
        console.error('fetchAllRevenues error:', error)
        commit('SET_REVENUE', [])
      }
    },
    async fetchAllVisitor({ commit }) {
      try {
        const data = await getAnalyticsVisitors()
        commit('SET_VISITOR', data || {})
      } catch (error) {
        console.error('fetchAllVisitor error:', error)
        commit('SET_VISITOR', {})
      }
    },
    async fetchAllTransaction({ commit }) {
      try {
        const data = await getAnalyticsTransactions()
        commit('SET_TRANSACTION', Array.isArray(data) ? data : (data || []))
      } catch (error) {
        console.error('fetchAllTransaction error:', error)
        commit('SET_TRANSACTION', [])
      }
    },
    async fetchAllReport({ commit }) {
      try {
        const data = await getAnalyticsReports()
        commit('SET_REPORT', Array.isArray(data) ? data : (data || []))
      } catch (error) {
        console.error('fetchAllReport error:', error)
        commit('SET_REPORT', [])
      }
    },
  },
  mutations: {
    SET_USERS(state, user) {
      state.user = user
    },
    SET_SERVICE(state, services) {
      state.services = services
    },
    SET_REVENUE(state, revenue) {
      state.revenue = revenue
    },
    SET_VISITOR(state, visitor) {
      state.visitor = visitor
    },
    SET_TRANSACTION(state, transaction) {
      state.transaction = transaction
    },
    SET_REPORT(state, report) {
      state.report = report
    },
  },
})
