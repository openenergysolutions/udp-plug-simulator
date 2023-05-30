// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

using Newtonsoft.Json;

namespace UdpPlugSimulator
{
    public interface ISettings
    {
        string FilePath { get; set; }
        string Name { get; set; }
        string MacAddress { get; set; }
        string IpAddress { get; set; }
        public string HostIpAddress { get; set; }
        int Port { get; set; }
        int Voltage { get; set; }
        int Current { get; set; }
        int Power { get; set; }
        int BroadcastIntervalMs { get; set; }
        void Update();
    }

    public class Settings : ISettings
    {
        public string FilePath { get; set; } = "./config/config.json";
        public string Name { get; set; } = "UDP-Plug";
        public string MacAddress { get; set; } = "0a81d60c2233";
        public string IpAddress { get; set; } = "0.0.0.0";
        public string HostIpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8556;
        public int Voltage { get; set; } = 120;
        public int Current { get; set; } = 5;
        public int Power { get; set; } = 25;
        public int BroadcastIntervalMs { get; set; } = 1000;
        public void Update()
        {
            try
            {
                Settings temp = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FilePath)).Settings;
                Voltage = temp.Voltage;
                Current = temp.Current;
                Power = temp.Power;
            }
            catch
            {
                //
            }
        }
    }

    class Config
    {
        public Settings Settings { get; set; }
    }
}