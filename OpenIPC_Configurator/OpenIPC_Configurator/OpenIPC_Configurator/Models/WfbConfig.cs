namespace OpenIPC_Configurator.Models;

public class WfbConfig
{
    public string channel { get; set; } = "";
    public string driver_txpower_override { get; set; } = "";
    public int frequency { get; set; } = 0;
    public int txpower { get; set; }
    public bool stbc { get; set; }
    public int ldpc { get; set; }
    public int mcs_index { get; set; }
    public int fec_k { get; set; }
    public int fec_n { get; set; }
}