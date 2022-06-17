using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BleGadget
{

    public interface IDeviceBuilder
    {
        BleDevice BuildDevice(BleDeviceManager m, string addr);

        bool IsMatchBuilder(List<string> services);

        int builderPriority { get; }

        string scanServiceUuid { get; }
        
    }

    /// <summary>
    /// Device Builder
    /// 各Deviceは RegisterBuilderでサービスのUUIDと実際のオブジェクト作成を紐づけてください 
    /// </summary>
    public static class DeviceBuilderManager
    {
        private static List<IDeviceBuilder> s_deviceBuilders = new List<IDeviceBuilder>();


        public static bool IsMatchBuilder(string maiServiceId, List<string> services)
        {
            foreach(var service in services)
            {
                if(maiServiceId.ToUpper() == service.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }


        public static void RegistBuilder(IDeviceBuilder builder)
        {
            s_deviceBuilders.Add(builder);
        }

        public static string[] GetServices()
        {
            var array = new string[s_deviceBuilders.Count];
            int idx = 0;
            foreach( var builder in s_deviceBuilders)
            {
                array[idx] = builder.scanServiceUuid.ToUpper();
                ++idx;
            }
            return array;
        }

        public static IDeviceBuilder GetBuilder(List<string> services)
        {
            foreach (var builder in s_deviceBuilders)
            {
                if (builder.IsMatchBuilder(services) )
                {
                    return builder;
                }
            }
            return null;
        }

    }

}
