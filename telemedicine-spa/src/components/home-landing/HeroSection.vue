<template>
  <section id="hero" class="hero-section">
    <article class="hero-content">
      <div class="hero-text-container">
        <h1 class="font-display font-bold text-slate-800 leading-tight">
          Healthcare, <span class="text-primary-600">anywhere</span>
        </h1>
        <p class="hero-subtitle">
          Connect with specialists 24/7. Get the care you need online or in person—treated by caring doctors who put you first.
        </p>
        <a href="#service">
          <button class="hero-cta btn-primary">
            Explore services
            <svg class="w-5 h-5 ml-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3" />
            </svg>
          </button>
        </a>
      </div>
      <div class="hero-img">
        <img src="@/assets/img/home-hero.png" alt="Healthcare illustration" />
      </div>
    </article>
  </section>
</template>

<script>
export default {}
</script>

<style scoped>
.hero-section {
  width: 100%;
  min-height: 90vh;
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfeff 40%, #f0f9ff 100%);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 6rem 1.5rem 4rem;
}

.hero-content {
  display: flex;
  max-width: 1200px;
  margin-inline: auto;
  align-items: center;
  justify-content: space-between;
  gap: 3rem;
}

.hero-text-container {
  flex: 0 0 48%;
}

.hero-text-container h1 {
  font-size: clamp(2.25rem, 5vw, 3.5rem);
  margin-bottom: 1.25rem;
}

.hero-subtitle {
  font-size: 1.125rem;
  line-height: 1.7;
  color: #475569;
  margin-bottom: 2rem;
}

.hero-cta {
  display: inline-flex;
  align-items: center;
}

.hero-img {
  flex: 0 0 48%;
}

.hero-img img {
  width: 100%;
  max-width: 480px;
  height: auto;
}

@media (max-width: 900px) {
  .hero-content {
    flex-direction: column-reverse;
    text-align: center;
  }

  .hero-text-container {
    flex: 1;
  }

  .hero-img {
    flex: 1;
  }

  .hero-img img {
    max-width: 360px;
  }
}
</style>
