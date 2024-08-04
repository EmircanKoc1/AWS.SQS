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
    [FromQuery] int maxResults) =>
{
    var listQueuesRequest = new ListQueuesRequest()
    {
        MaxResults = maxResults
    };

    var listedQueuesResponse = await _amazonSQS.ListQueuesAsync(listQueuesRequest);


    return Results.Ok(listedQueuesResponse);

});

app.MapPost("/create-queue", async (
    [FromServices] IAmazonSQS _amazonSQS,
    [FromQuery] string queueName) =>
{
    var createQueueRequest = new CreateQueueRequest()
    {
        QueueName = queueName
    };

    var createQueueResponse = await _amazonSQS.CreateQueueAsync(queueName);

    return Results.Ok(createQueueResponse);

});


app.MapDelete("/delete-queue-by-name", async (
    [FromServices] IAmazonSQS _amazonSQS,
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

app.MapGet("/get-queue-url-by-name", async (
    [FromServices] IAmazonSQS _amazonSQS,
    [FromQuery] string queueName) =>
{

    var getQueueUrlRequest = new GetQueueUrlRequest()
    {
        QueueName = queueName
    };

    var getQueueUrlReponse = await _amazonSQS.GetQueueUrlAsync(queueName);


    return Results.Ok(getQueueUrlReponse.QueueUrl);

});



app.MapPost("/send-message-to-queue ", async (
 [FromServices] IAmazonSQS _amazonSQS,
 [FromBody] string message,
 [FromQuery] string queueName) =>
{

    var getQueueUrlRequest = new GetQueueUrlRequest()
    {
        QueueName = queueName
    };


    var getQueueUrlResponse = await _amazonSQS.GetQueueUrlAsync(getQueueUrlRequest);

    var sendMessageRequest = new SendMessageRequest()
    {
        QueueUrl = getQueueUrlResponse.QueueUrl,
        MessageBody = message

    };

    var sendMessageResponse = await _amazonSQS.SendMessageAsync(sendMessageRequest);

    return Results.Ok(sendMessageResponse);
});


app.MapPost("/receive-message-from-queue", async (
 [FromServices] IAmazonSQS _amazonSQS,
 [FromQuery] string queueName,
 [FromQuery] int receiveCount) =>
{
    var getQueueUrlRequest = new GetQueueUrlRequest()
    {
        QueueName = queueName
    };


    var getQueueUrlResponse = await _amazonSQS.GetQueueUrlAsync(getQueueUrlRequest);


    var receiveMessageRequest = new ReceiveMessageRequest()
    {
        QueueUrl = getQueueUrlResponse.QueueUrl,
        MaxNumberOfMessages = receiveCount,
        VisibilityTimeout = 30,
        WaitTimeSeconds = 20
    };

    var receiveMessageResponse = await _amazonSQS.ReceiveMessageAsync(receiveMessageRequest);

    var deleteMessageResponses = new List<DeleteMessageResponse>(
        capacity: receiveCount);

    foreach (var message in receiveMessageResponse.Messages)
    {
        var deleteMessageRequest = new DeleteMessageRequest()
        {
            QueueUrl = getQueueUrlResponse.QueueUrl,
            ReceiptHandle = message.ReceiptHandle
        };

        var deleteMessageResponse = await _amazonSQS.DeleteMessageAsync(deleteMessageRequest);

        deleteMessageResponses.Add(deleteMessageResponse);
    }
    

    return Results.Ok(deleteMessageResponses);
});










app.Run();
