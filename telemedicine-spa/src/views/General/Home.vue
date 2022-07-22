<template>
  <div class="home">
    <Navbar />

    <main>
      <Hero />
      <AboutUs />
      <Appointment />
    </main>

    <Footer />
  </div>
</template>

<script>
import Navbar from '@/components/home-landing/Navbar.vue'
import Footer from '@/components/home-landing/Footer.vue'
import Appointment from '@/components/home-landing/AppointmentSection.vue'
import AboutUs from '@/components/home-landing/AboutUsSection.vue'
import Hero from '@/components/home-landing/HeroSection.vue'

export default {
  name: 'Home',
  components: { Navbar, Footer, Appointment, Hero, AboutUs },
}
</script>

<style>
:root {
  --max-width: 1080px;
  --max-width-md: var(--max-width-md);
  --bg-padding-inline: 3rem;
}
body {
  overflow: auto;
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
