<template>
  <main class="auth-wrapper">
    <div class="form-container card-modern">
      <div class="form-slot">
        <slot />
      </div>
      <div class="image-panel">
        <div class="image-overlay" />
        <div class="image-content">
          <h2 class="font-display font-bold text-3xl text-white mb-4">
            Healthcare, anywhere
          </h2>
          <p class="text-white/90 text-lg leading-relaxed">
            Connect with specialists 24/7. Your health, your schedule.
          </p>
        </div>
      </div>
    </div>
  </main>
</template>

<script>
export default {
  name: 'AuthLayout',
}
</script>

<style scoped>
.auth-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfeff 50%, #f0f9ff 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
}

.dark .auth-wrapper {
  background: linear-gradient(135deg, #0f172a 0%, #0c4a6e 50%, #134e4a 100%);
}

.form-container {
  width: 100%;
  max-width: 900px;
  min-height: 560px;
  display: flex;
  overflow: hidden;
  border-radius: 1.5rem;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.15);
}

.form-slot {
  flex: 0 0 55%;
  padding: 2.5rem 3rem;
  display: flex;
  flex-direction: column;
}

.image-panel {
  flex: 1;
  background: linear-gradient(135deg, #059682 0%, #04786a 50%, #065f55 100%);
  position: relative;
  display: flex;
  align-items: flex-end;
  padding: 2.5rem;
}

.image-overlay {
  position: absolute;
  inset: 0;
  background: url('@/assets/img/auth-layout-bg.jpg') center/cover;
  opacity: 0.15;
}

.image-content {
  position: relative;
  z-index: 1;
}

@media (max-width: 768px) {
  .image-panel {
    display: none;
  }

  .form-slot {
    flex: 1;
    padding: 2rem 1.5rem;
  }
}
</style>
