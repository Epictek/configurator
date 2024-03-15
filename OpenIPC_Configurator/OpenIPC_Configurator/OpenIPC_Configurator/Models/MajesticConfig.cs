namespace OpenIPC_Configurator.Models;

public class MajesticConfig
{
    public System System { get; set; }
    public Isp Isp { get; set; }
    public Image Image { get; set; }
    public Osd Osd { get; set; }
    public NightMode NightMode { get; set; }
    public Records Records { get; set; }
    public Video Video0 { get; set; }
    public Video Video1 { get; set; }
    public Jpeg Jpeg { get; set; }
    public Mjpeg Mjpeg { get; set; }
    public Audio Audio { get; set; }
    public Rtsp Rtsp { get; set; }
    public Hls Hls { get; set; }
    public Youtube Youtube { get; set; }
    public MotionDetect MotionDetect { get; set; }
    public Ipeye Ipeye { get; set; }
    public Netip Netip { get; set; }
    public Onvif Onvif { get; set; }
    public Watchdog Watchdog { get; set; }
}

public class System
{
    public string WebAdmin { get; set; }
    public int Buffer { get; set; }
}

public class Isp
{
    public int BlkCnt { get; set; }
    public bool LowDelay { get; set; }
}

public class Image
{
    public bool Mirror { get; set; }
    public bool Flip { get; set; }
    public string Rotate { get; set; }
    public int Contrast { get; set; }
    public int Hue { get; set; }
    public int Saturation { get; set; }
    public int Luminance { get; set; }
}

public class Osd
{
    public bool Enabled { get; set; }
    public string Template { get; set; }
}

public class NightMode
{
    public bool Enabled { get; set; }
    public bool NightAPI { get; set; }
    public int BacklightPin { get; set; }
    public bool IrSensorPinInvert { get; set; }
    public bool IrSoftSensor { get; set; }
    public int IrCutPin1 { get; set; }
    public int IrCutPin2 { get; set; }
    public int DncDelay { get; set; }
}

public class Records
{
    public bool Enabled { get; set; }
    public string Path { get; set; }
    public int MaxUsage { get; set; }
}

public class Video
{
    public bool Enabled { get; set; }
}

public class Jpeg
{
    public bool Enabled { get; set; }
}

public class Mjpeg
{
    public string Size { get; set; }
    public int Fps { get; set; }
    public int Bitrate { get; set; }
}

public class Audio
{
    public bool Enabled { get; set; }
    public int Volume { get; set; }
    public int Srate { get; set; }
    public string Codec { get; set; }
    public bool OutputEnabled { get; set; }
    public int SpeakerPin { get; set; }
    public bool SpeakerPinInvert { get; set; }
}

public class Rtsp
{
    public bool Enabled { get; set; }
    public int Port { get; set; }
}

public class Hls
{
    public bool Enabled { get; set; }
}

public class Youtube
{
    public bool Enabled { get; set; }
}

public class MotionDetect
{
    public bool Enabled { get; set; }
    public bool Visualize { get; set; }
    public bool Debug { get; set; }
}

public class Ipeye
{
    public bool Enabled { get; set; }
}

public class Netip
{
    public bool Enabled { get; set; }
}

public class Onvif
{
    public bool Enabled { get; set; }
}

public class Watchdog
{
    public bool Enabled { get; set; }
    public int Timeout { get; set; }
}