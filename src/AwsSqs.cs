using Amazon.SQS.Model;
using Amazon.SQS;
using Microsoft.Extensions.Logging;

namespace aws_sqs;

public class AwsSqs(
    IAmazonSQS sqsClient, 
    ILogger<AwsSqs> logger)
{
    private readonly IAmazonSQS _sqsClient = sqsClient;
    private readonly ILogger<AwsSqs> _logger = logger;

    /// <summary>
    /// Sends a message to the specified Amazon SQS queue.
    /// </summary>
    /// <param name="queueUrl">The URL of the SQS queue.</param>
    /// <param name="groupId">The ID of the message group for FIFO queues.</param>
    /// <param name="message">The content of the message to send.</param>
    /// <returns>An asynchronous task representing the operation.</returns>
    public async Task SendMessageAsync(
        string queueUrl, 
        string groupId, 
        string message)
    {
        try
        {
            var sendRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageGroupId = groupId,
                MessageBody = message,
                MessageDeduplicationId = Guid.NewGuid().ToString(),
            };

            var response = await _sqsClient.SendMessageAsync(sendRequest);
            _logger.LogInformation($"Message sent successfully with ID: {response.MessageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// Receives and processes messages from the specified Amazon SQS queue.
    /// </summary>
    /// <param name="queueUrl">The URL of the SQS queue.</param>
    /// <param name="maxIterationsByBatch">The maximum number of iterations to poll for messages in batches.</param>
    /// <returns>An asynchronous task representing the operation.</returns>
    public async Task ReceiveMessagesAsync(
        string queueUrl, 
        int maxIterationsByBatch)
    {
        try
        {
            for (int i = 0; i < maxIterationsByBatch; i++)
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 10
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest);

                if (response.Messages.Count > default(int))
                {
                    foreach (var message in response.Messages)
                    {
                        _logger.LogInformation($"Message Id: {message.MessageId}, Message Body: {message.Body}");
                        await DeleteMessagesAsync(queueUrl, message.ReceiptHandle);
                    }
                }
                else
                {
                    _logger.LogInformation("No messages available.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a message from the specified Amazon SQS queue using its receipt handle.
    /// </summary>
    /// <param name="queueUrl">The URL of the SQS queue.</param>
    /// <param name="receiptHandle">The receipt handle of the message to delete.</param>
    /// <returns>An asynchronous task representing the operation.</returns>
    private async Task DeleteMessagesAsync(
        string queueUrl, 
        string receiptHandle)
    {
        try
        {
            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}