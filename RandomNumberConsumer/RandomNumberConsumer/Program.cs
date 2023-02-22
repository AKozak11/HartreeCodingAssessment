using Confluent.Kafka;
using Common.Models;
using Common.Messaging;
using EntityConnector.Models;
using RandomNumberConsumer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RandomNumberConsumer
{
    class Program
    {
        public static IConfiguration? Configuration { get; set; }
        
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder().ConfigureServices((hostBuilderContext, services) =>
            {
                Configuration = new ConfigurationBuilder()
                    .AddJsonFile("KafkaConfig.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                ConsumerConfig sqlServiceConsumerConfig = new ConsumerConfig();
                Configuration.Bind("KafkaConfig", sqlServiceConsumerConfig);
                Configuration.Bind("SqlServiceConsumerConfig:KafkaConfig", sqlServiceConsumerConfig);
                services.AddSingleton<ConsumerConfig>(sqlServiceConsumerConfig);


                MessageConfig messageConfig = new MessageConfig();
                Configuration.Bind("MessageConfig", messageConfig);
                services.AddSingleton<MessageConfig>(messageConfig);

                services.AddDbContext<Context>(optionsBuilder => optionsBuilder.UseSqlServer(Configuration["ConnectionString"], options =>
                {
                    options.EnableRetryOnFailure(5);
                }), ServiceLifetime.Transient);

                DbContextOptionsBuilder<Context> optionsBuilder = new DbContextOptionsBuilder<Context>();
                optionsBuilder.UseSqlServer(Configuration["ConnectionString"]);
                using (Context context = new Context(optionsBuilder.Options))
                {
                    context.Database.Migrate();
                }

                services.AddSingleton<IMessageReader<string, string>>(sp =>
                {
                    return new MessageReader<string, string>(messageConfig.KafkaTopic, sp.GetRequiredService<ConsumerConfig>());
                });


                services.AddHostedService<SqlService>();

            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
        }
    }
}