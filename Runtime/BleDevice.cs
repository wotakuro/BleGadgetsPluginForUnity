using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{

    /// <summary>
    /// BLEデバイスのベース。
    /// これを派生したクラスを作って下しあ
    /// </summary>
    public abstract class BleDevice
    {

        private List<string> services;
        private Dictionary<string,string> charastrics;

        public BleDevice(BleDeviceManager m, string addr)
        {
            this.manager = m;
            this.Address = addr;
            this.services = new List<string>();
            this.charastrics = new Dictionary<string, string>();
        }
        

        public void OnFindDevice(string name, int rssi, byte[] data)
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


        protected abstract void OnNotificateData(string serviceUuid, string charastristicUuid, byte[] data);
        protected abstract void OnReadData(string serviceUuid, string charastristicUuid, byte[] data);


        internal void OnDisconnect()
        {
            this.IsConnect = false;
            this.services.Clear();
            this.charastrics.Clear();
        }

        public virtual void OnDiscoverService(string serviceUuid)
        {

        }
        public virtual void OnDiscoverCharastristic(string serviceUuid,string charastristicUuid)
        {
        }

        protected void ReadRequest(string serviceUuid, string charastristicUuid)
        {
            toio.Ble.ReadCharacteristic(this.Address,serviceUuid,charastristicUuid, OnReadData);
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

    }
}
