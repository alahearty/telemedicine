<template>
    <section id="service">
        <SectionHeader title="Services" />
        <div class="wrapper">
            <div class="line"></div>
            <div class="single-service">
                <div class="social">
            <i class="fa fa-codepen"></i></div>
            <span></span>
                <h3>Physiotherapy Examination</h3>
            <p>
            Lorem ipsum dolor sit amet, consectetur adipisicing elit. Et delectus, in veritatis veniam non praesentium corporis dolorum necessitatibus. Culpa odio enim amet praesentium illo voluptates ab quidem, nam consequuntur. Natus.</p>
        </div>
        <div class="single-service">
                <div class="social">
                    <i class="fa fa-codepen"></i></div>
                <span></span>
                <h3> Eye Examination</h3>
                <p>
                Lorem ipsum dolor sit amet, consectetur adipisicing elit. Et delectus, in veritatis veniam non praesentium corporis dolorum necessitatibus. Culpa odio enim amet praesentium illo voluptates ab quidem, nam consequuntur. Natus.</p>
        </div>
        <div class="single-service">
            <div class="social">
                <i class="fa fa-diamond"></i>
            </div>
            <span></span>
                <h3> Health Care</h3>
            <p>
            Lorem ipsum dolor sit amet, consectetur adipisicing elit. Et delectus, in veritatis veniam non praesentium corporis dolorum necessitatibus. Culpa odio enim amet praesentium illo voluptates ab quidem, nam consequuntur. Natus.
            </p>
        </div>
        </div>
    </section>
</template>
<script>
import SectionHeader from '@/components/home-landing/SectionHeader.vue'

export default {
    name: 'Service',
    components: { SectionHeader }
}
</script>
<style scoped>
.wrapper{
    width: 1170px;
    margin: 5% auto;
}
    .wrapper h2{
        text-transform: uppercase;
        font-family: poppins;
        font-weight: bold;
        text-align: center;
        font-size: 60px;
        color: #fff;
        margin: 0;
    }
    .single-service {
        position: relative;
        width: 31%;
        height: 420px;
        background: #fff;
        box-sizing: border-box;
        padding: 0 15px;
        transition: .5s;
        overflow: hidden;
        float: left;
        margin: 0 10px;
        text-align: center;
    }
    .line {
        width: 150px;
        height: 3px;
        background: #fff;
        margin: 0 auto 60px auto;
    }
    .single-service p{            
        color: #262626;
        font-size: 14px;
    }
    .single-service h3 {
        font-size: 25px;
        text-transform: uppercase;
        font-family: poppins;
        letter-spacing: 1px;
        color: #262626;
    }
    .social {
        width: 60px;
        height: 80px;
        background: #262626;
        border-radius: 50%;
        margin: 5% auto;
    }
    .social i {
        font-size: 30px;
        padding: 15px;
        color: #fff;
    }
    .single-service:hover{
        box-shadow: 0 30px 35px rgba(0,0,0,0.7);
    }
    .single-service span {
        position: absolute;
        top: 0;
        left: -110%;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.7);
        transition: .7s;
        transform: skewX(10deg);
    }
    .single-service:hover span{
        left: 110%;
    }

    @media (max-width:1000px){
        .wrapper {
            width: 100%;
        }
        .single-service {
            width: 95%;
            margin-bottom: 30px;
        }
     .wrapper h2 {
        font-size: 30px;
     }
  }

</style>