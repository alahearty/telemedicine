<template>
  <footer class="footer">
    <section class="newsletter">
      <div class="inner">
        <div>
          <h2 class="font-display font-bold text-white text-2xl mb-3">
            Stay updated
          </h2>
          <p class="text-white/85 leading-relaxed">
            Telemedicine was created to improve the patient experience with world-class care and a wide range of services.
          </p>
        </div>
        <form class="newsletter-form">
          <h3 class="font-semibold text-white/90 text-sm uppercase tracking-wider mb-3">Newsletter</h3>
          <div class="newsletter-btn-container">
            <input type="email" placeholder="Your email" />
            <button type="button" class="subscribe-btn">Subscribe</button>
          </div>
        </form>
      </div>
    </section>
    <section class="footer-links">
      <div class="inner">
        <div class="promo-container">
          <TelemedicineLogo />
          <p class="mt-4 font-medium text-white/90">
            The world's most trusted<br />telehealth company.
          </p>
        </div>
        <div class="links-container">
          <ul>
            <li class="link-title">Departments</li>
            <li>Eye care</li>
            <li>Cardiac care</li>
            <li>Neurology</li>
          </ul>
          <ul>
            <li class="link-title">Membership</li>
            <li>Free trial</li>
            <li>Silver</li>
            <li>Premium</li>
          </ul>
          <ul>
            <li class="link-title">Support</li>
            <li>About us</li>
            <li>Contact</li>
            <li>FAQ</li>
          </ul>
        </div>
      </div>
    </section>
    <section class="copyright">
      <div class="inner">
        <p>© 2024 <span class="text-primary-300">Telemedicine</span>. All rights reserved.</p>
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
.footer {
  background: linear-gradient(135deg, #065f55 0%, #04786a 50%, #059682 100%);
  color: #ccfbf1;
}

.footer section {
  padding-inline: var(--bg-padding-inline, 1.5rem);
}

.footer .inner {
  max-width: 1200px;
  margin-inline: auto;
}

.newsletter {
  padding: 5rem 0 4rem;
}

.newsletter .inner {
  display: flex;
  gap: 3rem;
  align-items: flex-start;
}

.newsletter-form input {
  border: 1px solid rgba(255, 255, 255, 0.3);
  background: rgba(255, 255, 255, 0.1);
  color: white;
  padding: 0.875rem 1rem;
  border-radius: 12px;
  flex: 1;
}

.newsletter-form input::placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.newsletter-btn-container {
  display: flex;
  gap: 0.75rem;
}

.subscribe-btn {
  background: white;
  color: #04786a;
  padding: 0.875rem 1.5rem;
  border-radius: 12px;
  font-weight: 700;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

.subscribe-btn:hover {
  background: #ecfdf8;
  transform: translateY(-1px);
}

.footer-links {
  padding: 4rem 0;
  background: rgba(0, 0, 0, 0.15);
}

.footer-links .inner {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.footer-links svg {
  color: white;
  height: 40px;
  width: 200px;
}

.links-container {
  display: flex;
  gap: 4rem;
}

.link-title {
  font-weight: 700;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: white;
  margin-bottom: 1rem;
}
.link-title + li {
  margin-top: 0;
}

.footer-links li {
  margin-bottom: 0.5rem;
  cursor: pointer;
  opacity: 0.9;
  transition: opacity 0.2s;
}

.footer-links li:hover {
  opacity: 1;
}

.copyright {
  padding: 1.5rem 0;
  text-align: center;
  font-size: 0.875rem;
  opacity: 0.8;
}

@media (max-width: 900px) {
  .newsletter .inner {
    flex-direction: column;
    gap: 2rem;
  }

  .footer-links .inner {
    flex-direction: column;
    gap: 3rem;
  }

  .links-container {
    flex-wrap: wrap;
    gap: 2rem;
  }
}

@media (max-width: 520px) {
  .newsletter-btn-container {
    flex-direction: column;
  }

  .subscribe-btn {
    width: 100%;
  }
}
</style>
