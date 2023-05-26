// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

namespace UdpPlugSimulator
{
    public interface IDeviceManager
    {
        List<Plug> Plugs { get; }

        void Stop();
        void Start();
    }
}