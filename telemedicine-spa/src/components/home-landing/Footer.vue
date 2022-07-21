<template>
  <footer>
    <section class="newsletter">
      <div class="inner">
        <div>
          <h2>Get an update every week</h2>
          <p>
            Livedoc was created in order to improve the patient experience.
            Providing world-class tests, and a wide range of other services.
          </p>
        </div>

        <form>
          <h3>Subscribe to newsletter</h3>
          <div class="newsletter-btn-container">
            <input type="email" placeholder="Email" />
            <button class="subscribe-btn">Subscribe</button>
          </div>
        </form>
      </div>
    </section>
    <section class="footer-links">
      <div class="inner"></div>
    </section>
    <section class="copyright">
      <div class="inner"></div>
    </section>
  </footer>
</template>

<script>
export default {
  name: 'Footer',
}
</script>

<style scoped>
footer {
  background: #283779;
  min-height: 400px;
  color: #b2dded;
}
footer section {
  padding-inline: 3rem;
}
footer section .inner {
  max-width: var(--max-width);
  margin-inline: auto;
  width: 100%;
}
.newsletter,
.footer-links {
  padding-block: 7rem;
}
.newsletter .inner {
  display: flex;
  gap: 1.5rem;
}
.newsletter .inner > div,
.newsletter .inner > form {
  flex: 0.5;
}
.newsletter h2 {
  font-weight: 700;
  font-size: clamp(1.7rem, 4vw, 2.2rem);
  line-height: 120%;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 1rem;
}
.newsletter p {
  line-height: 120%;
  color: #b2dded;
}
.newsletter h3 {
  font-size: 1.15rem;
  text-transform: uppercase;
  font-weight: 700;
  margin-bottom: 1rem;
}
.newsletter-btn-container {
  display: flex;
  gap: 1rem;
}
.newsletter input {
  border: 1px solid rgba(240, 244, 249, 0.95);
  color: #b2dded;
  background-color: transparent;
  padding: 1em;
  border-radius: 1rem;
  flex: 1;
}
.newsletter button {
  background-color: #fff;
  padding: 1em;
  border: 1px solid rgba(240, 244, 249, 0.95);
  color: #464558;
  font-weight: 700;
  border-radius: 1rem;
  flex: 0.5;
}

.footer-links {
  background-color: rgba(27, 113, 161, 0.75);
}

@media (max-width: 1024px) {
  .newsletter .inner {
    flex-direction: column;
    gap: 2.5rem;
  }
}
</style>
