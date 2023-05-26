// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.Logging;

namespace UdpPlugSimulator
{
    public class DeviceManager : IDeviceManager
    {
        private readonly List<Plug> _plugs = new List<Plug>();
        private readonly ILogger<DeviceManager> _logger;
        private readonly ISettings _settings;

        public DeviceManager(ILogger<DeviceManager> logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;

            Plug plug = new Plug(logger, _settings);

            _logger.LogInformation($"Initial plug with settings: {_settings}");

            Plugs.Add(plug);
        }

        public List<Plug> Plugs { get { return _plugs; } }

        public void Stop()
        {
            _logger.LogInformation("Stop all plugs...");
            foreach (Plug plug in Plugs)
            {
                plug.Stop();
            }
        }

        public void Start()
        {
            _logger.LogInformation("Start all plugs...");
            foreach (Plug plug in Plugs)
            {
                plug.Start();
            }
        }
    }
}
