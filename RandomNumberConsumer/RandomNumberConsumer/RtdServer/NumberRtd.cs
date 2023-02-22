using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using Newtonsoft.Json;
using ExcelDna.Integration;
using ExcelDna.Integration.Rtd;
using System.Collections.Generic;

namespace RtdFunctions
{
    public class NumServer : ExcelRtdServer
    {
        private readonly Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
        private Dictionary<string, ConsumeResult<string, string>> _previousMessage = new Dictionary<string, ConsumeResult<string, string>>();
        private IMessageReader<string, string> _messageReader;
        private MessageConfig _messageConfig;
        private Timer timer;

        public NumServer()
        {
            timer = new Timer(Callback);
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
            ConsumeResult<string, string> message = _messageReader.Consume();
            if (message is not null && !string.IsNullOrEmpty(message.Value))
            {
                // If there is an entry for this message key in the dictionary, commit it so that it won't be consumed again when the excel add in is closed and opened
                // Do not commit the current message so that it will be read again when add in is reopened
                if (_previousMessage.ContainsKey(message.Key)) _messageReader.Commit(_previousMessage[message.Key]);
                else _previousMessage.Add(message.Key, message);

                // save current message in memory so it is committed on next message read with same key
                _previousMessage[message.Key] = message;

                RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(message.Value);
                if (topics.ContainsKey(message.Key)) topics[message.Key].UpdateValue(rnd.Value.Value);
            }
            Start();
        }

        protected override void ServerTerminate()
        {
            _messageReader.Dispose();
            timer.Dispose();
            timer = null;
        }

        protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
        {
            if (!topics.ContainsKey(topicInfo[0])) topics.Add(topicInfo[0], topic);
            Start();
            return topics[topicInfo[0]].Value;
        }

        protected override void DisconnectData(Topic topic)
        {
            string key = topics.Where(t => t.Value == topic).First().Key;
            if (!string.IsNullOrEmpty(key)) topics.Remove(key);
            if (topics.Count == 0)
                Stop();
        }
        private MessageReader<string, string> GetMessageReader()
        {
            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = "localhost:9092",
                EnableAutoCommit = false,
                SecurityProtocol = 0,
                SessionTimeoutMs = 6000,
                GroupId = "excel-group",
                ClientId = "excel"
            };
            return new MessageReader<string, string>("RANDOM_NUMBER_DATA", consumerConfig);
        }
    }
    public static class Rtdfunctions
    {
        public static object GetData(string key)
        {
            return XlCall.RTD("RtdFunctions.NumServer", null, key);
        }
    }
}