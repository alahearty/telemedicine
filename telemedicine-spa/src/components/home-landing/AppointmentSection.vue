<template>
  <section id="appointment">
    <SectionHeader title="Appointment" />

    <div class="inner">
      <div class="img-container">
        <img src="@/assets/img/appointment-bg.png" alt="background" />
      </div>
      <form>
        <div class="inputs-container">
          <input type="text" placeholder="Name" autocapitalize="word" />
          <input type="tel" placeholder="Phone" />
        </div>
        <div class="inputs-container">
          <input type="email" placeholder="Email" />
          <select>
            <option value="category1">Category 1</option>
            <option value="category2">Category 2</option>
            <option value="category3">Category 3</option>
          </select>
        </div>
        <div class="inputs-container">
          <textarea cols="30" rows="10" placeholder="Message"></textarea>
        </div>
        <button class="submit-btn">Submit</button>
      </form>
    </div>
  </section>
</template>

<script>
import SectionHeader from '@/components/home-landing/SectionHeader.vue'

export default {
  name: 'Appointment',
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
}

.inner .inputs-container {
  display: flex;
  gap: 0.7rem;
  margin-top: 1rem;
}

form {
  z-index: 1;
}

form input,
form select,
form textarea {
  flex: 1;
  width: 100%;

  border: 1px solid #b2dded;
  border-radius: 1rem;
  padding: 0.7rem 1rem;
  font-size: 1rem;
  outline: none;
}

form select {
  color: #919191;
}

form .submit-btn {
  width: 100%;
  padding: 0.975rem 3.5rem;
  font-size: 1rem;
  background: #283779;
  color: #fff;
  border-radius: 50px;
  font-weight: 700;
  margin-top: 1rem;
}

form input:focus,
form textarea:focus {
  background-color: rgba(178, 221, 237, 0.24);
  border: 1px solid #1b71a1;
}

form,
.inner .img-container {
  flex: 0.5;
}

@media (max-width: 1024px) {
  .inner {
    flex-direction: column;
  }
  .inner .img-container {
    display: grid;
    place-items: center;
  }
}

@media (max-width: 628px) {
  header .faded-title {
    display: none;
  }
}
</style>
