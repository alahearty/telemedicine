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

<style>
:root {
  --max-width: 1200px;
  --bg-padding-inline: 1.5rem;
}

body {
  overflow: auto;
}

body::-webkit-scrollbar {
  width: 10px;
}

body::-webkit-scrollbar-thumb {
  background: #94a3b8;
  border-radius: 5px;
}

body::-webkit-scrollbar-thumb:hover {
  background: #64748b;
}

body::-webkit-scrollbar-track {
  background: #f1f5f9;
}

.home {
  font-family: 'DM Sans', system-ui, sans-serif;
}

main > section {
  min-height: 60vh;
  padding-top: 5rem;
  padding-bottom: 4rem;
}

@media (max-width: 520px) {
  :root {
    --bg-padding-inline: 1rem;
  }
}
</style>
