<template>
  <main class="auth-wrapper">
    <div class="form-container">
      <slot />
      <div class="image-bg" />
    </div>
  </main>
</template>

<script>
export default {
  name: 'AuthLayout',
}
</script>

<style>
main.auth-wrapper {
  min-height: 100vh;
  background-color: #eee;
  display: grid;
  place-items: center;
  font-family: 'Mulish', sans-serif;
  padding-inline: 1.5rem;
}
.form-container {
  width: 100%;
  max-width: 700px;
  height: 520px;
  overflow-y: scroll;
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 3px 3px 16px 0px rgba(172, 172, 172, 0.5);
  display: flex;
  overflow: hidden;
}
.form-container button {
  font-family: 'Mulish', sans-serif;
  cursor: pointer;
}
.form-container.reverse {
  flex-direction: row-reverse;
}
.form-container form {
  flex: 0.55;
  padding: 1.5rem 3rem;
  height: 100%;
  display: flex;
  flex-direction: column;
  /* overflow-y: scroll; */
}

.form-container h1 {
  font-family: 'Rokkitt', serif;
  font-weight: 700;
  font-size: 2.25rem;
  color: #161843;
}

.form-container p {
  font-size: 1rem;
  color: #161843;
}

.form-container .inputs-container {
  margin-top: 1rem;
}

.form-container .image-bg {
  flex: 0.45;
  background-color: #046586;
  background-image: url('@/assets/img/auth-layout-bg.jpg');
  background-size: cover;
  background-repeat: no-repeat;
  background-position: center;
}

.form-container input,
.form-container select {
  width: 100%;
  padding: 0.5em 1em;
  font-size: 0.875rem;
  margin-bottom: 0.7rem;
  border: 1px solid #cad6e4;
  border-radius: 5px;
}

.form-container select {
  padding-inline: 0.7em;
  color: #00000090;
}

.buttons-container {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.buttons-container a {
  font-weight: 400;
  font-size: 0.875rem;
  color: #0c6787;
  font-weight: 700;
}

.submit-button {
  padding: 0.7em 1.5em;
  background-color: rgb(4, 101, 134);
  color: #fff;
  font-size: 0.875rem;
  border-radius: 5px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.3rem;
  box-shadow: 3px 11px 17px -10px rgba(4, 101, 134, 0.75);
}
.submit-button:hover {
  box-shadow: 3px 11px 17px -10px rgba(4, 101, 134, 0.75) inset;
}

.divider {
  display: block;
  width: 100%;
  padding: 1rem;
  border-bottom: 1px solid #cad6e4;
}

p.signin-options-title {
  font-size: 0.75rem;
  color: #838a9e;
  text-align: center;
  margin-block: 1rem;
}

.signin-options-container {
  display: flex;
  gap: 1rem;
}
.signin-options-container button {
  flex: 0.5;
  border: 1px solid #cad6e4;
  padding: 0.5rem;
  font-size: 0.875rem;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  transition: background-color 250ms ease;
}
.signin-options-container button:hover {
  background-color: #eee;
}
.signin-options-container button svg {
  width: 25px;
  height: 25px;
}
.alt-sign-in {
  margin-top: auto;
}
.alt-sign-in p {
  font-size: 0.875rem;
  text-align: center;
}
.alt-sign-in a {
  color: #046586;
  text-decoration: underline;
}

@media (max-width: 550px) {
  .form-container form {
    padding: 1.5rem 1.5rem;
    flex: 1;
  }
  .form-container .image-bg {
    flex: 0;
  }
  .submit-button {
    padding: 0.5rem 1rem;
  }
}

@media (max-width: 320px) {
  .form-container form {
    overflow-y: scroll;
  }
  .buttons-container {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
    margin-bottom: 1rem;
  }
  .buttons-container .submit-button {
    margin-left: 0;
  }
  .signin-options-container {
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
  }
  p.signin-options-title {
    margin-block: 0.5rem;
  }
}
</style>
