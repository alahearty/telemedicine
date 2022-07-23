<template>
  <header>
    <h2>{{ title }}</h2>
    <span class="faded-title"> {{ title }} </span>
  </header>
</template>

<script>
export default {
  name: 'SectionHeader',
  props: ['title'],
}
</script>

<style scoped>
header {
  display: grid;
  place-items: center;
  position: relative;
}
header h2 {
  font-size: clamp(1.7rem, 6vw, 2.3rem);
  text-transform: uppercase;
  font-weight: 700;
  text-align: center;
  color: #283779;
}
header .faded-title {
  font-size: clamp(3.5rem, 6vw, 7rem);
  color: rgba(178, 221, 237, 0.15);
  text-transform: uppercase;
  position: absolute;
  z-index: -1;
  font-weight: 700;
}
</style>
