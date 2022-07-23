<template>
  <section>
    <img class="bg-img" src="@/assets/img/dot-bg.png" alt="background" />
    <div class="inner">
      <div class="image-container">
        <img src="@/assets/img/home-promo-bg.png" alt="background" />
      </div>
      <div class="content">
        <h2>Eye Care with Top Professionals and In Budget.</h2>
        <p>
          We've built a healthcare system that puts your needs first. For us,
          there is nothing more important than the health of you and your loved
          ones.
        </p>
        <button>Learn more</button>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: 'Promo',
}
</script>

<style scoped>
section {
  position: relative;
  background-color: #1b71a1;
  padding-inline: var(--bg-padding-inline);
}
section .bg-img {
  position: absolute;
  bottom: 4rem;
  right: 4rem;
  opacity: 0.3;
}
.inner {
  padding-block: 3rem;
  width: 100%;
  max-width: var(--max-width);
  margin: auto;
  display: flex;
  gap: 2rem;
  align-items: center;
}
.inner > div {
  flex: 0.45;
}
.content h2 {
  font-weight: 700;
  font-size: clamp(1.7rem, 4vw, 2.1rem);
  line-height: 120%;
  color: #fff;
}
.content p {
  font-size: 1rem;
  line-height: 130%;
  color: #b2dded;
  margin-top: 2rem;
  margin-bottom: 3rem;
}
.content button {
  border: 1px solid #b2dded;
  border-radius: 50px;
  padding: 0.7em 2.25em;
  color: #283779;
  font-weight: 700;
  transition: all 250ms ease;
  background: #ffffff;
}
.content button:hover {
  background-color: #283779;
  color: #fff;
}
@media (max-width: 800px) {
  .inner {
    flex-direction: column;
    text-align: center;
  }
}
</style>
