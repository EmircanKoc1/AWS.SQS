using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonSQS>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/list-queues", async (
    [FromServices] IAmazonSQS _amazonSQS,
    [FromServices] IConfiguration _configuration,
    [FromQuery] int maxResults) =>
{
    var listQueuesRequest = new ListQueuesRequest()
    {
        MaxResults = maxResults
    };

    var listedQueuesResponse = await _amazonSQS.ListQueuesAsync(listQueuesRequest);


    return Results.Ok(listedQueuesResponse);

});

app.MapPost("create-queue", async (
    [FromServices] IAmazonSQS _amazonSQS,
    [FromServices] IConfiguration _configuration,
    [FromQuery] string queueName) =>
{
   

    var listedQueuesResponse = await _amazonSQS.ListQueuesAsync(listQueuesRequest);


    return Results.Ok(listedQueuesResponse);

});






app.MapPost("/send-message ", async (
 [FromServices] IConfiguration _configuration,
 [FromServices] IAmazonSQS _amazonSQS,
 [FromBody] string message) =>
{


    var queueUrl = _configuration;






    return await _amazonSQS.ReceiveMessageAsync("");


});






app.Run();
