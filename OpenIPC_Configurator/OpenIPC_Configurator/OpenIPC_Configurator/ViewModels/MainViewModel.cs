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
    public Dictionary<object, object> MajesticConfigDynamic { get; set; }
    
    public MajesticConfig MajesticConfig { get; set; }
    public WfbConfig WfbConfig { get; set; }

    public string IPAddress { get; set; } = "127.0.0.1";
   
    
    public ICommand ConnectCommand { get; }
    
    public MainViewModel()
    {
        ConnectCommand = ReactiveCommand.Create(() =>
        {
            _ = ConnectClicked();
        });
        
    }
    
    //const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";

    private const string MAJESTIC_CONF_LOCATION = "/etc/majestic.yaml";
    const string USER = "root";
    const string PASSWORD = "12345";

    async Task ConnectClicked()
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

            LoadMajesticYaml(majesticStream);

            var yaml = MajesticYamlToString();

            byte[] byteArray = Encoding.UTF8.GetBytes(yaml);
            await using MemoryStream stream = new MemoryStream(byteArray);
            scp.Upload(stream, MAJESTIC_CONF_LOCATION);
            
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex) ;
        }
    }

    void LoadMajesticYaml(Stream stream)
    {
        stream.Position = 0;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Deserialize YAML to object
        using var reader = new StreamReader(stream);
        MajesticConfigDynamic = deserializer.Deserialize<Dictionary<object, object>>(reader);
        if (MajesticConfigDynamic == null)
        {
            MajesticConfigDynamic = new Dictionary<object, object>();
            MajesticConfig = new MajesticConfig();
            return;
        }
        MajesticConfig = new MajesticConfig()
        {
            Image = (Image)(!MajesticConfigDynamic.ContainsKey(nameof(MajesticConfig.Image))
                ? new Image()
                : MajesticConfigDynamic["image"])
        };
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
