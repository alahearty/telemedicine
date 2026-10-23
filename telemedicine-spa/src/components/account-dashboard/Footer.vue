<template>
  <div class="p-4">
    <div class="bg-white dark:bg-gray-800 w-full p-6 font-lexend shadow-md rounded-md mt-5">
      <div class="wrapper-link flex flex-wrap gap-4 text-sm text-gray-500 dark:text-gray-400">
        <router-link to="/dashboard" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
          Dashboard
        </router-link>
        <router-link to="/dashboard/book-service" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
          Book Appointment
        </router-link>
        <router-link to="/dashboard/hospital" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
          Hospitals
        </router-link>
        <router-link to="/dashboard/account" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
          Account
        </router-link>
      </div>
    </div>
    <footer class="w-full box-border p-5 mt-3 text-center">
      <p class="text-sm dark:text-gray-400">
        Telemedicine Platform &copy; {{ new Date().getFullYear() }}
      </p>
    </footer>
  </div>
</template>
