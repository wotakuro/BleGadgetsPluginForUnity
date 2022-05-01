using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public class BleDevice
    {
        private byte[] buffer = new byte[32];

        private List<string> services;
        private Dictionary<string,string> charastrics;

        public BleDevice() { 
        }
        
        internal void Initialize(BleDeviceManager m,string addr)
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

        internal void OnDiscoverService(string serviceUuid)
        {

        }
        internal void OnDiscoverCharastristic(string serviceUuid,string charastristicUuid)
        {
        }

        protected void ReadRequest(string serviceUuid, string charastristicUuid)
        {
            toio.Ble.ReadCharacteristic(this.Address,serviceUuid,charastristicUuid, OnReadData);
        }
        protected virtual void OnReadData(string serviceUuid,string charastristicUuid,byte[] data)
        {
            string str = ("OnReadData:" + serviceUuid + ":" + charastrics + "\n");
            for (int i = 0;i < data.Length; ++i)
            {
                str += data[i] + "::";
            }
            Debug.Log(str);
        }

        protected void NotificateRequest(string serviceUuid, string charastristicUuid,bool flag = true)
        {
            if (flag)
            {
                toio.Ble.SubscribeCharacteristic(this.Address, serviceUuid, charastristicUuid, OnNotificateData);
            }
            else
            {
                toio.Ble.UnSubscribeCharacteristic(this.Address, serviceUuid, charastristicUuid,
                    (str)=> { });
            }
        }
        protected virtual void OnNotificateData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }

        protected void WriteRequest(string serviceUuid,string charastristicUuid,byte[] data,int length)
        {
            toio.Ble.WriteCharacteristic(this.Address,
                serviceUuid, charastristicUuid, data, length, false, null);
        }

        private BleDeviceManager manager;

        public string Address { get; private set; }
        public bool IsConnect { get; internal set; }
        public int Rssi { get; internal set; }
        public double lastRssiUpdatedAt { get; private set; }

        #region ORIGIN
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

        private int currentPower = 0;
        public int OutputPower
        {
            set
            {
                if(currentPower == value)
                {
                    return;
                }
                Debug.Log("Setoutput Power " + value);
                buffer[0] = 1;
                // value
                buffer[1] = (byte)(value & 0xFF);
                buffer[2] = (byte)(value >> 8 & 0xFF);
                buffer[3] = (byte)(value >> 16 & 0xFF);
                buffer[4] = (byte)(value >> 24 & 0xFF);
                WriteRequest(ServiceUUID, pwmDutyUuid, buffer, 5);
                this.currentPower = value;
            }
            get
            {
                return currentPower;
            }
        }

        public void UpdateBatteryData()
        {
            buffer[0] = 0;
            WriteRequest( ServiceUUID, batteryDataUuid, buffer, 1);
        }

        public void ReadRequests()
        {
            ReadRequest(ServiceUUID, pwmDutyUuid);
            ReadRequest(ServiceUUID, batteryDataUuid);
        }



        #endregion ORIGIN
        /*
        public DisconectMode diconnectMode { get; private set; }

        public int batteryVoltage { get; private set; }


        */
    }
}
