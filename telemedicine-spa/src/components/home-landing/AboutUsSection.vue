<template>
  <section id="about">
    <SectionHeader title="About us" />

    <div class="inner">
      <div class="content">
        <h2>We are developing a healthcare system around you</h2>
        <p>
          We think that everyone should have easy access to excellent
          healthcare. Our aim is to make the procedure as simple as possible for
          our patients and to offer treatment no matter where they are — in
          person or at their convenience.
        </p>
        <button>Learn more</button>
      </div>
      <div class="img-container">
        <img src="@/assets/img/home-about-bg.png" alt="background" />
      </div>
    </div>
  </section>
</template>

<script>
import SectionHeader from '@/components/home-landing/SectionHeader.vue'

export default {
  name: 'AboutUsSection',
  components: { SectionHeader },
}
</script>

<style scoped>
section {
  display: flex;
  flex-direction: column;
  padding-inline: var(--bg-padding-inline);
}
.inner {
  padding-block: 3rem;
  width: 100%;
  max-width: var(--max-width);
  margin: auto;
  min-height: 400px;
  display: flex;
  gap: 2rem;
  align-items: center;
}
.inner > div {
  flex: 0.5;
}
.content h2 {
  font-weight: 700;
  font-size: clamp(1.7rem, 4vw, 2.1rem);
  line-height: 120%;
  color: #283779;
}
.content p {
  font-size: 1rem;
  line-height: 130%;
  color: #1b71a1;
  margin-top: 2rem;
  margin-bottom: 3rem;
}
.content button {
  border: 1px solid #283779;
  border-radius: 50px;
  padding: 0.7em 2.25em;
  color: #283779;
  font-weight: 700;
  transition: all 250ms ease;
}
.content button:hover {
  background-color: #283779;
  color: #fff;
}
@media (max-width: 1024px) {
  .inner {
    flex-direction: row-reverse;
  }
}
@media (max-width: 800px) {
  .inner {
    flex-direction: column-reverse;
    text-align: center;
  }
}
</style>
