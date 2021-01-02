using System.Collections;
using UnityEngine;

public class EngineTest : MonoBehaviour
{

    void Start()
    {
        /*
        string jsonString = "{\"app_id\":\"carno_test\",\"dev_id\":\"carno_satellite\",\"hardware_serial\":\"4C7D29BDD3A0B1A2\",\"port\":5,\"counter\":564,\"payload_raw\":\"Bd4AAen5HAEAAF7kREIAlgFA\",\"payload_fields\":{\"gas_heater_duration\":150,\"gas_heater_temperature\":320,\"gas_resistance\":24292,\"humidity\":71.69,\"humidity_oversampling\":8,\"iir_coefficient\":3,\"pressure\":1254.33,\"pressure_oversampling\":8,\"temperature\":15.02,\"temperature_oversampling\":8},\"metadata\":{\"time\":\"2021-01-04T12:27:16.349462259Z\",\"frequency\":867.9,\"modulation\":\"LORA\",\"data_rate\":\"SF7BW125\",\"airtime\":71936000,\"coding_rate\":\"4/5\",\"gateways\":[{\"gtw_id\":\"carnospace_groundstation\",\"timestamp\":3962045812,\"time\":\"2021-01-04T12:27:16Z\",\"channel\":0,\"rssi\":-81,\"snr\":9.5,\"rf_chain\":0}]}}";
        BMESensor sensor = JsonUtility.FromJson<BMESensor>(jsonString);
        System.Diagnostics.Debug.Write(sensor);
        */
        /*
        TTNLoginInfo logininfo = new TTNLoginInfo();
        logininfo.mqttServerHostname = "eu.thethings.network";
        logininfo.username = "changeme";
        logininfo.password = "secret";

        string data = JsonUtility.ToJson(logininfo);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/TTNLoginInfo.json", data);
        */
        TextAsset textFile = (TextAsset)Resources.Load("TTNLoginInfo", typeof(TextAsset));
        //System.IO.StringReader textStream = new System.IO.StringReader(textFile.text);
        //string config = textStream.ToString();
        TTNLoginInfo loginInfo = JsonUtility.FromJson<TTNLoginInfo>(textFile.text);
        System.Diagnostics.Debug.Write(loginInfo);
    }
}
