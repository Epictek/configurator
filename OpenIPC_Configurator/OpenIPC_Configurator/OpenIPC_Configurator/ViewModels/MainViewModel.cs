using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenIPC_Configurator.Models;
using ReactiveUI;
using Renci.SshNet;
using Renci.SshNet.Common;
using Sini;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenIPC_Configurator.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Dictionary<string, object> MajesticConfigDynamic { get; set; }
   
    public Dictionary<string, int> Frequencies { get; } = new Dictionary<string, int>()
    {
        {"5180 MHz", 36},
        {"5200 MHz", 40},
        {"5220 MHz", 44},
        {"5240 MHz", 48},
        {"5260 MHz", 52},
        {"5280 MHz", 56},
        {"5300 MHz", 60},
        {"5320 MHz", 64},
        {"5500 MHz", 100},
        {"5520 MHz", 104},
        {"5540 MHz", 108},
        {"5560 MHz", 112},
        {"5580 MHz", 116},
        {"5600 MHz", 120},
        {"5620 MHz", 124},
        {"5640 MHz", 128},
        {"5660 MHz", 132},
        {"5680 MHz", 136},
        {"5700 MHz", 140},
        {"5720 MHz", 144},
        {"5745 MHz", 149},
        {"5765 MHz", 153},
        {"5785 MHz", 157},
        {"5805 MHz", 161},
        {"5825 MHz", 165},
        {"5845 MHz", 169},
        {"5865 MHz", 173},
        {"5885 MHz", 177}
    };
    
    public int[] BitRates { get; } =
    [
        1024,
        2048,
        3072,
        4096,
        5120,
        6144,
        7168,
        8192,
        9216,
        10240,
        11264,
        12288,
        13312
    ];
    
    public string[] Resolutions { get; } =
    [
        "1280x720",
        "1920x1080",
        "3200x1800",
        "3840x2160"
    ];

    public string[] Codecs { get; } = ["h264", "h265"];

    private MajesticConfig _majesticConfig;
    public MajesticConfig MajesticConfig
    {
        get => _majesticConfig;
        set => this.RaiseAndSetIfChanged(ref _majesticConfig, value);
    }

    private WfbConfig _wfbConfig = new WfbConfig();

    public WfbConfig WfbConfig
    {
        get => _wfbConfig;
        set => this.RaiseAndSetIfChanged(ref _wfbConfig, value);
    }

    
    private string _ipAddress = "192.168.0.1";

    public string IPAddress
    {
        get => _ipAddress;
        set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
    }

    public ICommand FetchCommand { get; }
    public ICommand SaveCommand { get; }
    

    private bool _alertIsVisible;
    public bool AlertIsVisible
    {
        get => _alertIsVisible;
        set => this.RaiseAndSetIfChanged(ref _alertIsVisible, value);
    }
    
    private string _alertText = "";
    public string AlertText
    {
        get => _alertText;
        set => this.RaiseAndSetIfChanged(ref _alertText, value);
    }

    private bool isLoaded = false;

    public bool IsLoaded
    {
        get => isLoaded;
        set => this.RaiseAndSetIfChanged(ref isLoaded, value);
    }
 
    
    public MainViewModel()
    {
        FetchCommand = ReactiveCommand.Create(async () =>
        {
            await FetchClicked();
        });
        SaveCommand = ReactiveCommand.Create(() =>
        {
            _ = SaveClicked();
        });

    }
    
    //const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";

    private const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";
    const string USER = "root";
    const string PASSWORD = "12345";

    async Task FetchClicked()
    {
        App.Logger.Information("Fetch clicked");
        AlertIsVisible = false;

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var scp = new ScpClient(
                new ConnectionInfo(IPAddress, USER, new PasswordAuthenticationMethod(USER, PASSWORD))
                    { Timeout = TimeSpan.FromSeconds(5), RetryAttempts = 1 });

            try
            {
                App.Logger.Information("Connecting");
                 await scp.ConnectAsync(cts.Token);
                App.Logger.Information("Connected");

            }
            catch (Exception ex)
            {
                App.Logger.Error(ex, "error connecting to ssh");

                AlertIsVisible = true;
                AlertText = "ssh connection failed: " + ex.Message;
                return;
 
            }
            
            try
            {
                var majesticStream = new MemoryStream();
            
                scp.Download(MAJESTIC_CONF_LOCATION, majesticStream);
                LoadMajesticYaml(majesticStream);
                IsLoaded = true;
                scp.Dispose();

            }
            catch (Exception ex)
            {
                App.Logger.Error(ex, "error downloading majestic config");
                AlertIsVisible = true;
                AlertText = ex.Message;
            }
    }


    async Task SaveClicked()
    {
        var cts = new CancellationTokenSource();
        var scp = new ScpClient(new ConnectionInfo(IPAddress, USER, new PasswordAuthenticationMethod(USER, PASSWORD)));
            
        try
        {
            await scp.ConnectAsync(cts.Token);
        }
        catch (Exception ex)
        {
            AlertIsVisible = true;
            AlertText = ex.Message;
            return;
        }

        try
        {
            //backup current majestic.yaml
            var path = Path.Join(Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "OpenIPC_Configurator");
            Directory.CreateDirectory(path);
            var file =  File.OpenWrite(Path.Join(path, $"majestic-{DateTime.Now:u}.yaml"));
            scp.Download(MAJESTIC_CONF_LOCATION, file);

            
            var yaml = MajesticYamlToString();
            byte[] byteArray = Encoding.UTF8.GetBytes(yaml);
            await using MemoryStream stream = new MemoryStream(byteArray);
            scp.Upload(stream, MAJESTIC_CONF_LOCATION);
            scp.Dispose();
        }
        catch (Exception ex)
        {
            AlertIsVisible = true;
            AlertText = ex.Message;
        }
    }
    
    void LoadMajesticYaml(Stream stream)
    {
        stream.Position = 0;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        using var reader = new StreamReader(stream);
        MajesticConfig = deserializer.Deserialize<MajesticConfig>(reader);

    }

    string MajesticYamlToString(){
	
        // Serialize object to YAML and save
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        
        return serializer.Serialize(MajesticConfig);

    }

    void LoadWfbConf()
    {
       WfbConfig  = IniFile.ToObject<WfbConfig>("wfb.conf", IniFile.ParseObjectFlags.AllowAdditionalKeys | IniFile.ParseObjectFlags.AllowMissingFields | IniFile.ParseObjectFlags.LowercaseKeysAndSections);
  
    }


    void SaveWfbConf(){

       var file = IniFile.FromObject(WfbConfig,
           IniFile.ParseObjectFlags.AllowAdditionalKeys | IniFile.ParseObjectFlags.AllowAdditionalKeys | IniFile.ParseObjectFlags.LowercaseKeysAndSections);
       file.SaveTo("wfb.conf");
     
	//IniFile.FromObject()
    }
}
