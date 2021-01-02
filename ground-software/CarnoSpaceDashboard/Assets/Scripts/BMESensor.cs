[System.Serializable]
public class BMESensor
{
    public string app_id;
    public string dev_id;
    public string hardware_serial;
    public int port;
    public int counter;
    public string payload_raw;
    public BMESensorPayload payload_fields;
    public Metadata metadata;
}