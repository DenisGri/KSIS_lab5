using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using LabService;

namespace SClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5000");
            var client = new Greeter.GreeterClient(channel);

            using var call = client.SayHelloStream();

            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Message);
                    Console.WriteLine(response.MessageBox);
                }
            });

            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(result))
                {
                    break;
                }

                await call.RequestStream.WriteAsync(new HelloRequest() {Name = result});
            }

            await call.RequestStream.CompleteAsync();
            await readTask;
            Console.ReadKey();
        }
    }
}
