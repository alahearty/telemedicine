﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Admin.Queries.GetTelemedicinePayment;
using telemedicine_webapi.Application.Admin.Queries.GetTelemedicineService;
using telemedicine_webapi.Application.Admin.Queries.GetUsers;

namespace telemedicine.api.Controllers;

[Permit("Administrator")]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ApiControllerBase
{
    [HttpGet("services")]
    public async Task<IActionResult> GetServices()
    {
        //To be Replaced
        //var services = new List<dynamic>()
        //{
        //    new { name = "Consultation 1", data = new int[] { 30, 40, 45, 50, 49, 60, 70, 91 }},
        //    new { name = "Consultation 2", data = new int[]  {20, 34, 45, 55, 79, 87, 90, 98 } }
        //};

        var response = await Mediator.Send(new GetTelemedicineServiceQuery());
        return Ok(response);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetRevenue()
    { 
        
        var response = await Mediator.Send(new GetTelemedicinePaymentQuery());
        return Ok(response);
        //var revenue = new List<dynamic>()
        //{
        //    new { name = "Revenue", data = new int[] { 30, 40, 45, 50, 49, 60, 70, 91 }},
        //    new { name = "Revenue (Previous period)", data = new int[]  {20, 34, 45, 55, 79, 87, 90, 98 } }
        //};
        //return Ok(revenue);
    }

    [HttpGet("visitors")]// no. of active users
    public IActionResult GetVisitor()
    {
        var visitor = new { name = "Visitor", data = new int[] { 30, 40, 45, 50, 49, 60, 70, 91 } };
        return Ok(visitor);
    }

    [HttpGet("transactions")]
    public IActionResult GetTransactions()
    {
        var transactions = new List<dynamic>()
        {
            new { transaction = "Payment from Ike yolanda", datetime = "Apr 22, 2022", amount = "Rp.450.000", statusTransaction = "completed"},
            new { transaction = "Payment from Ice Wulandari", datetime = "May 2, 2022", amount = "Rp.250.000", statusTransaction= "completed"},
            new { transaction = "Payment from Alfiah Gipta Jannatil Hasanah", datetime = "May 5, 2022", amount = "Rp.150.000", statusTransaction= "progress"},
            new { transaction = "Payment failed from #046577", datetime = "May 5, 2022", amount = "Rp.180.000" , statusTransaction= "cancelled"}
        };
        return Ok(transactions);
    }

    [HttpGet("reports")]
    public IActionResult GetReports()
    {
        var reports = new List<dynamic>()
        {
            new { title = "1,780", subtitle = "New Physians this week", total = "27.9%", report = "Product Report"},
            new { title = "5,355", subtitle = "Visitor this week", total = "47.9%", report= "Visitor Report"},
            new { title = "475", subtitle = "User signups this week", total = "", report= "User Report"}
        };
        return Ok(reports);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var response = await Mediator.Send(new GetUsersQuery());
        return Ok(response);
        //var user = new List<dynamic>()
        //{
        //    new { role = "admin"},
        //    new { title = "SuperAdmin"},
        //    new { title = "User"},
        //    new { title = "Customer"}
        //};
        //return Ok(user);
    }
}
