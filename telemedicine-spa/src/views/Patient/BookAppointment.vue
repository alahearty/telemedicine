<template>
  <div class="book-appointment">
    <table>
      <tr>
        <th>Services</th>
        <th>Cost</th>
        <th>Description</th>
      </tr>
      <tr v-for="service in services" :key="service.id">
        <td>{{ service.title }}</td>
        <td>{{ service.cost }}</td>
        <td>{{ service.description }}</td>
      </tr>
    </table>
  </div>
</template>

<script>
export default {
  name: 'BookAppointment',
  data() {
    return {
      services: [
        {
          id: 1,
          title: 'Eye care',
          cost: '$200',
          description:
            'Lorem ipsum dolor sit amet consectetur adipisicing elit. Pariatur, esse!',
        },
        {
          id: 2,
          title: 'Neurology',
          cost: '$500',
          description:
            'Lorem ipsum dolor sit amet consectetur adipisicing elit. Pariatur, esse!',
        },
        {
          id: 3,
          title: 'Cardiac care',
          cost: '$1000',
          description:
            'Lorem ipsum dolor sit amet consectetur adipisicing elit. Pariatur, esse!',
        },
        {
          id: 1,
          title: 'Eye care',
          cost: '$200',
          description:
            'Lorem ipsum dolor sit amet consectetur adipisicing elit. Pariatur, esse!',
        },
        {
          id: 1,
          title: 'Cardiac care',
          cost: '$200',
          description:
            'Lorem ipsum dolor sit amet consectetur adipisicing elit. Pariatur, esse!',
        },
      ],
    }
  },
}
</script>

<style scoped>
.book-appointment {
  flex: 1;
  min-height: 400px;
}
table,
th,
td {
  border: 1px solid #dee2e6;
  text-align: left;
  padding: 0.7rem;
}
table {
  width: 100%;
}
h3 {
  font-weight: 500;
}
</style>
