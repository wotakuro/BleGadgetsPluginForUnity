using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public class MabeeeDevice
    {
        private byte[] buffer = new byte[32];

        private List<string> services;
        private Dictionary<string,string> charastrics;

        public MabeeeDevice(BleDeviceManager m,string addr)
        {
            this.manager = m;
            this.Address = addr;
            this.services = new List<string>();
            this.charastrics = new Dictionary<string, string>();
        }

        public void OnFindDevice(string service, int rssi, byte[] data)
        {
            this.Rssi = rssi;
            this.lastRssiUpdatedAt = Time.timeAsDouble;
        }


        public void Connect()
        {
            toio.Ble.ConnectToPeripheral(Address,
                manager.OnConnectDevice,
                manager.OnDiscoveredService,
                manager.OnDiscoveredCharacteristic,
                manager.OnDisconnect);
        }
        public void Disconnect()
        {
            toio.Ble.DisconnectPeripheral(this.Address, manager.OnDisconnect);
        }

        internal void OnDisconnect()
        {
            this.IsConnect = false;
            this.services.Clear();
            this.charastrics.Clear();
        }


        private BleDeviceManager manager;

        public string Address { get; private set; }
        public bool IsConnect { get; internal set; }
        public int Rssi { get; internal set; }
        public double lastRssiUpdatedAt { get; private set; }


        public static readonly string ServiceUUID = "B9F5FF00-D813-46C6-8B61-B453EE2C74D9";
        public static readonly string pwmDutyUuid = "B9F53006-D813-46C6-8B61-B453EE2C74D9";
        public static readonly string batteryDataUuid = "B9F51001-D813-46C6-8B61-B453EE2C74D9";
        public static readonly string overCurrentUuid = "B9F51002-D813-46C6-8B61-B453EE2C74D9";
        public static readonly string pioReadUuid = "B9F53003-D813-46C6-8B61-B453EE2C74D9";


        public enum DisconectMode
        {
            Reset,
            Continue
        }
        public int OutputPower
        {
            set
            {
                buffer[0] = 1;
                // value
                buffer[1] = (byte)(value & 0xFF);
                buffer[2] = (byte)(value >> 8 & 0xFF);
                buffer[3] = (byte)(value >> 16 & 0xFF);
                buffer[4] = (byte)(value >> 24 & 0xFF);
                toio.Ble.WriteCharacteristic(this.Address,
                    ServiceUUID, pwmDutyUuid, buffer, 5, false,null);
            }
        }
        /*
        public DisconectMode diconnectMode { get; private set; }

        public int batteryVoltage { get; private set; }


        */
    }
}
