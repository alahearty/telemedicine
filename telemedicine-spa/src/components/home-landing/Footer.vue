<template>
  <footer>
    <section class="newsletter">
      <img class="img-bg" src="@/assets/img/footer-bg.png" />

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
      <img class="img-bg" src="@/assets/img/footer-dot-bg.png" />
      <div class="inner">
        <div class="promo-container">
          <TelemedicineLogo />
          <p>
            The world's most trusted <br />
            telehealth company.
          </p>
        </div>
        <div class="links-container">
          <ul>
            <li class="link-title">Departments</li>
            <li>Eye care</li>
            <li>Cardiac care</li>
            <li>Heart care</li>
          </ul>
          <ul>
            <li class="link-title">Membership</li>
            <li>Free trial</li>
            <li>Silver</li>
            <li>Premium</li>
          </ul>
          <ul>
            <li class="link-title">Customer care</li>
            <li>About us</li>
            <li>Contact us</li>
            <li>Get update</li>
          </ul>
        </div>
      </div>
    </section>
    <section class="copyright">
      <div class="inner">
        <p>All rights reserved &copy; <span>Telemedicine</span> 2022</p>
      </div>
    </section>
  </footer>
</template>

<script>
import TelemedicineLogo from '@/components/icons/TelemedicineHomeFooterLogo.vue'

export default {
  name: 'Footer',
  components: { TelemedicineLogo },
}
</script>

<style scoped>
footer {
  background: #283779;
  min-height: 400px;
  color: #b2dded;
}
footer section {
  padding-inline: var(--bg-padding-inline);
}
footer section .inner {
  max-width: var(--max-width);
  margin-inline: auto;
  width: 100%;
}
.newsletter,
.newsletter {
  position: relative;
}

.newsletter .img-bg {
  position: absolute;
  top: -8.125rem;
  right: 0;
  width: 25%;
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
  z-index: 1;
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
  position: relative;
  background-color: rgba(27, 113, 161, 0.75);
}
.footer-links .inner {
  display: flex;
  justify-content: space-between;
}

.footer-links .img-bg {
  position: absolute;
  top: -3.25rem;
  left: 0;
  opacity: 0.2;
}

.footer-links svg {
  color: #fff;
  height: 50px;
  width: 250px;
}

.footer-links .promo-container p {
  margin-top: 1.5rem;
  font-weight: 600;
}

.footer-links .links-container {
  width: 470px;
  display: flex;
  justify-content: space-between;
}
.footer-links ul .link-title {
  margin-bottom: 2rem;
  font-size: 1.2rem;
  font-weight: 700;
  color: #f0f4f9;
  cursor: default;
}

.footer-links li {
  margin-bottom: 0.5rem;
  font-weight: 600;
  cursor: pointer;
}

.copyright {
  padding-block: 2rem;
  text-align: center;
  color: #dbdbe9;
  font-weight: 700;
  font-size: 0.875rem;
}
.copyright span {
  color: #0dafca;
}

@media (max-width: 900px) {
  .newsletter .inner {
    flex-direction: column;
    gap: 2.5rem;
  }
  .footer-links .inner {
    flex-direction: column;
    gap: 5rem;
  }
  .footer-links .links-container {
    width: 100%;
    max-width: 600px;
  }
}
@media (max-width: 520px) {
  .footer-links .links-container {
    flex-direction: column;
    gap: 1.5rem;
  }
  .newsletter-btn-container {
    flex-direction: column;
  }
  .newsletter-btn-container button {
    max-width: 170px;
  }
  .footer-links .inner {
    gap: 3rem;
  }
  .footer-links svg {
    height: 30px;
    width: 150px;
  }
  .footer-links ul .link-title {
    margin-bottom: 0.7rem;
  }
}
</style>
