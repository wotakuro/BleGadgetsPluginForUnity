using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BleGadget;
using BleGadget.Devices;

namespace BleGadget.Samples
{
    public class BcoreSample : MonoBehaviour
    {

        // BLEデバイススキャン開始ボタン
        [SerializeField]
        private Button startScanBtn;
        // 見つかったBLEデバイスに接続するボタン
        [SerializeField]
        private Button connectBtn;

        // 一気に値を設定するBurstコマンドを利用するか？
        [SerializeField]
        private Toggle burstCommad;

        [SerializeField]
        private Slider[] motorOutputSliders;

        [SerializeField]
        private Slider[] servoPositionSliders;

        [SerializeField]
        private Toggle[] portOutputToggles;

        [SerializeField]
        private Text debugText;

        // Start is called before the first frame update
        void Start()
        {
            // 各ボタンを押したときの処理を設定します
            startScanBtn.onClick.AddListener(OnClickStartScanButton);
            connectBtn.onClick.AddListener(OnClickConnectButton);
            // Bleの初期化処理をします
            BleDeviceManager.Instance.Initialize();
        }

        private void OnClickStartScanButton()
        {
            // DeviceのServiceを指定してスキャンを開始します。
            // 現在は1つまでしか接続できません
            BleDeviceManager.Instance.StartScan(BcoreDevice.ServiceUUID);
        }
        private void OnClickConnectButton()
        {
            // Scanして発見したbcoreデバイスを列挙します
            var list = new List<BcoreDevice>();
            BleDeviceManager.Instance.GetConnectableDevices(list);
            Debug.Log("Connect Device " + list.Count);
            for (int i = 0; i < list.Count; ++i)
            {
                // Deviceへ接続します
                list[i].Connect();
            }
        }

        private void Update()
        {
            // Mabeeデバイスをリストアップします
            var list = new List<BcoreDevice>();
            BleDeviceManager.Instance.GetAllDevices(list);
            string txt = "Devices " + list.Count + "\n";
            for (int i = 0; i < list.Count; ++i)
            {
                txt += list[i].Address + ":" + list[i].Rssi + "::" + list[i].IsConnect + "\n";
            }
            this.debugText.text = txt;

            // Sliderの値を接続中のbcoreの出力値にセットします
            BleDeviceManager.Instance.GetConnectedDevices(list);
            for (int i = 0; i < list.Count; ++i)
            {
                var bcore = list[i];
                if (burstCommad.isOn)
                {
                    int portVal = BcoreDevice.GetPortValue(portOutputToggles[0].isOn,
                            portOutputToggles[1].isOn,
                            portOutputToggles[2].isOn,
                            portOutputToggles[3].isOn);
                    bcore.SetBurstCommand(
                        (int)motorOutputSliders[0].value,
                        (int)motorOutputSliders[1].value,
                        portVal,
                        (int)servoPositionSliders[0].value,
                        (int)servoPositionSliders[1].value,
                        (int)servoPositionSliders[2].value,
                        (int)servoPositionSliders[3].value ) ;
                }
                else
                {
                    for(int j = 0; j < motorOutputSliders.Length; ++j)
                    {
                        int power = (int)motorOutputSliders[j].value;
                        bcore.SetMotorPwm(j, power);
                    }
                    for (int j = 0; j < servoPositionSliders.Length; ++j)
                    {
                        int position = (int)servoPositionSliders[j].value;
                        bcore.SetServoPosition(j, position);
                    }

                    bcore.SetPortOut(
                        BcoreDevice.GetPortValue(portOutputToggles[0].isOn,
                            portOutputToggles[1].isOn,
                            portOutputToggles[2].isOn,
                            portOutputToggles[3].isOn));
                }
            }
        }

    }
}