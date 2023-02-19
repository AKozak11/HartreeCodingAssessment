using Confluent.Kafka;
using Common.Models;
using Common.Messaging;
using EntityConnector.Models;
using RandomNumberConsumer.Services;
using Microsoft.EntityFrameworkCore;

namespace RandomNumberConsumer
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder().ConfigureServices((hostBuilderContext, services) =>
            {
                Configuration = new ConfigurationBuilder()
                    .AddJsonFile("KafkaConfig.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                IMessageConfig consumerMessageConfig = new ConsumerMessageConfig();
                Configuration.Bind("MessageConfig", consumerMessageConfig);
                services.AddSingleton<IMessageConfig>(consumerMessageConfig);

                ConsumerConfig sqlServiceConsumerConfig = new ConsumerConfig();
                Configuration.Bind("KafkaConfig", sqlServiceConsumerConfig);
                Configuration.Bind("SqlServiceConsumerConfig:KafkaConfig", sqlServiceConsumerConfig);

                ConsumerConfig excelRTDServiceConsumerConfig = new ConsumerConfig();
                Configuration.Bind("KafkaConfig", excelRTDServiceConsumerConfig);
                Configuration.Bind("ExcelRTDServiceConsumerConfig:KafkaConfig", excelRTDServiceConsumerConfig);

                DbContextOptionsBuilder<Context> optionsBuilder = new DbContextOptionsBuilder<Context>();
                optionsBuilder.UseSqlServer(Configuration["ConnectionString"]);
                using (Context context = new Context(optionsBuilder.Options))
                {
                    context.Database.Migrate();
                }
                
                services.AddDbContext<Context>(optionsBuilder => optionsBuilder.UseSqlServer(Configuration["ConnectionString"]));

                // message reader factory
                // one for sql service consumer
                // and one for excel rtd service consumer
                services.AddSingleton<Func<string, IMessageReader<string, string>>>(sp => key =>
                {
                    if (key == "sql")
                    {
                        return new MessageReader<string, string>(consumerMessageConfig.KafkaTopic, sqlServiceConsumerConfig);
                    }
                    else if (key == "excel")
                    {
                        return new MessageReader<string, string>(consumerMessageConfig.KafkaTopic, excelRTDServiceConsumerConfig);
                    }
                    else throw new InvalidOperationException($"Cannot create MessageReader of type {key}. Options are [sql, excel].");
                });

                services.AddHostedService<SqlService>();
                // services.AddHostedService<ExcelRTDService>();
            });
        }
        // private static         
        // IHost host = Host.CreateDefaultBuilder(args)
        //     .ConfigureServices(services =>
        //     {
        //         services.AddHostedService<Worker>();
        //     })
        //     .Build();

        // host.Run();
    }
}