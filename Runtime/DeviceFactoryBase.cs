using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{
    public abstract class DeviceFactoryBase<T> 
        where T: BleDevice,new()
    {
        public abstract string ServiceUUID { get; }
        public static BleDevice Create(BleDeviceManager manager, string addr)
        {
            var val = new T();
            val.Initialize(manager, addr);
            return val;
        }
    }
}
