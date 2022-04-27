using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mabeee
{
    public class MabeeeDeviceManager 
    {
        private Dictionary<string, MabeeeDevice> deviceDictionary;

        private MabeeeDeviceManager() { }

        public void Init()
        {
            toio.Ble.Initialize( ()=> { });
        }

        public void StartScan()
        {
        }

        public void StopScan()
        {
        }

        public void GetConnectableDevices(List<MabeeeDevice> devices)
        {
        }
        public void GetConnectedDevices(List<MabeeeDevice> devices)
        {

        }

        public void GetAllDevices(List<MabeeeDevice> devices)
        {

        }
    }
}