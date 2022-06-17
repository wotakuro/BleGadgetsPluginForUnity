using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BleGadget.Devices
{
    public class MabeeeDevice : BleDevice
    {
        public static readonly string ServiceUUID = "B9F5FF00-D813-46C6-8B61-B453EE2C74D9";
        private static readonly string pwmDutyUuid = "B9F53006-D813-46C6-8B61-B453EE2C74D9";
        private static readonly string batteryDataUuid = "B9F51001-D813-46C6-8B61-B453EE2C74D9";
        private static readonly string overCurrentUuid = "B9F51002-D813-46C6-8B61-B453EE2C74D9";
        private static readonly string pioReadUuid = "B9F53003-D813-46C6-8B61-B453EE2C74D9";

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterToBuilder()
        {
            DeviceBuilderManager.RegistBuilder(new Builder() );
        }

        class Builder : IDeviceBuilder
        {
            public int builderPriority => 0;
            public string scanServiceUuid => ServiceUUID;

            public BleDevice BuildDevice(BleDeviceManager m, string addr)
            {
                return new MabeeeDevice(m, addr);
            }

            public bool IsMatchBuilder(List<string> services)
            {
                return DeviceBuilderManager.IsMatchBuilder(scanServiceUuid, services);
            }

        }

        public MabeeeDevice(BleDeviceManager m, string addr) : base(m, addr) { }





        private byte[] buffer = new byte[32];

        protected override void OnReadData(string serviceUuid, string charastristicUuid, byte[] data)
        {
            /*
            string str = ("OnReadData:" + serviceUuid + ":" + charastristicUuid + "\n");
            for (int i = 0; i < data.Length; ++i)
            {
                str += data[i] + "::";
            }
            Debug.Log(str);
            */
        }

        protected override void OnNotificateData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }



        public enum DisconectMode
        {
            Reset,
            Continue
        }

        private int currentPower = 0;
        public void SetOutputPower(int pow)
        {
            if (currentPower == pow)
            {
                return;
            }
            //Debug.Log("Setoutput Power " + value);
            buffer[0] = 1;
            // value
            buffer[1] = (byte)(pow & 0xFF);
            buffer[2] = (byte)(pow >> 8 & 0xFF);
            buffer[3] = (byte)(pow >> 16 & 0xFF);
            buffer[4] = (byte)(pow >> 24 & 0xFF);
            WriteRequest(ServiceUUID, pwmDutyUuid, buffer, 5);
            this.currentPower = pow;
        }

        private void UpdateBatteryData()
        {
            buffer[0] = 0;
            WriteRequest(ServiceUUID, batteryDataUuid, buffer, 1);
        }

        private void ReadRequests()
        {
            ReadRequest(ServiceUUID, pwmDutyUuid);
            ReadRequest(ServiceUUID, batteryDataUuid);
        }



        /*
        public DisconectMode diconnectMode { get; private set; }

        public int batteryVoltage { get; private set; }


        */
    }
}