using System;
using System.Collections.Generic;

namespace OpenIPC_Configurator.Models;

public class MajesticConfig
    {
        public SystemConfig System { get; set; }
        public IspConfig Isp { get; set; }
        public ImageConfig Image { get; set; }
        public VideoConfig Video0 { get; set; } 
        public VideoConfig Video1 { get; set; }
        public JpegConfig Jpeg { get; set; }
        public OsdConfig Osd { get; set; }
        public AudioConfig Audio { get; set; }
        public RtspConfig Rtsp { get; set; }
        public NightModeConfig NightMode { get; set; }
        public MotionDetectConfig MotionDetect { get; set; }
        public RecordsConfig Records { get; set; }
        public OutgoingConfig Outgoing { get; set; }
        public WatchdogConfig Watchdog { get; set; }
        public HlsConfig Hls { get; set; }
        public OnvifConfig Onvif { get; set; }
        public IpeyeConfig Ipeye { get; set; }
        public YoutubeConfig Youtube { get; set; }
        public NetipConfig Netip { get; set; }
        public CloudConfig Cloud { get; set; }
    }

    public class SystemConfig
    {
        public int WebPort { get; set; }
        public int HttpsPort { get; set; }
        public string LogLevel { get; set; }
        public int Buffer { get; set; }
        public bool Plugins { get; set; }
    }

    public class IspConfig
    {
        public string AntiFlicker { get; set; }
    }

    public class ImageConfig
    {
        public bool Mirror { get; set; }
        public bool Flip { get; set; }
        public int Rotate { get; set; }
        public int Contrast { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Luminance { get; set; }
    }

    public class VideoConfig
    {
        public bool Enabled { get; set; }
        public string Codec { get; set; }
        public int Fps { get; set; }
        public int Bitrate { get; set; }
        public string RcMode { get; set; }
        public int GopSize { get; set; }
        public string Size { get; set; }
    }

    public class JpegConfig
    {
        public bool Enabled { get; set; }
        public int Qfactor { get; set; }
        public int Fps { get; set; }
    }

    public class OsdConfig
    {
        public bool Enabled { get; set; }
        public string Font { get; set; }
        public string Template { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }

    public class AudioConfig
    {
        public bool Enabled { get; set; }
        public int Volume { get; set; }
        public int Srate { get; set; }
        public string Codec { get; set; }
        public bool OutputEnabled { get; set; }
        public int OutputVolume { get; set; }
    }

    public class RtspConfig
    {
        public bool Enabled { get; set; }
        public int Port { get; set; }
    }

    public class NightModeConfig
    {
        public bool Enabled { get; set; }
        public bool IrCutSingleInvert { get; set; }
        public bool ColorToGray { get; set; }
        public bool IrSensorPinInvert { get; set; }
    }

    public class MotionDetectConfig
    {
        public bool Enabled { get; set; }
        public bool Visualize { get; set; }
        public bool Debug { get; set; }
    }

    public class RecordsConfig
    {
        public bool Enabled { get; set; }
        public string Path { get; set; }
        public int MaxUsage { get; set; }
    }

    public class OutgoingConfig
    {
        public bool Enabled { get; set; }
    }

    public class WatchdogConfig
    {
        public bool Enabled { get; set; }
        public int Timeout { get; set; }
    }

    public class HlsConfig
    {
        public bool Enabled { get; set; }
    }

    public class OnvifConfig
    {
        public bool Enabled { get; set; }
    }

    public class IpeyeConfig
    {
        public bool Enabled { get; set; }
    }

    public class YoutubeConfig
    {
        public bool Enabled { get; set; }
    }

    public class NetipConfig
    {
        public bool Enabled { get; set; }
    }

    public class CloudConfig
    {
        public bool Enabled { get; set; }
    }


public static class DictionaryExtensions
{
    public static T ToObject<T>(this IDictionary<string, object> source) where T : new()
    {
        return (T)ToObject(typeof(T), source);
    }

    private static object ToObject(Type objectType, IDictionary<string, object> source)
    {
        if (source == null) return null;
        
        if (typeof(IDictionary<string, object>).IsAssignableFrom(objectType))
        {
            var dict = new Dictionary<string, object>();
            foreach (var item in source)
            {
                dict.Add(Char.ToUpper(item.Key[0]) + item.Key.Substring(1), ConvertValue(item.Value));
            }
            return dict;
        }
        else if (objectType.IsPrimitive || objectType == typeof(string))
        {
            return source;
        }

        var obj = Activator.CreateInstance(objectType);
        foreach (var item in source)
        {
            // Convert the first letter to uppercase if it's in camelCase to match PascalCase properties
            string pascalCaseKey = Char.ToUpper(item.Key[0]) + item.Key.Substring(1);
            var property = objectType.GetProperty(pascalCaseKey);
            if (property != null)
            {
                var value = ConvertValue(item.Value, property.PropertyType);
                property.SetValue(obj, value, null);
            }
        }
        return obj;
    }

    private static object ConvertValue(object value, Type targetType = null)
    {
        if (value is IDictionary<string, object> dictionaryValue)
        {
            if (targetType == null || targetType == typeof(object))
            {
                // If targetType is not provided or is object, convert to dictionary
                return dictionaryValue.ToObject<Dictionary<string, object>>();
            }
            else
            {
                // If targetType is provided, convert to the specified object type
                return ToObject(targetType, dictionaryValue);
            }
        }
        else
        {
            // For non-dictionary types, simply return the value as is
            // Additional type conversion logic can be added here if necessary
            return value;
        }
    }
}