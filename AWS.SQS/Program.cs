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
    var createQueueRequest = new CreateQueueRequest()
    {
        QueueName = queueName
    };

    var createQueueResponse = await _amazonSQS.CreateQueueAsync(queueName);

    return Results.Ok(createQueueResponse);

});


app.MapDelete("delete-queue-by-name", async (
    [FromServices] IAmazonSQS _amazonSQS,
    [FromServices] IConfiguration _configuration,
    [FromQuery] string queueName) =>
{

    var getQueueUrlResponse = await _amazonSQS.GetQueueUrlAsync(queueName);

    var deleteQueueRequest = new DeleteQueueRequest()
    {
        QueueUrl = getQueueUrlResponse.QueueUrl
    };

    var deleteQueueResponse = await _amazonSQS.DeleteQueueAsync(deleteQueueRequest);

    return Results.Ok(deleteQueueResponse);


});














app.Run();
