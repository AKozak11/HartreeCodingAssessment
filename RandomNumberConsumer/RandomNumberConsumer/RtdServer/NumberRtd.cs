using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using Newtonsoft.Json;
using ExcelDna.Integration;
using ExcelDna.Integration.Rtd;
using System.Collections.Generic;

// using System.Timers;
// namespace NumberRtd
// {
namespace RtdFunctions
{
    public class NumServer : ExcelRtdServer
    {
        // private readonly List<Topic> topics = new List<Topic>();
        private readonly Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
        private IMessageReader<string, string> _messageReader;
        private MessageConfig _messageConfig;
        private Timer timer;

        public NumServer()
        {
            timer = new Timer(Callback);
            _messageConfig = GetMessageConfig();
            _messageReader = GetMessageReader();
        }

        private void Start()
        {
            timer.Change(100, 100);
        }

        private void Stop()
        {
            timer.Change(-1, -1);
        }

        private void Callback(object o)
        {
            Stop();
            // foreach (Topic topic in topics)
            //     topic.UpdateValue(GetTime());
            Message<string, string> message = _messageReader.ReadMessage();
            if (message is not null && !string.IsNullOrEmpty(message.Value))
            {
                RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(message.Value);
                if (topics.ContainsKey(message.Key)) topics[message.Key].UpdateValue(rnd.Value.Value);
            }
            Start();
        }

        protected override void ServerTerminate()
        {
            timer.Dispose();
            timer = null;
        }

        protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
        {
            // topics.Add(topic);
            Start();
            if (!topics.ContainsKey(topicInfo[0]) && _messageConfig.Keys.Contains(topicInfo[0])) topics.Add(topicInfo[0], topic);
            Start();
            return null;
        }

        protected override void DisconnectData(Topic topic)
        {
            string key = topics.Where(t => t.Value == topic).First().Key;
            if(!string.IsNullOrEmpty(key)) topics.Remove(key);
            // topics.Remove(topic);
            if (topics.Count == 0)
                Stop();
        }
        private MessageConfig GetMessageConfig()
        {
            return new MessageConfig
            {
                Keys = new string[] {
                    "Key1",
                    "Key2"
                }
            };
        }
        private MessageReader<string, string> GetMessageReader()
        {

            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = "localhost:9092",
                EnableAutoCommit = true,
                SecurityProtocol = 0,
                SessionTimeoutMs = 6000,
                GroupId = "excel-group",
                ClientId = "excel"
            };

            return new MessageReader<string, string>("RANDOM_NUMBER_DATA", consumerConfig);
        }
    }
    public static class Timefunctions
    {
        public static object GetData(string key)
        {
            return XlCall.RTD("RtdFunctions.NumServer", null, key);
        }
    }

    // public class TimeServer : ExcelRtdServer
    // {
    //     // private Func<string, IMessageReader<string, string>> _readerFactory;
    //     // private IMessageReader<string, string> _messageReader;
    //     // private readonly Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
    //     // private MessageConfig _messageConfig;
    //     private List<Topic> topics = new List<Topic>();
    //     private Timer timer;

    //     public TimeServer(Func<string, IMessageReader<string, string>> readerFactory, MessageConfig messageConfig)
    //     {
    //         timer = new Timer(Callback);
    //         // _messageConfig = messageConfig;
    //         // _readerFactory = readerFactory;
    //         // _messageReader = _readerFactory("excel");
    //         // foreach (string k in _messageConfig.Keys) Console.WriteLine(k);
    //     }

    //     private void Start()
    //     {
    //         timer.Change(100, 200);
    //     }

    //     private void Stop()
    //     {
    //         timer.Change(-1, -1);
    //     }

    //     private void Callback(object o)
    //     {
    //         Stop();
    //         // Message<string, string> message = _messageReader.ReadMessage();
    //         // if (message is not null && !string.IsNullOrEmpty(message.Value))
    //         // {
    //         //     RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(message.Value);
    //         //     if (topics.ContainsKey(message.Key)) topics[message.Key].UpdateValue(rnd.Value.Value);
    //         // }
    //         // foreach (string key in topics.Keys) topics[key].UpdateValue(12);
    //         Start();
    //     }

    //     // private static string GetTime()
    //     // {
    //     //     return DateTime.Now.ToString("HH:mm:ss.fff");
    //     // }

    //     protected override void ServerTerminate()
    //     {
    //         timer.Dispose();
    //         timer = null;
    //     }

    //     protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
    //     {
    //         // if (!topics.ContainsKey(topicInfo[0]) && _messageConfig.Keys.Contains(topicInfo[0])) topics.Add(topicInfo[0], topic);
    //         Start();
    //         return 10;
    //     }

    //     protected override void DisconnectData(Topic topic)
    //     {
    //         // topics.Remove(topics.Where(t => t.Value == topic).First().Key);
    //         if (topics.Count == 0)
    //             Stop();
    //     }
    // }
    // public static class Functions
    // {
    //     public static object GetData(string key)
    //     {
    //         return XlCall.RTD("RtdFunctions.TimeServer", null, key);
    //     }
    // }
}