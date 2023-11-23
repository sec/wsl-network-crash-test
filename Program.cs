using Google.Api.Gax;
using Google.Cloud.PubSub.V1;

Environment.SetEnvironmentVariable("PUBSUB_EMULATOR_HOST", "localhost:8085");
await Task.WhenAll(Enumerable.Range(0, 32).Select(i => Run()));

static async Task Run()
{
    var projectId = "test";
    var subscriptionId = Guid.NewGuid().ToString("n");
    var topicName = TopicName.FromProjectTopic(projectId, Guid.NewGuid().ToString("n"));
    var subscriptionName = new SubscriptionName(projectId, subscriptionId);

    var publisherService = await new PublisherServiceApiClientBuilder { EmulatorDetection = EmulatorDetection.EmulatorOnly }.BuildAsync();
    await publisherService.CreateTopicAsync(topicName);

    var subscriberService = await new SubscriberServiceApiClientBuilder { EmulatorDetection = EmulatorDetection.EmulatorOnly }.BuildAsync();
    await subscriberService.CreateSubscriptionAsync(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 60);

    while (true)
    {
        try
        {
            await using var publisher = await new PublisherClientBuilder
            {
                TopicName = topicName,
                EmulatorDetection = EmulatorDetection.EmulatorOnly
            }.BuildAsync();
            await publisher.PublishAsync("ping");

            var msg = await subscriberService.PullAsync(subscriptionName, 1);
            Console.Write(".");

            await Task.Delay(Random.Shared.Next(100, 500));
        }
        catch
        {
            Console.WriteLine("Fail");
        }
    }
}
