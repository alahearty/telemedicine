<template>
  <div class="home">
    <Navbar />

    <main>
      <Hero />
      <Departments />
      <Promo />
      <AboutUs />
      <Service />
    </main>

    <Footer />
  </div>
</template>

<script>
import Navbar from '@/components/home-landing/Navbar.vue'
import Footer from '@/components/home-landing/Footer.vue'
import Service from '@/components/home-landing/ServicesSection.vue'
import AboutUs from '@/components/home-landing/AboutUsSection.vue'
import Departments from '@/components/home-landing/Departments.vue'
import Hero from '@/components/home-landing/HeroSection.vue'
import Promo from '@/components/home-landing/PromoSection.vue'

export default {
  name: 'Home',
  components: {
    Navbar,
    Footer,
    Service,
    Hero,
    AboutUs,
    Departments,
    Promo,
  },
}
</script>

<style scoped>
:root {
  --max-width: 1080px;
  --max-width-md: var(--max-width-md);
  --bg-padding-inline: 3rem;
}
body {
  overflow: auto;
}
body::-webkit-scrollbar {
  width: 12px;
}

body::-webkit-scrollbar-thumb {
  background-color: rgb(177, 177, 177);
  border: 2px solid #fff;
  border-radius: 8px;
}
body::-webkit-scrollbar-track {
  background: #fff;
}
body::-webkit-scrollbar-thumb:hover {
  background-color: rgb(124, 124, 124);
}
.home {
  font-family: 'Mulish', sans-serif;
  height: 100vh;
  position: relative;
}

main > section {
  min-height: 100vh;
  padding-top: 5rem;
  padding-bottom: 3rem;
}

.home button {
  font-family: 'Mulish', sans-serif;
}

@media (max-width: 520px) {
  :root {
    --bg-padding-inline: 1rem;
  }
}
</style>
