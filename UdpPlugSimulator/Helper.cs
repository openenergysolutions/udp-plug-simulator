// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

namespace UdpPlugSimulator
{
    internal class Helper
    {
        private static readonly Random _random = new Random();

        public static string GetRandomMacAddress()
        {
            var buffer = new byte[6];
            _random.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => string.Format("{0}", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':').ToLower();
        }
    }
}
