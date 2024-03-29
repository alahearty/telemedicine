import { createStore } from 'vuex'
import axios from 'axios'

const BASE_URL = "https://localhost:7231/api/Analytics/";

export default createStore({
    state: {
        services: [],
        revenue: [],
        visitor: [],
        transaction: [],
        report: [],
        user: []
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
            const data = await axios.get(`${BASE_URL}/User`)
              commit('SET_USERS', data.data)
            }
            catch (error) {
                alert(error)
                console.log(error)
            }
        },
         async fetchServices({ commit }) {
          try {
            const data = await axios.get(`${BASE_URL}/Service`)
              commit('SET_SERVICE', data.data)
            }
            catch (error) {
                alert(error)
                console.log(error)
            }
        },
         async fetchAllRevenues({ commit }) {
          try {
            const data = await axios.get(`${BASE_URL}/Revenue`)
              commit('SET_REVENUE', data.data)
            }
            catch (error) {
                alert(error)
                console.log(error)
            }
        },
        
        async fetchAllVisitor({ commit }) {
          return new Promise((resolve, reject) => {
            axios.get(`${BASE_URL}/Visitor`)
              .then(() => {
                commit('SET_VISITOR', data.data);
                resolve(data.data)
              })
              .catch(error => {
                reject(error)
              })
          })
        },
        
        async fetchAllTransaction({ commit }) {
          try {
            const data = await axios.get(`${BASE_URL}/Transaction`)
              commit('SET_TRANSACTION', data.data)
            }
            catch (error) {
                alert(error)
                console.log(error)
            }
        }, 
        async fetchAllReport({ commit }) {
          try {
            const data = await axios.get(`${BASE_URL}/Report`)
              commit('SET_REPORT', data.data)
            }
            catch (error) {
                alert(error)
                console.log(error)
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
        state.visitor = visitor;
        console.log("Store visitor", state.visitor);
      },
      SET_TRANSACTION(state, transaction) {
        state.transaction = transaction;
        console.log("Store transaction", state.transaction);
      },
      SET_REPORT(state, report) {
        state.report = report;
        console.log("Store report", state.report);
      },
    }
})
