using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mabeee
{
    public class MabeeeDevice
    {

        public enum DisconectMode
        {
            Reset,
            Continue
        }

        public string Address { get; private set; }
        public bool IsConnect { get; private set; }
        public int Rssi { get; private set; }
        public int OutputPower { get; private set; }
        public DisconectMode diconnectMode { get; private set; }

        public int batteryVoltage { get; private set; }

        public double lastUpdatedAt { get; private set; }

        private MabeeeDeviceManager manager;
    }
}
