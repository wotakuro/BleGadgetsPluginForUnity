using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public class BleDeviceManager 
    {
        public static BleDeviceManager Instance { get; private set; } = new BleDeviceManager();

        private Dictionary<string, BleDevice> deviceDictionary;
        private Dictionary<string, List<string>> deviceServices = new Dictionary<string, List<string>>();

        private BleDeviceManager() { }

        private string currntService;
        // serviceUUID必須は仮実装(※toioのBLE自体を少し改変しないと復数機種対応できない
        public void Initialize()
        {
            // [仮実装] serviceUUID
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
            var services = DeviceBuilderManager.GetServices();
            toio.Ble.StartScan( services,
                this.OnFindDevice, OnFindDeviceServices);
        }

        public void StopScan()
        {
            toio.Ble.StopScan();
        }

        public void GetConnectableDevices<T>(List<T> devices) where T : BleDevice
        {
            devices.Clear();
            if (deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                if (device.IsConnect) { continue; }
                var obj = device as T;
                if (obj != null)
                {
                    devices.Add(obj);
                }
            }
        }
        public void GetConnectedDevices<T>(List<T> devices) where T : BleDevice
        {
            devices.Clear();
            if (deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                if (!device.IsConnect) { continue; }
                var obj = device as T;
                if (obj != null)
                {
                    devices.Add(obj);
                }
            }
        }

        public void GetAllDevices<T>(List<T> devices) where T : BleDevice
        {
            devices.Clear();
            if(deviceDictionary == null) { return; }
            foreach (var device in deviceDictionary.Values)
            {
                var obj = device as T;
                if (obj != null)
                {
                    devices.Add(obj);
                }
            }
        }


        private void OnFindDeviceServices(string addr,List<string> services)
        {
            if (!deviceServices.ContainsKey(addr))
            {
                List<string> list = new List<string>(services.Count);
                foreach (var service in services) {
                    list.Add(service.ToUpper());
                }
                deviceServices.Add(addr, list);
            }
        }
        private List<string> GetDeviceServices(string addr)
        {
            List<string> services;
            if (deviceServices.TryGetValue(addr,out services))
            {
                return services;
            }
            return null; 
        }
        
        private void OnFindDevice(string addr, string name, int rssi, byte[] data)
        {
            //Debug.Log("OnFindDevice : " + addr + "::" + rssi);
            if(deviceDictionary == null)
            {
                deviceDictionary = new Dictionary<string, BleDevice>();
            }
            BleDevice device;
            if(!deviceDictionary.TryGetValue(addr,out device) ){
                var builder = DeviceBuilderManager.GetBuilder( this.GetDeviceServices(addr) );
                if (builder != null)
                {
                    device = builder.BuildDevice(this, addr);
                    deviceDictionary.Add(addr, device);
                }
                else
                {
                    Debug.LogError("not Found Service " + addr);
                }
            }
            if (device != null)
            {
                device.OnFindDevice(name, rssi, data);
            }
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