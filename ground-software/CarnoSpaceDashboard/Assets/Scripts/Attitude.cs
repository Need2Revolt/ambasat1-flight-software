[System.Serializable]
public class Attitude
{
    public string app_id;
    public string dev_id;
    public string hardware_serial;
    public int port;
    public int counter;
    public string payload_raw;
    public AttitudePayloadFields payload_fields;
    public Metadata metadata;
}