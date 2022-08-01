<template>
  <div class="book-appointment">
    <article
      v-for="service in services"
      :class="{ active: selected === service.id }"
      :key="service.id"
      @click="handleClick(service.id)"
    >
      <h2 class="title">{{ service.title }}</h2>
      <p class="description">
        {{ service.description }}
      </p>

      <div>
        <p class="cost">
          {{ service.cost }}
        </p>
        <button class="select-btn" :class="{ remove: selected === service.id }">
          {{ this.selected === service.id ? 'Deselect' : 'Select' }}
        </button>
      </div>
    </article>
  </div>
  <button :disabled="!selected" class="payment-btn" @click="pushToPayment">
    Continue to payment
  </button>
</template>

<script>
export default {
  name: 'BookAppointment',
  data() {
    return {
      selected: null,
      services: [
        {
          id: 1,
          title: 'Eye care',
          cost: '$200',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 2,
          title: 'Neurology',
          cost: '$200',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 3,
          title: 'Cardiac care',
          cost: '$300',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 4,
          title: 'Eye care',
          cost: '$200',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 5,
          title: 'Cardiac care',
          cost: '$150',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 6,
          title: 'Eye care',
          cost: '$383',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 7,
          title: 'Cardiac care',
          cost: '$395',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 8,
          title: 'Physiotherapy',
          cost: '$999',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
        {
          id: 9,
          title: 'Tooth care',
          cost: '$220',
          description: `Lorem ipsum dolor sit amet consectetur, adipisicing elit. Aut quas exercitationem minima enim dicta soluta ea aperiam quos reiciendis nostrum.`,
        },
      ],
    }
  },
  methods: {
    handleClick(id) {
      if (this.selected === id) {
        this.selected = null
      } else {
        this.selected = id
      }
    },
    pushToPayment() {
      this.$router.push('/payment')
    },
  },
}
</script>

<style scoped>
.book-appointment {
  flex: 1;
  min-height: 400px;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}
article {
  padding: 1.5rem;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 2px 2px 4px -2px rgba(194, 194, 194, 0.75);
  transition: all 300ms ease;
  cursor: pointer;
}
article.active {
  background-color: #c7d2fe;
  box-shadow: 5px 7px 4px -2px rgba(194, 194, 194, 0.75);
}
article:hover {
  /* background-color: #c7d2fe; */
  box-shadow: 5px 7px 4px -2px rgba(194, 194, 194, 0.75);
}
article:hover .select-btn {
  outline: 1px solid #3730a3;
}
article:hover .select-btn.remove {
  outline: none;
}
article .title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #3730a3;
}
article .description {
  margin-bottom: 1.5rem;
  font-weight: 400;
  font-size: 0.95rem;
  opacity: 0.8;
}
article > div {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
}
article .cost {
  font-size: 1.15rem;
}
article .select-btn {
  padding: 0.3em 1em;
  background-color: #c7d2fe;
  border-radius: 4px;
  cursor: pointer;
  color: #3730a3;
  font-weight: 500;
  transition: all 300ms ease;
}
article .select-btn.remove {
  background-color: rgb(206, 61, 61);
  color: #fff;
}

button.payment-btn {
  display: block;
  color: #fff;
  margin-top: 2rem;
  padding: 0.7em 1em;
  cursor: pointer;
  background-color: #3730a3;
  border-radius: 6px;
  transition: box-shadow 500ms ease;
}
.payment-btn:hover {
  box-shadow: 5px 7px 4px -2px #3830a34d;
}
.payment-btn:disabled {
  border: 1px solid #999999;
  background-color: #cccccc;
  color: #666666;
  cursor: not-allowed;
}
.payment-btn:disabled:hover {
  box-shadow: 0 0 0 0;
}
</style>
