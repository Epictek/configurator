using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenIPC_Configurator.Helpers;
using OpenIPC_Configurator.Models;
using ReactiveUI;
using Renci.SshNet;
using Sini;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenIPC_Configurator.ViewModels;

public class MainViewModel : ViewModelBase
{
    
    
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

    
    private Dictionary<string, string> _majesticConfigDynamic = new Dictionary<string, string>();
    public Dictionary<string, string> MajesticConfigDynamic
    {
        get => _majesticConfigDynamic;
        set => this.RaiseAndSetIfChanged(ref _majesticConfigDynamic, value);
    }
    
    
    
    public WfbConfig WfbConfig { get; set; }

    public string IPAddress { get; set; } = "127.0.0.1";
   
    
    public ICommand FetchCommand { get; }
    public ICommand SaveCommand { get; }

    public MainViewModel()
    {
        FetchCommand = ReactiveCommand.Create(() =>
        {
            _ = FetchClicked();
        });
        SaveCommand = ReactiveCommand.Create(() =>
        {
            SaveClicked();
        });

    }
    
    //const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";

    private const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";
    const string USER = "root";
    const string PASSWORD = "12345";

    // async Task ConnectClicked()
    // {
    //     var cts = new CancellationTokenSource();
    //     var scp = new ScpClient(new ConnectionInfo(IPAddress, USER, new PasswordAuthenticationMethod(USER, PASSWORD)));
    //
    //     try
    //     {
    //         await scp.ConnectAsync(cts.Token);
    //     }
    //     catch (Exception ex)
    //     {
    //        return; 
    //     }
    //
    //     try
    //     {
    //         var majesticStream = new MemoryStream();
    //
    //         scp.Download(MAJESTIC_CONF_LOCATION, majesticStream);
    //
    //         LoadMajesticYaml(majesticStream);
    //
    //         var yaml = MajesticYamlToString();
    //
    //         byte[] byteArray = Encoding.UTF8.GetBytes(yaml);
    //         await using MemoryStream stream = new MemoryStream(byteArray);
    //         scp.Upload(stream, MAJESTIC_CONF_LOCATION);
    //         
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.Error.WriteLine(ex) ;
    //     }
    // }
    
    
    async Task FetchClicked()
    { try
        {

            var majesticStream = File.OpenRead("majestic.yaml");
            MajesticConfigDynamic = YamlReader.ReadYamlToDictionary(majesticStream);
            // LoadMajesticYaml(majesticStream);

        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex) ;
        }
    }


    void SaveClicked()
    {
        var yaml = MajesticYamlToString();

        Console.WriteLine(yaml);            

    }
    
    void LoadMajesticYaml(Stream stream)
    {
        // stream.Position = 0;
        // var deserializer = new DeserializerBuilder()
        //     .WithNamingConvention(CamelCaseNamingConvention.Instance)
        //     .IgnoreUnmatchedProperties()
        //     .Build();
        //
        // // Deserialize YAML to object
        // using var reader = new StreamReader(stream);
        // MajesticConfig = deserializer.Deserialize<MajesticConfig>(reader);

    }

    string MajesticYamlToString(){
	
        // Serialize object to YAML and save
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        MajesticConfigDynamic["derp"] = "derp";
        
        return serializer.Serialize(MajesticConfigDynamic);

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
