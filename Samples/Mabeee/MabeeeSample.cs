using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BleGadget;
using BleGadget.Devices;

namespace BleGadget.Samples
{
    public class MabeeeSample : MonoBehaviour
    {
        // BLEデバイススキャン開始ボタン
        [SerializeField]
        private Button startScanBtn;
        // 見つかったBLEデバイスに接続するボタン
        [SerializeField]
        private Button connectBtn;
        // Mabeeeの出力用のバー
        [SerializeField]
        private Slider outputPowSlider;


        [SerializeField]
        private Text infoArea;

        private void Start()
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
            BleDeviceManager.Instance.StartScan();
        }

        private void OnClickConnectButton()
        {
            // Scanして発見したMabeeeデバイスを列挙します
            var list = new List<MabeeeDevice>();
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
            var list = new List<MabeeeDevice>();
            BleDeviceManager.Instance.GetAllDevices(list);
            string txt = "Devices " + list.Count + "\n";
            for (int i = 0; i < list.Count; ++i)
            {
                txt += list[i].Address + ":" + list[i].Rssi + "::" + list[i].IsConnect + "\n";
            }
            this.infoArea.text = txt;

            // Sliderの値を接続中のMabeeeの出力値にセットします
            BleDeviceManager.Instance.GetConnectedDevices(list);
            for (int i = 0; i < list.Count; ++i)
            {
                var mabeee = list[i];
                mabeee.SetOutputPower( (int)outputPowSlider.value );
            }
        }


    }
}