// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace UdpPlugSimulator
{
    public class Plug
    {
        private readonly Random _random = new Random();
        public event PropertyChangedEventHandler PropertyChanged;

        public const int BROAD_CAST_PORT = 8555;
        private UdpClient _udpClient;

        private string _name = "OES-Plug";
        private PlugStatus _status = PlugStatus.On;

        public string IpAddress { get; set; } = "127.0.0.1";
        public string HostIpAddress { get; set; } = "127.0.0.1";
        public string MacAddress { get; set; } = Helper.GetRandomMacAddress();
        public int Port { get; set; } = 8556;

        private readonly ISettings _settings;
        private readonly ILogger _logger;

        public PlugStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }
        public int Voltage { get; set; } = 120;
        public int Current { get; set; }
        public int Power { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private bool _shuttingDown = false;

        public bool IsRunning
        {
            get { return _udpClient != null; }
        }

        public Plug(ILogger logger, ISettings settings)
        {
            _settings = settings;
            _logger = logger;

            IpAddress = _settings.IpAddress;
            HostIpAddress = _settings.HostIpAddress;
            Port = _settings.Port;
            Name = _settings.Name;
            MacAddress = _settings.MacAddress;
            Voltage = _settings.Voltage;
            Current = _settings.Current;
            Power = _settings.Power;
        }

        public void Start()
        {
            Stop();

            _udpClient = new UdpClient();

            try
            {
                _udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse(IpAddress), Port));
                _udpClient.EnableBroadcast = true;
            }
            catch
            {
                _udpClient.Close();
                _udpClient = null;
                throw;
            }

            var from = new IPEndPoint(0, 0);
            Task.Run(() =>
            {
                while (!_shuttingDown)
                {
                    try
                    {
                        // wait for control messages
                        var recvBuffer = _udpClient.Receive(ref from);
                        var control = Encoding.UTF8.GetString(recvBuffer);
                        Debug.WriteLine($"Received message: {control}");

                        var controlMessage = JsonConvert.DeserializeObject<Request>(control);

                        Status = controlMessage.FC == FC.Off ? PlugStatus.Off : PlugStatus.On;

                        var response = new Response()
                        {
                            FC = controlMessage.FC,
                            Accepted = true,
                        };

                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                        _udpClient.Send(bytes, bytes.Length, from);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Invalid message.  Error occurred when trying to parse message: {0}", ex.Message);
                    }
                }
            });

            Task.Run(() =>
            {
                while (_udpClient != null)
                {
                    try
                    {
                        // Broadcast status messages
                        var data = GetData();
                        if (_udpClient != null)
                        {
                            _logger.LogInformation("Broadcast: {0}", Encoding.UTF8.GetString(data));
                            _udpClient.Send(data, data.Length, "255.255.255.255", BROAD_CAST_PORT);

                            Thread.Sleep(10000);
                        }
                    }
                    catch { }
                }
            });
        }

        public void Stop()
        {
            if (_udpClient != null)
            {
                _shuttingDown = true;
                _udpClient.Close();

                Thread.Sleep(1000);
                _udpClient = null;
                _shuttingDown = false;
            }
        }

        private byte[] GetData()
        {
            var data = new Data()
            {
                Name = Name,
                PlugStatus = Status,
                MacAddress = MacAddress,
                IpAddress = HostIpAddress,
                W = Math.Round(Power + _random.NextDouble(), 2),
                V = Math.Round(Voltage + _random.NextDouble(), 2),
                I = Math.Round(Current + _random.NextDouble(), 2)
            };

            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PlugStatus
    {
        Off,
        On
    }

    public enum FC
    {
        Off,
        On
    }

    public class Request
    {
        [JsonProperty("fc")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FC FC { get; set; }
    }

    public class Response
    {
        [JsonProperty("fc")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FC FC { get; set; }
        [JsonProperty("accepted")]
        public bool Accepted { get; set; }
    }

    public class Data
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PlugStatus PlugStatus { get; set; }
        [JsonProperty("mac_address")]
        public string MacAddress { get; set; }
        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }
        [JsonProperty("power")]
        public double W { get; set; }
        [JsonProperty("voltage")]
        public double V { get; set; }
        [JsonProperty("current")]
        public double I { get; set; }
    }
}
