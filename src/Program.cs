using Amazon.SQS;
using aws_sqs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

//Create a logger factory to configure logging behavior.
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<AwsSqs>();
var awsSqs = new AwsSqs(new AmazonSQSClient(), logger);

//Define constants for AWS configuration and queue details.
const string AWS_REGION = "us-east-1";
const string AWS_ACCOUNT_ID = "123456789123";
const string AWS_QUEUE_NAME = "queue-test.fifo";
const string QUEUE_URL = $"https://sqs.{AWS_REGION}.amazonaws.com/{AWS_ACCOUNT_ID}/{AWS_QUEUE_NAME}";

const string GROUP_ID = "3f4c387d-b589-4b2a-993e-380133703ba1";
const int MAX_ITERATIONS_BY_BATCH = 4;

//Send a message to the SQS queue.
await awsSqs.SendMessageAsync(QUEUE_URL, GROUP_ID,
    JsonConvert.SerializeObject(new
    {
        GroupId = GROUP_ID,
        UserId = Guid.NewGuid(),
        UserName = $"User-{Guid.NewGuid()}",
        Age = Random.Shared.Next(18, 100),
    }));

//Receive and process messages from the SQS queue, using the defined batch iteration limit.
await awsSqs.ReceiveMessagesAsync(
    QUEUE_URL, 
    MAX_ITERATIONS_BY_BATCH);