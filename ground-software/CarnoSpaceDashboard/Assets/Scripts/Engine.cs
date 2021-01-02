using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
//using Newtonsoft.Json;


public class Engine : MonoBehaviour
{
    public string mqttServerHostname;
    public string username;
    public string password;

    private MqttClient client;

    //metadata GUI objects
    private Label lastSeen;
    private Label lastGatewayId;
    private Label rssi;
    //metadata values
    private bool metadataRefresh = false;
    private string lastSeenValue;
    private string lastGatewayIdValue;
    private int rssiValue;

    //telemetry GUI objects
    private Label voltage;
    private Label bootCount;
    private Label lsm9ds1Status;
    private Label bme680Status;
    //telemetry data
    private bool telemetryRefresh = false;
    private int voltageValue;
    private int bootCountValue;
    private string lsm9ds1StatusValue;
    private string bme680StatusValue;

    //attitude GUI objects
    private Label gyro;
    private Label accelerometer;
    private Label magnetometer;
    //attitude data
    private bool attitudeRefresh = false;
    private double gyroXValue;
    private double gyroYValue;
    private double gyroZValue;
    private double accelerationXValue;
    private double accelerationYValue;
    private double accelerationZValue;
    private double magnetometerXValue;
    private double magnetometerYValue;
    private double magnetometerZValue;

    //sensor GUI objects
    private Label gasHeaterDuration;
    private Label gasHeaterTemperature;
    private Label gasResistance;
    private Label humidity;
    private Label humidityOversampling;
    private Label temperature;
    private Label temperatureOversampling;
    private Label pressure;
    private Label pressureOversampling;
    private Label iirCoefficient;

    //sensor data
    private int gasHeaterDurationValue;
    private int gasHeaterTemperatureValue;
    private int gasResistanceValue;
    private double humidityValue;
    private int humidityOversamplingValue;
    private int iirCoefficientValue;
    private double pressureValue;
    private int pressureOversamplingValue;
    private double temperatureValue;
    private int temperatureOversamplingValue;
    private bool sensorRefresh = false;

    //funky stuff
    Transform Ambasat1ModelTransform;

    void Start()
    {
        Debug.Log("At least i'm alive");

        TextAsset textFile = (TextAsset)Resources.Load("TTNLoginInfo", typeof(TextAsset));
        TTNLoginInfo loginInfo = JsonUtility.FromJson<TTNLoginInfo>(textFile.text);
        client = new MqttClient(loginInfo.mqttServerHostname);

        client.MqttMsgSubscribed += client_MqttMsgSubscribed;
        client.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
        client.MqttMsgPublished += client_MqttMsgPublished;
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        Debug.Log("Callbacks setup done");

        client.Connect(Guid.NewGuid().ToString(), loginInfo.username, loginInfo.password);
        Debug.Log("Connection done!");

        string[] topic = { "+/devices/+/events/activations", "+/devices/+/up" };
        byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };
        client.Subscribe(topic, qosLevels);
        Debug.Log("Subscription done!");

        Ambasat1ModelTransform = GameObject.Find("Ambasat1Model").transform;
    }

    void Update()
    {
        if(metadataRefresh)
        {
            lastSeen.text = $"Last seen: {lastSeenValue}";
            lastGatewayId.text = $"Gateway ID: {lastGatewayIdValue}";
            rssi.text = $"RSSI: {rssiValue}";

            metadataRefresh = false;
        }

        if(telemetryRefresh) {
            //Telemetry fields
            voltage.text = $"Voltage: {voltageValue} millivolts";
            bootCount.text = $"Boot count: {bootCountValue}";
            lsm9ds1Status.text = $"LSM9DS1 status: {lsm9ds1StatusValue}";
            bme680Status.text = $"BME680 status: {bme680StatusValue}";
            
            telemetryRefresh = false;
        }

        if(attitudeRefresh)
        {
            //Attitude fields
            gyro.text = $"X: {gyroXValue}\t\tY: {gyroYValue}\t\tZ: {gyroZValue}";
            accelerometer.text = $"X: {accelerationXValue}\t\tY: {accelerationYValue}\t\tZ: {accelerationZValue}";
            magnetometer.text = $"X: {magnetometerXValue}\t\t\tY: {magnetometerYValue}\t\t\tZ: {magnetometerZValue}";

            Quaternion calculatedAttitude = Madgwick.MadgwickQuaternionUpdate(accelerationXValue, accelerationYValue, accelerationZValue, gyroXValue, gyroYValue, gyroZValue, magnetometerXValue, magnetometerYValue, magnetometerZValue);
            Ambasat1ModelTransform.rotation = Quaternion.Inverse(calculatedAttitude);
            //Ambasat1ModelTransform.rotation = calculatedAttitude;
            attitudeRefresh = false;
        }

        if(sensorRefresh)
        {
            gasHeaterDuration.text = $"Gas heater duration: {gasHeaterDurationValue}";
            gasHeaterTemperature.text = $"Gas heater temperature: {gasHeaterTemperatureValue}";
            gasResistance.text = $"Gas resistance: {gasResistanceValue}";
            humidity.text = $"Humidity: {humidityValue}";
            humidityOversampling.text = $"Humidity oversampling: {humidityOversamplingValue}";
            temperature.text = $"Temperature: {temperatureValue}";
            temperatureOversampling.text = $"Temperature oversampling: {temperatureOversamplingValue}";
            pressure.text = $"Pressure: {pressureValue}";
            pressureOversampling.text = $"Pressure oversampling: {pressureOversamplingValue}";
            iirCoefficient.text = $"IIR coefficient: {iirCoefficientValue}";

            sensorRefresh = false;
        }
    }

    void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
    {
        // write your code
        //called when unsubscribing from the queue?
        Debug.Log("client_MqttMsgUnsubscribed");
        Debug.Log("" + e);
    }

    void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
    {
        // write your code
        //called when subscribing to the queue?
        Debug.Log("client_MqttMsgSubscribed");
        Debug.Log("" + e);
    }

    void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
    {
        // write your code
        Debug.Log("client_MqttMsgPublished");
        Debug.Log("" + e);
    }

    public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        //Debug.Log("client_MqttMsgPublishReceived");
        //Debug.Log(new string(Encoding.UTF8.GetChars(e.Message)));
        string jsonString = new string(Encoding.UTF8.GetChars(e.Message));

        if (jsonString.Contains("\"port\":1,"))
        {
            Telemetry telemetry = JsonUtility.FromJson<Telemetry>(jsonString);

            //TODO metadata duplicate across the 3 payloads... not good....
            lastSeenValue = telemetry.metadata.time;
            lastGatewayIdValue = telemetry.metadata.gateways[0].gtw_id; //TODO multiple gateways
            rssiValue = telemetry.metadata.gateways[0].rssi;  //TODO multiple gateways
            metadataRefresh = true;

            //actual telemetry data
            voltageValue = telemetry.payload_fields.milli_volts;
            bootCountValue = telemetry.payload_fields.boot_count;
            //lsm9ds1StatusValue
            if (telemetry.payload_fields.LSM9DS1_found)
            {
                if (telemetry.payload_fields.LSM9DS1_active)
                {
                    lsm9ds1StatusValue = "working correctly";
                }
                else
                {
                    lsm9ds1StatusValue = "sensor found but not active";
                }

            }
            else
            {
                lsm9ds1StatusValue = "sensor not found";
            }
            //bme680StatusValue
            if (telemetry.payload_fields.mission_sensor_found)
            {
                if (telemetry.payload_fields.mission_sensor_active)
                {
                    bme680StatusValue = "working correctly";
                }
                else
                {
                    bme680StatusValue = "sensor found but not active";
                }

            }
            else
            {
                bme680StatusValue = "sensor not found";
            }

            //notify new data is available
            telemetryRefresh = true;
        }
        else if (jsonString.Contains("\"port\":2,"))
        {
            Attitude attitude = JsonUtility.FromJson<Attitude>(jsonString);

            //TODO metadata duplicate across the 3 payloads... not good....
            lastSeenValue = attitude.metadata.time;
            lastGatewayIdValue = attitude.metadata.gateways[0].gtw_id; //TODO multiple gateways
            rssiValue = attitude.metadata.gateways[0].rssi;  //TODO multiple gateways
            metadataRefresh = true;

            gyroXValue = attitude.payload_fields.gyro_x;
            gyroYValue = attitude.payload_fields.gyro_y;
            gyroZValue = attitude.payload_fields.gyro_z;

            accelerationXValue = attitude.payload_fields.acceleration_x;
            accelerationYValue = attitude.payload_fields.acceleration_y;
            accelerationZValue = attitude.payload_fields.acceleration_z;

            magnetometerXValue = attitude.payload_fields.magnetic_x;
            magnetometerYValue = attitude.payload_fields.magnetic_y;
            magnetometerZValue = attitude.payload_fields.magnetic_z;

            attitudeRefresh = true;
        }
        else if (jsonString.Contains("\"port\":5,"))
        {
            Debug.Log(jsonString);
            BMESensor sensor = JsonUtility.FromJson<BMESensor>(jsonString);

            Debug.Log(sensor.payload_raw);
            Debug.Log(sensor.payload_fields);

            //TODO metadata duplicate across the 3 payloads... not good....
            lastSeenValue = sensor.metadata.time;
            lastGatewayIdValue = sensor.metadata.gateways[0].gtw_id; //TODO multiple gateways
            rssiValue = sensor.metadata.gateways[0].rssi;  //TODO multiple gateways
            metadataRefresh = true;

            //sensor data
            gasHeaterDurationValue = sensor.payload_fields.gas_heater_duration;
            gasHeaterTemperatureValue = sensor.payload_fields.gas_heater_temperature;
            gasResistanceValue = sensor.payload_fields.gas_resistance;
            humidityValue = sensor.payload_fields.humidity;
            humidityOversamplingValue = sensor.payload_fields.humidity_oversampling;
            iirCoefficientValue = sensor.payload_fields.iir_coefficient;
            pressureValue = sensor.payload_fields.pressure;
            pressureOversamplingValue = sensor.payload_fields.pressure_oversampling;
            temperatureValue = sensor.payload_fields.temperature;
            temperatureOversamplingValue = sensor.payload_fields.temperature_oversampling;

            sensorRefresh = true;
        }

        else
        {
            //TODO unrecognized payload
            Debug.Log(new string(Encoding.UTF8.GetChars(e.Message)));
        }
    }

    /*
     * Find object reference for the label to update later when data arrives
     */
    private void OnEnable()
    {
        var visualRootElement = GetComponent<UIDocument>().rootVisualElement;

        //Telemetry fields
        lastSeen = visualRootElement.Q<Label>("lastSeen");
        lastGatewayId = visualRootElement.Q<Label>("lastGatewayId");
        rssi = visualRootElement.Q<Label>("RSSI");
        voltage = visualRootElement.Q<Label>("voltage");
        bootCount = visualRootElement.Q<Label>("bootCount");
        lsm9ds1Status = visualRootElement.Q<Label>("lsm9ds1Status");
        bme680Status = visualRootElement.Q<Label>("bme680Status");

        //Attitude fields
        gyro = visualRootElement.Q<Label>("gyro");
        accelerometer = visualRootElement.Q<Label>("accelerometer");
        magnetometer = visualRootElement.Q<Label>("magnetometer");

        //Sensor fields
        gasHeaterDuration = visualRootElement.Q<Label>("gasHeaterDuration");
        gasHeaterTemperature = visualRootElement.Q<Label>("gasHeaterTemperature");
        gasResistance = visualRootElement.Q<Label>("gasResistance");
        humidity = visualRootElement.Q<Label>("humidity");
        humidityOversampling = visualRootElement.Q<Label>("humidityOversampling");
        temperature = visualRootElement.Q<Label>("temperature");
        temperatureOversampling = visualRootElement.Q<Label>("temperatureOversampling");
        pressure = visualRootElement.Q<Label>("pressure");
        pressureOversampling = visualRootElement.Q<Label>("pressureOversampling");
        iirCoefficient = visualRootElement.Q<Label>("iirCoefficient");
    }
}
