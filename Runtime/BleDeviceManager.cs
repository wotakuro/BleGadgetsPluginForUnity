using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public class BleDeviceManager 
    {
        public static BleDeviceManager Instance { get; private set; } = new BleDeviceManager();

        private Dictionary<string, MabeeeDevice> deviceDictionary;

        private BleDeviceManager() { }

        public void Initialize()
        {
            toio.Ble.Initialize(OnInitialize, OnInitializeFailed);
        }

        private void OnInitialize()
        {
            Debug.Log("OnInitialize");

        }

        private void OnInitializeFailed(string err)
        {
            Debug.LogError(err);
        }

        private void OnFinalize()
        {
            toio.Ble.Finalize();
        }

        public void StartScan()
        {
            Debug.Log("StartScan");
            toio.Ble.StartScan(new string[] { MabeeeDevice.ServiceUUID},
                this.OnFindDevice);
        }

        public void StopScan()
        {
            toio.Ble.StopScan();
        }

        public void GetConnectableDevices(List<MabeeeDevice> devices)
        {
            devices.Clear();
            if (deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                if (device.IsConnect) { continue; }
                devices.Add(device);
            }
        }
        public void GetConnectedDevices(List<MabeeeDevice> devices)
        {

        }

        public void GetAllDevices(List<MabeeeDevice> devices)
        {
            devices.Clear();
            if(deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                devices.Add(device);
            }
        }

        private void OnFindDevice(string addr, string service, int rssi, byte[] data)
        {
            //Debug.Log("OnFindDevice : " + addr + "::" + rssi);
            if(deviceDictionary == null)
            {
                deviceDictionary = new Dictionary<string, MabeeeDevice>();
            }
            MabeeeDevice device;
            if(!deviceDictionary.TryGetValue(addr,out device) ){
                device = new MabeeeDevice(this, addr);
                deviceDictionary.Add(addr, device);
            }
            device.OnFindDevice(service, rssi, data);
        }

        internal void OnConnectDevice(string addr)
        {
            Debug.Log("OnConnectDevice " + addr);
            MabeeeDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
                device.IsConnect = true;
            }
        }
        internal void OnDiscoveredService(string addr, string serviceUuid)
        {
            Debug.Log("OnDiscoveredService " + addr +"::"+serviceUuid);

            MabeeeDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
            }
        }
        internal void OnDiscoveredCharacteristic(string addr, string serviceUuid,string charastristicUuid)
        {
            Debug.Log("OnDiscoveredCharacteristic " + addr + "::" + serviceUuid + "::" + charastristicUuid);
            MabeeeDevice device;
            if (!deviceDictionary.TryGetValue(addr, out device))
            {
            }
        }
        internal void OnDisconnect(string addr)
        {
            Debug.Log("OnDisconnect " + addr);
            MabeeeDevice device;
            if (!deviceDictionary.TryGetValue(addr, out device))
            {
                device.OnDisconnect();
            }
        }
    }
}