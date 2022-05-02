using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{

    /// <summary>
    /// Device Builder
    /// 各Deviceは RegisterBuilderでサービスのUUIDと実際のオブジェクト作成を紐づけてください 
    /// </summary>
    public static class DeviceBuilder
    {
        public delegate BleDevice BuildDeviceDelegate(BleDeviceManager m , string addr);

        public static Dictionary<string, BuildDeviceDelegate> s_builders = new Dictionary<string, BuildDeviceDelegate>();


        public static void RegistBuilder(string serviceUuid, BuildDeviceDelegate builder)
        {
            if (s_builders.ContainsKey(serviceUuid))
            {
                Debug.LogError("All ready exist service " + serviceUuid);
            }
            s_builders.Add(serviceUuid, builder);
        }

        public static string[] GetServices()
        {
            var array = new string[s_builders.Count];
            int idx = 0;
            foreach( var key in s_builders.Keys)
            {
                array[idx] = key;
                ++idx;
            }
            return array;
        }

        public static BuildDeviceDelegate GetBuilder(string service)
        {
            BuildDeviceDelegate builder;
            if(s_builders.TryGetValue(service,out builder))
            {
                return builder;
            }
            return null;
        }

    }

}
