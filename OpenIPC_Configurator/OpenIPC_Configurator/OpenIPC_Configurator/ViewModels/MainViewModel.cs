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
using Sini;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenIPC_Configurator.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Dictionary<string, object> MajesticConfigDynamic { get; set; }
    
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
    
    private MajesticConfig _majesticConfig = new MajesticConfig();
    public MajesticConfig MajesticConfig
    {
        get => _majesticConfig;
        set => this.RaiseAndSetIfChanged(ref _majesticConfig, value);
    }

    
    
    public WfbConfig WfbConfig { get; set; }

    
    private string _ipAddress = "192.168.0.1";

    public string IPAddress
    {
        get => _ipAddress;
        set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
    }

    public ICommand FetchCommand { get; }
    public ICommand SaveCommand { get; }

    private bool isLoaded = false;
    public bool IsLoaded
    {
        get => isLoaded;
        set => this.RaiseAndSetIfChanged(ref isLoaded, value);
    }
 
    
    public MainViewModel()
    {
        FetchCommand = ReactiveCommand.Create(() =>
        {
            _ = FetchClicked();
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
            var cts = new CancellationTokenSource();
            var scp = new ScpClient(new ConnectionInfo(IPAddress, USER, new PasswordAuthenticationMethod(USER, PASSWORD)));
            
            try
            {
                await scp.ConnectAsync(cts.Token);
            }
            catch (Exception ex)
            {
               return; 
            }
            
            try
            {
                var majesticStream = new MemoryStream();
            
                scp.Download(MAJESTIC_CONF_LOCATION, majesticStream);

                IsLoaded = true;
                scp.Dispose();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex) ;
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
            // ignored
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
