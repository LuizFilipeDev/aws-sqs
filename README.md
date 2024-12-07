# AWS SQS

A .NET Console Application designed to simplify a example of interaction with AWS Simple Queue Service (SQS). The application provides a class (`AwsSqs`) that encapsulates common SQS operations, such as sending, receiving, and deleting messages from a queue.

## What are the functions?

The main functions of the `AwsSqs` class are:

1. **Send Message**  
   Send a message to an AWS SQS queue.  
   The message includes attributes like `GroupId`, `UserId`, `UserName`, and `Age`.

2. **Receive Messages**  
   Retrieve messages in batches from the SQS queue, process them, and delete them after processing.

3. **Delete Message**  
   Delete a message from the queue after it has been successfully processed.

## AWS SQS Service
AWS SQS is a fully managed message queuing service that enables asynchronous communication between services or components of distributed systems. This application interacts with SQS using the AWS SDK.

### How to use the application

#### 1. Update AWS Configuration
Modify the constants in the code for your specific SQS setup:
- **AWS_REGION**: Specify the AWS region where your SQS queue is hosted (e.g., `us-east-1`).
- **AWS_ACCOUNT_ID**: Replace with your AWS account ID.
- **AWS_QUEUE_NAME**: Replace with your SQS queue name.

Example:
```csharp
const string AWS_REGION = "us-east-1";
const string AWS_ACCOUNT_ID = "123456789012";
const string AWS_QUEUE_NAME = "queue-test.fifo";
```

#### 2. Set up Logger
The application uses a console logger to provide feedback on operations. No additional configuration is needed for basic logging.

#### 3. Sending Messages
The `SendMessageAsync` method sends a message to the queue. The message contains serialized JSON data with attributes such as:

- **GroupId**: The group identifier for FIFO queues.
- **UserId**: A unique identifier for the user.
- **UserName**: A generated username.
- **Age**: A randomly assigned age.

#### Example usage:
```csharp
await awsSqs.SendMessageAsync(QUEUE_URL, GROUP_ID, 
    JsonConvert.SerializeObject(new
    {
        GroupId = GROUP_ID,
        UserId = Guid.NewGuid(),
        UserName = $"User-{Guid.NewGuid()}",
        Age = Random.Shared.Next(18, 100)
    }));
```

#### 4. Receiving Messages
The `ReceiveMessagesAsync` method retrieves messages from the queue in batches. It logs the received messages and deletes them after processing.

#### Example usage:
```csharp
await awsSqs.ReceiveMessagesAsync(QUEUE_URL, MAX_ITERATIONS_BY_BATCH);
```

## Points to Highlight
- **FIFO Queue Support**: The implementation includes features like `MessageGroupId` and `MessageDeduplicationId` for FIFO queue compliance.
- **Logging**: Logs information on successful operations and errors for better traceability.
- **Scalable Processing**: The `ReceiveMessagesAsync` method supports batch processing with configurable iteration limits.

## Before Running the Application
1. Ensure you have valid AWS credentials configured on your system or in the AWS SDK for .NET.
2. Update the constants in the code to match your SQS setup.
3. Ensure your SQS queue (e.g., `queue-test.fifo`) is created and ready to use.

## About
This project is developed using:

- **.NET 8**
- **AWS SDK for .NET**
- **Newtonsoft.Json** for JSON serialization
- **Microsoft.Extensions.Logging** for logging

It provides a foundational class to handle SQS operations with a focus on simplicity and extensibility.