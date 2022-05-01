using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public class BleDeviceManager 
    {
        public static BleDeviceManager Instance { get; private set; } = new BleDeviceManager();

        private Dictionary<string, BleDevice> deviceDictionary;

        private BleDeviceManager() { }

        public void Initialize()
        {
            toio.Ble.Initialize(OnInitialize, OnInitializeFailed);
        }

        private void OnInitialize()
        {
            Debug.Log("OnInitialize");
            BehaviourProxy.Initialize(OnUpdate,OnFinalize);
        }

        private void OnInitializeFailed(string err)
        {
            Debug.LogError(err);
        }

        private void OnUpdate()
        {

        }

        private void OnFinalize()
        {
            toio.Ble.Finalize();
        }

        public void StartScan()
        {
            Debug.Log("StartScan");
            toio.Ble.StartScan(new string[] { BleDevice.ServiceUUID},
                this.OnFindDevice);
        }

        public void StopScan()
        {
            toio.Ble.StopScan();
        }

        public void GetConnectableDevices(List<BleDevice> devices)
        {
            devices.Clear();
            if (deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                if (device.IsConnect) { continue; }
                devices.Add(device);
            }
        }
        public void GetConnectedDevices(List<BleDevice> devices)
        {
            devices.Clear();
            if (deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                if (!device.IsConnect) { continue; }
                devices.Add(device);
            }
        }

        public void GetAllDevices(List<BleDevice> devices)
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
                deviceDictionary = new Dictionary<string, BleDevice>();
            }
            BleDevice device;
            if(!deviceDictionary.TryGetValue(addr,out device) ){
                device = new BleDevice();
                device.Initialize(this, addr);
                deviceDictionary.Add(addr, device);
            }
            device.OnFindDevice(service, rssi, data);
        }

        internal void OnConnectDevice(string addr)
        {
            Debug.Log("OnConnectDevice " + addr);
            BleDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
                device.IsConnect = true;
            }
        }
        internal void OnDiscoveredService(string addr, string serviceUuid)
        {
            Debug.Log("OnDiscoveredService " + addr +"::"+serviceUuid);

            BleDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
                device.OnDiscoverService(serviceUuid);
            }
        }
        internal void OnDiscoveredCharacteristic(string addr, string serviceUuid,string charastristicUuid)
        {
            Debug.Log("OnDiscoveredCharacteristic " + addr + "::" + serviceUuid + "::" + charastristicUuid);
            BleDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
                device.OnDiscoverCharastristic(serviceUuid,charastristicUuid);
            }
        }
        internal void OnDisconnect(string addr)
        {
            BleDevice device;
            if (deviceDictionary.TryGetValue(addr, out device))
            {
                device.OnDisconnect();
            }
        }


    }
}