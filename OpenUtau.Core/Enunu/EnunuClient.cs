﻿using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Serilog;

namespace OpenUtau.Core.Enunu {
    class EnunuClient {
        private static volatile EnunuClient instance;
        private static readonly object lockObj = new object();

        private EnunuClient() { }

        public static EnunuClient Inst {
            get {
                if (instance == null) {
                    lock (lockObj) {
                        if (instance == null) {
                            instance = new EnunuClient();
                        }
                    }
                }
                return instance;
            }
        }

        internal T SendRequest<T>(string[] args) {
            using (var client = new RequestSocket()) {
                client.Connect("tcp://localhost:15555");
                string request = JsonConvert.SerializeObject(args);
                Log.Information($"EnunuProcess sending {request}");
                client.SendFrame(request);
                var message = client.ReceiveFrameString();
                Log.Information($"EnunuProcess received {message}");
                return JsonConvert.DeserializeObject<T>(message);
            }
        }
    }
}
