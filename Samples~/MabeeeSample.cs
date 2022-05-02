using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MabeeeSample : MonoBehaviour
{
    [SerializeField]
    private Button startScanBtn;
    [SerializeField]
    private Button connectBtn;

    [SerializeField]
    private Slider slider;


    [SerializeField]
    private Text infoArea;

    private void Start()
    {
        startScanBtn.onClick.AddListener(StartScan);
        connectBtn.onClick.AddListener(Connect);

        BleGadget.BleDeviceManager.Instance.Initialize( BleGadget.MabeeeDevice.ServiceUUID);
    }



    private void StartScan()
    { 
        BleGadget.BleDeviceManager.Instance.StartScan();
    }

    private void Connect()
    {
        var list = new List<BleGadget.MabeeeDevice>();
        BleGadget.BleDeviceManager.Instance.GetConnectableDevices(list);
        Debug.Log("Connect Device " + list.Count);
        if(list.Count > 0)
        {
            list[0].Connect();
        }
    }

    private void Update()
    {
        var list = new List<BleGadget.MabeeeDevice>();
        BleGadget.BleDeviceManager.Instance.GetAllDevices(list);

        string txt = "Devices " + list.Count + "\n";
        for(int i = 0; i < list.Count; ++i)
        {
            txt += list[i].Address + ":" + list[i].Rssi + "::" + list[i].IsConnect + "\n";
        }
        this.infoArea.text = txt;

        // update pwoer
        BleGadget.BleDeviceManager.Instance.GetConnectedDevices(list);
        for (int i = 0; i < list.Count; ++i)
        {
            var mabeee = list[i];
            mabeee.OutputPower = (int)slider.value;
        }
    }


}
