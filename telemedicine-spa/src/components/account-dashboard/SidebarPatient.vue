<template>
  <nav class="sidebar">
    <div class="sidebar-head">
      <router-link to="/dashboard" class="logo-link">
        <span class="logo-text font-display font-bold text-slate-800 dark:text-slate-100">Telemedicine</span>
        <span class="logo-dot text-primary-500">.</span>
      </router-link>
      <button class="lg:hidden close-btn" @click="$emit('close')" aria-label="Close menu">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
    <div class="sidebar-list">
      <p class="sidebar-label">Patient Menu</p>
      <div class="nav-items">
        <router-link to="/dashboard" exact class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:dashboard" /></span>
          <span>Dashboard</span>
        </router-link>
        <router-link to="/dashboard/account" exact class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:user" /></span>
          <span>Account</span>
        </router-link>
        <router-link to="/dashboard/book-service" exact class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:calendar" /></span>
          <span>Book Appointment</span>
        </router-link>
        <router-link to="/dashboard/appointments" class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:calendar-check" /></span>
          <span>My Appointments</span>
        </router-link>
        <router-link to="/dashboard/doctors" class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:user-account" /></span>
          <span>Find a Doctor</span>
        </router-link>
        <router-link to="/dashboard/update-record" exact class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:user-detail" /></span>
          <span>Update Record</span>
        </router-link>
        <router-link to="/dashboard/payment" exact class="nav-item">
          <span class="nav-icon"><Icon icon="bxs:credit-card" /></span>
          <span>Payment</span>
        </router-link>
      </div>
    </div>
  </nav>
</template>

<script>
export default { name: 'SidebarPatient' }
</script>

<style scoped>
.sidebar { width: 280px; min-height: 100vh; background: white; border-right: 1px solid #e2e8f0; display: flex; flex-direction: column; }
.dark .sidebar { background: #0f172a; border-color: #1e293b; }
.sidebar-head { padding: 1.5rem; display: flex; align-items: center; justify-content: space-between; }
.logo-link { display: flex; align-items: baseline; text-decoration: none; }
.logo-text { font-size: 1.25rem; }
.logo-dot { font-size: 1.5rem; }
.close-btn { display: none; padding: 0.5rem; color: #64748b; background: transparent; border: none; cursor: pointer; border-radius: 8px; }
.sidebar-list { padding: 0 1rem 1.5rem; flex: 1; }
.sidebar-label { font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: #94a3b8; margin-bottom: 0.75rem; padding: 0 0.75rem; }
.nav-items { display: flex; flex-direction: column; gap: 0.25rem; }
.nav-item { display: flex; align-items: center; gap: 0.75rem; padding: 0.75rem 1rem; border-radius: 12px; font-weight: 500; color: #475569; text-decoration: none; transition: all 0.2s ease; }
.dark .nav-item { color: #94a3b8; }
.nav-item:hover { background: #f1f5f9; color: #04786a; }
.dark .nav-item:hover { background: #1e293b; color: #34d3b4; }
.nav-item.router-link-active { background: #ecfdf8; color: #04786a; }
.dark .nav-item.router-link-active { background: rgba(16, 185, 154, 0.15); color: #34d3b4; }
.nav-icon { font-size: 1.25rem; opacity: 0.9; }
@media (max-width: 1024px) { .close-btn { display: block; } }
</style>
