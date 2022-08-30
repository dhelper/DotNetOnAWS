using System;
using Amazon;
using Amazon.SQS;

namespace QueueListener
{
    internal class Program
    {
        private const string QueueName = "MyMessageQ";

        public static void Main(string[] args)
        {
            using (var sqsClient = new AmazonSQSClient(RegionEndpoint.EUWest2))
            {
                var wrapper = new QueueWrapper(sqsClient);

                var queueUrl = wrapper.GetQueueUrl(QueueName).Result;
                
                Console.WriteLine($"Listening for messages, queue url: {queueUrl}");

                do
                {
                    var message = wrapper.GetNextMessage(queueUrl).Result;

                    Console.WriteLine(message);
                    Console.WriteLine("-----------------");

                } while (true);
            }
        }
    }
}