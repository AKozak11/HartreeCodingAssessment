using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using Newtonsoft.Json;
using ExcelDna.Integration;
using ExcelDna.Integration.Rtd;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// using Serilog.Sinks.File;

namespace RtdFunctions
{
    [ComVisible(true)]
    public class NumServer : ExcelRtdServer
    {
        private readonly Dictionary<string, Topic> _topics = new Dictionary<string, Topic>();
        private Dictionary<string, ConsumeResult<string, string>> _previousMessages;
        private Dictionary<string, KeyedDataProvider> _dataProviders;
        protected override bool ServerStart()
        {
            _dataProviders = new Dictionary<string, KeyedDataProvider>();
            _previousMessages = new Dictionary<string, ConsumeResult<string, string>>();
            return true;
        }

        protected override void ServerTerminate()
        {
            foreach (string key in _dataProviders.Keys) _dataProviders[key].Dispose();
        }

        protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
        {
            if (!_topics.ContainsKey(topicInfo[0])) _topics.Add(topicInfo[0], topic);
            if (!_previousMessages.ContainsKey(topicInfo[0])) _previousMessages.Add(topicInfo[0], null); // initial message is null to prevent committing last message from kafka
            if (!_dataProviders.ContainsKey(topicInfo[0]))
            {
                // each key will get assigned it's own dataprovider to prevent committing messages
                // yet to be consumed by other keys
                KeyedDataProvider dataProvider = new KeyedDataProvider(topicInfo[0]);
                _dataProviders.Add(topicInfo[0], dataProvider);
                dataProvider.NewData += consumeResult =>
                {
                    if (consumeResult is not null && !string.IsNullOrEmpty(consumeResult.Message.Value))
                    {
                        // If there is an entry for this message key in the dictionary, commit the existing entry so that it won't be consumed again when the excel add in is closed and re-opened
                        // Do not commit the current message, so that it will be read again when add in is re-opened (this is the last consumed message until this section of code is triggered again)
                        if (_previousMessages[consumeResult.Message.Key] is not null) dataProvider._messageReader.Commit(_previousMessages[consumeResult.Message.Key]);

                        // save current consumeresult in memory to be committed later
                        _previousMessages[consumeResult.Message.Key] = consumeResult;

                        RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(consumeResult.Message.Value);
                        if (_topics.ContainsKey(consumeResult.Message.Key))
                            _topics[consumeResult.Message.Key].UpdateValue(rnd.Value.Value);
                    }
                };
            }
            return _topics[topicInfo[0]].Value;
        }

        protected override void DisconnectData(Topic topic)
        {
            List<KeyValuePair<string, Topic>> kvp = _topics.Where(t => t.Value == topic).ToList();
            if (kvp.Count > 0)
            {
                string key = kvp.First().Key;
                _topics.Remove(key);
                _previousMessages.Remove(key);
                _dataProviders.Remove(key);
            }
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