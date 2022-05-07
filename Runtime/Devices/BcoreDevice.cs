using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BleGadget.Devices
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
            DeviceBuilder.RegistBuilder(ServiceUUID, (m, addr) => new BcoreDevice(m, addr));
        }



        private byte[] buffer = new byte[32];
        private int[] motorPower = new int[2];
        private int portOut;
        private int[] servoParam = new int[4];

        public BcoreDevice(BleDeviceManager m, string addr) : base(m, addr)
        {
            this.portOut = 0x7FFFFFFF;
            for (int i = 0; i < motorPower.Length; ++i)
            {
                motorPower[i] = 0x7FFFFFFF;
            }
            for (int i = 0; i < servoParam.Length; ++i)
            {
                servoParam[i] = 0x7FFFFFFF;
            }
        }



        /// <summary>
        /// モーター出力設定
        /// ※同じ値が連続して指定された場合は更新しません
        /// </summary>
        /// <param name="idx">モーターの指定 . 0-1で指定します</param>
        /// <param name="pow">モーターの出力。-0x80～0x7fで指定します</param>
        /// <param name="forceUpdate">強制アップデートフラグ</param>
        public void SetMotorPwm(int idx, int pow, bool forceUpdate = false)
        {
            if (idx < 0 || idx >= motorPower.Length)
            {
                Debug.LogError("SetMotorPwm Invalid index " + idx);
                return;
            }
            if (pow > 0x7f || pow < -0x80)
            {
                Debug.LogError("SetMotorPwm Invalid Power " + pow);
                return;
            }
            buffer[0] = (byte)idx;
            buffer[1] = ConvertMoterPower(pow);

            if (forceUpdate || motorPower[idx] != pow)
            {
                this.WriteRequest(ServiceUUID, CharastristicSetMotorPWM, buffer, 2);
            }
            motorPower[idx] = pow;
        }

        private static byte ConvertMoterPower(int pow)
        {
            return (byte)(0x80 + pow);
        }

        /// <summary>
        /// ポート出力のOn/Off切り替え
        /// ※同じ値が連続して指定された場合は更新しません
        /// </summary>
        /// <param name="flag">bit0[0x01]/bit1[0x02]/bit2[0x04]/bit3[0x08]という形で設定します</param>
        /// <param name="forceUpdate">強制アップデートフラグ</param>
        public void SetPortOut(int flag, bool forceUpdate = false)
        {
            buffer[0] = (byte)flag;
            if (forceUpdate || flag != this.portOut)
            {
                this.WriteRequest(ServiceUUID, CharastristicSetPortOut, buffer, 1);
            }
            this.portOut = flag;
        }

        /// <summary>
        /// サーボの位置を更新します
        /// ※同じ値が連続して指定された場合は更新しません
        /// </summary>
        /// <param name="idx">対象のサーボモーター(0～3)</param>
        /// <param name="position">ポジションの設定 (0x00～0xFF)</param>
        /// <param name="forceUpdate">強制アップデートフラグ</param>
        public void SetServoPosition(int idx, int position, bool forceUpdate = false)
        {
            if (idx < 0 || idx >= servoParam.Length)
            {
                Debug.LogError("SetServoPosition Invalid index " + idx);
                return;

            }
            if (position < 0 || position > 0xFF)
            {
                Debug.LogError("SetServoPosition Invalid position " + position);
                return;
            }

            buffer[0] = (byte)idx;
            buffer[1] = (byte)position;
            if (forceUpdate || servoParam[idx] != position)
            {
                this.WriteRequest(ServiceUUID, CharastristicSetServoPosition,
                    buffer, 2);
            }
            servoParam[idx] = position;
        }

        /// <summary>
        /// 一気に更新するコマンド呼び出します
        /// ※すでに同じ値が指定された場合は更新しません
        /// </summary>
        /// <param name="motor1">モーター１の出力。-0x80～0x7fで指定します</param>
        /// <param name="motor2">モーター２の出力。-0x80～0x7fで指定します</param>
        /// <param name="portFlag">ポートフラグ。bit0[0x01]/bit1[0x02]/bit2[0x04]/bit3[0x08]という形で設定します</param>
        /// <param name="servo1">サーボモーター１の位置更新。ポジションの設定 (0x00～0xFF)</param>
        /// <param name="servo2">サーボモーター２の位置更新。ポジションの設定 (0x00～0xFF)</param>
        /// <param name="servo3">サーボモーター３の位置更新。ポジションの設定 (0x00～0xFF)</param>
        /// <param name="servo4">サーボモーター４の位置更新。ポジションの設定 (0x00～0xFF)</param>
        /// <param name="forceUpdate"></param>
        public void SetBurstCommand(int motor1, int motor2,
            int portFlag, int servo1, int servo2, int servo3, int servo4,
            bool forceUpdate = false)
        {
            buffer[0] = ConvertMoterPower(motor1);
            buffer[1] = ConvertMoterPower(motor2);
            buffer[2] = (byte)portFlag;
            buffer[3] = (byte)servo1;
            buffer[4] = (byte)servo2;
            buffer[5] = (byte)servo3;
            buffer[6] = (byte)servo4;

            if (forceUpdate ||
                (this.motorPower[0] != motor1 ||
                this.motorPower[1] != motor2 ||
                this.portOut != portFlag ||
                this.servoParam[0] != servo1 ||
                this.servoParam[1] != servo2 ||
                this.servoParam[2] != servo3 ||
                this.servoParam[3] != servo4))
            {
                this.WriteRequest(ServiceUUID, CharastristicBurstCommand,
                    buffer, 7);
            }
            this.motorPower[0] = motor1;
            this.motorPower[1] = motor2;
            this.portOut = portFlag;
            this.servoParam[0] = servo1;
            this.servoParam[1] = servo2;
            this.servoParam[2] = servo3;
            this.servoParam[3] = servo4;
        }




        protected override void OnReadData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }

        protected override void OnNotificateData(string serviceUuid, string charastristicUuid, byte[] data)
        {
        }

        public static int GetPortValue(bool p1,bool p2,bool p3,bool p4)
        {
            int val = 0;
            if (p1)
            {
                val |= 0x01;
            }
            if (p2)
            {
                val |= 0x02;
            }
            if (p3)
            {
                val |= 0x04;
            }
            if (p4)
            {
                val |= 0x08;
            }
            return val;
        }


    }
}