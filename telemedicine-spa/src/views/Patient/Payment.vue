<template>
  <div>
    <div class="columns">
      <ul class="price">
        <li class="header">Basic</li>
        <li class="grey">$ 9.99 / year</li>
        <li>10GB Storage</li>
        <li>10 Emails</li>
        <li>10 Domains</li>
        <li>Neurology</li>
        <li>1GB Bandwidth</li>
        <li>Limited Access</li>
        <li class="grey"><a href="#" class="button">Proceed Payment</a></li>
      </ul>
    </div>

    <div class="columns">
      <ul class="price">
        <li class="header" style="background-color:#04AA6D">Pro</li>
        <li class="grey">$ 24.99 / year</li>
        <li>Eye care</li>
        <li>Neurology</li>
        <li>Cardiac care</li>
        <li>Cardiac care</li>
        <li>Physiotherapy</li>
        <li>Limited Access</li>
        <li class="grey"><a href="#" class="button">Proceed Payment</a></li>
      </ul>
    </div>

    <div class="columns">
      <ul class="price">
        <li class="header">Premium</li>
        <li class="grey">$ 49.99 / year</li>
        <li>Eye care</li>
        <li>Neurology</li>
        <li>Cardiac care</li>
        <li>2GB Bandwidth</li>
        <li>Physiotherapy</li>
        <li>Unlimited Services</li>
        <li class="grey"><a href="#" class="button">Proceed Payment</a></li>
      </ul>
      </div>
  </div>
</template>

<script>
export default {}
</script>

<style scoped>
  * {
  box-sizing: border-box;
}

.columns {
  float: left;
  width: 33.3%;
  padding: 8px;
}

.price {
  list-style-type: none;
  border: 1px solid #eee;
  margin: 0;
  padding: 0;
  -webkit-transition: 0.3s;
  transition: 0.3s;
}

.price:hover {
  box-shadow: 0 8px 12px 0 rgba(0,0,0,0.2)
}

.price .header {
  background-color: #111;
  color: white;
  font-size: 25px;
}

.price li {
  border-bottom: 1px solid #eee;
  padding: 20px;
  text-align: center;
}

.price .grey {
  background-color: #eee;
  font-size: 20px;
}

.button {
  background-color: #04AA6D;
  border: none;
  color: white;
  padding: 10px 25px;
  text-align: center;
  text-decoration: none;
  font-size: 18px;
}

@media only screen and (max-width: 600px) {
  .columns {
    width: 100%;
  }
}
</style>
