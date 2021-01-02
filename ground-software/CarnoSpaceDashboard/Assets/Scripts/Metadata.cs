using System.Collections.Generic;
[System.Serializable]
public class Metadata
{
    public string time;
    public double frequency;
    public string modulation;
    public string data_rate;
    public int airtime;
    public string coding_rate;
    public List<Gateway> gateways;
}