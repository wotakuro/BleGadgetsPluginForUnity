using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BleGadget
{
    // https://github.com/ymmtynk/bCore/wiki/2.1._bCoreServiceCharacteristic
    public class BcoreDevice : BleDevice
    {
        public static readonly string ServiceUUID = "389CAAF0-843F-4d3b-959D-C954CCE14655";

        private static readonly string CharastristicGetBattery = "389CAAF1-843F-4d3b-959D-C954CCE14655";
        private static readonly string CharastristicSetMotorPWM = "389CAAF2-843F-4d3b-959D-C954CCE14655";
        private static readonly string CharastristicSetPortOut = "389CAAF3-843F-4d3b-959D-C954CCE14655";
        private static readonly string CharastristicSetServoPosition = "389CAAF4-843F-4d3b-959D-C954CCE14655";

        private static readonly string CharastristicBurstCommand = "389CAAF5-843F-4d3b-959D-C954CCE14655";
        private static readonly string CharastristicGetFunctions = "389CAAFF-843F-4d3b-959D-C954CCE14655";

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterToBuilder()
        {
            DeviceBuilder.RegistBuilder(ServiceUUID ,(m,addr)=>new BcoreDevice(m,addr) );
        }

        public BcoreDevice(BleDeviceManager m, string addr) : base(m, addr) { }


        public void SetMotorPwm(int idx,int pow)
        {

        }
        public void SetPortOut(int val)
        {

        }

        public void SetServoPosition(int idx, int pow)
        {

        }
        public void SetBurstCommand(int motor1,int motor2,
            int portVal,int servo1,int servo2,int servo3,int servo4)
        {

        }



        private byte[] buffer = new byte[32];

        protected override void OnReadData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }

        protected override void OnNotificateData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }





        /*
        public DisconectMode diconnectMode { get; private set; }

        public int batteryVoltage { get; private set; }


        */
    }
}