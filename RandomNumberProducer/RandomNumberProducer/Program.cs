using Confluent.Kafka;
using Common.Messaging;
using Common.Models;

namespace RandomNumberProducer
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run(); //.RunAsync().GetAwaiter().GetResult();
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder().ConfigureServices(async (hostBuilderContext, services) =>
            {
                Configuration = new ConfigurationBuilder()
                    .AddJsonFile("KafkaConfig.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                ProducerConfig producerConfig = new ProducerConfig();
                Configuration.Bind("KafkaConfig", producerConfig);
                services.AddSingleton<ProducerConfig>(producerConfig);

                ProducerMessageConfig producerMessageConfig = new ProducerMessageConfig();
                Configuration.Bind("MessageConfig", producerMessageConfig);
                services.AddSingleton<ProducerMessageConfig>(producerMessageConfig);

                services.AddSingleton<IMessageWriter<string, string>>((serviceProvider) =>
                {
                    return new MessageWriter<string, string>(producerMessageConfig.KafkaTopic, serviceProvider.GetRequiredService<ProducerConfig>());
                });



                services.AddHostedService<Worker>();
            });
        }
        // IHost host = Host.CreateDefaultBuilder(args)
        //     .ConfigureServices(services =>
        //     {
        //         services.AddHostedService<Worker>();
        //     })
        //     .Build();

        // host.Run();
    }
}