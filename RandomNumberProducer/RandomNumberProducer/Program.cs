using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using RandomNumberProducer.Services;

namespace RandomNumberProducer
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        // *ENTRY POINT*
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args)
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

                MessageConfig messageConfig = new MessageConfig();
                Configuration.Bind("MessageConfig", messageConfig);
                services.AddSingleton<MessageConfig>(messageConfig);

                services.AddSingleton<IMessageWriter<string, string>>((serviceProvider) =>
                {
                    return new MessageWriter<string, string>(messageConfig.KafkaTopic, serviceProvider.GetRequiredService<ProducerConfig>());
                });

                services.AddHostedService<MessageProducerService>();
            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
        }
    }
}