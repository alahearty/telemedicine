<template>
  <section id="departments">
    <SectionHeader title="Departments" />

    <div class="inner">
      <div class="department">
        <BrainIcon />
        <span>Neurology</span>
      </div>
      <div class="department">
        <EyeIcon />
        <span>Eye care</span>
      </div>
      <div class="department">
        <CardiacIcon />
        <span>Cardiac care</span>
      </div>
      <div class="department">
        <HeartIcon />
        <span>Heart care</span>
      </div>
      <div class="department">
        <OsteoporosisiIcon />
        <span>Osteoporosisi</span>
      </div>
      <div class="department">
        <EarIcon />
        <span>ENT</span>
      </div>
    </div>
  </section>
</template>

<script>
import SectionHeader from '@/components/home-landing/SectionHeader.vue'
import BrainIcon from '@/components/icons/BrainIcon.vue'
import EyeIcon from '@/components/icons/EyeIcon.vue'
import EarIcon from '@/components/icons/EarIcon.vue'
import OsteoporosisiIcon from '@/components/icons/OsteoporosisiIcon.vue'
import HeartIcon from '@/components/icons/HeartIcon.vue'
import CardiacIcon from '@/components/icons/CardiacIcon.vue'

export default {
  name: 'Departments',
  components: {
    SectionHeader,
    BrainIcon,
    EyeIcon,
    EarIcon,
    OsteoporosisiIcon,
    HeartIcon,
    CardiacIcon,
  },
}
</script>

<style scoped>
section {
  min-height: 70vh;
  padding-inline: var(--bg-padding-inline);
  display: grid;
  place-items: center;
}
.inner {
  padding-block: 3rem;
  width: 100%;
  max-width: var(--max-width);
  margin: auto;
  display: flex;
  gap: 4rem;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
}
.inner .department {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}
.department svg {
  width: 50px;
  height: 50px;
}
.department span {
  font-weight: 400;
  font-size: 1.25rem;
  line-height: 120%;
  color: #9291a1;
}
</style>
