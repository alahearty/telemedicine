<template>
  <div class="home">
    <Navbar />

    <main>
      <div class="hero">
        <article class="hero-content">
          <div class="hero-text-container">
            <h1>
              We're <span>determined</span> for your <span>better life.</span>
            </h1>
            <p>
              You can get the care you need 24/7 – be it online or in person.
              You will be treated by caring specialist doctors.
            </p>
            <button class="hero-cta">Make an appointment</button>
          </div>

          <div class="hero-img">
            <img src="@/assets/img/home-hero.png" />
          </div>
        </article>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script>
import Navbar from '@/components/home-landing/Navbar.vue'
import Footer from '@/components/home-landing/Footer.vue'

export default {
  name: 'Home',
  components: { Navbar, Footer },
}
</script>

<style>
:root {
  --max-width: 1080px;
  --max-width-md: var(--max-width-md);
}
body {
  overflow: auto;
}
.home {
  font-family: 'Mulish', sans-serif;
  height: 100vh;
  position: relative;
}

.home button {
  font-family: 'Mulish', sans-serif;
}

.home .hero {
  width: 100%;
  min-height: 100vh;
  background-image: url('@/assets/img/home-hero-bg1.png');
  background-repeat: no-repeat;
  background-size: cover;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  padding-inline: 3rem;
}
.home .hero-content {
  display: flex;
  max-width: var(--max-width);
  margin-inline: auto;
  align-items: center;
  justify-content: space-between;
  margin-top: 2rem;
}
.hero-content .hero-text-container {
  flex: 0.45;
}

.hero-content h1 {
  font-size: clamp(2rem, 5vw, 3rem);
  font-weight: 300;
  line-height: 120%;
  color: #283779;
  margin-bottom: 1rem;
}
.hero-content h1 span {
  font-weight: 700;
}
.hero-content p {
  font-size: 1.125rem;
  line-height: 130%;
  color: #1b71a1;
  margin-bottom: 2.5rem;
}
.hero-content .hero-cta {
  padding: 0.975rem 3.5rem;
  font-size: 1rem;
  background: #283779;
  color: #fff;
  border-radius: 50px;
  font-weight: 700;
}
.hero-content .hero-img {
  flex: 0.5;
}

@media (max-width: 800px) {
  .home .hero-content {
    max-width: var(--max-width-md);
    flex-direction: column-reverse;
    text-align: center;
    margin-top: 7rem;
  }
  .hero-content .hero-img {
    max-width: 100%;
  }
  .hero-content .hero-text-container {
    padding-block: 4rem;
  }
  .hero-content h1 {
    font-size: clamp(2rem, 6vw, 3rem);
  }
}
</style>
