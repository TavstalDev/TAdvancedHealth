using Tavstal.TLibrary.Models.Config;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Database
{
    public class DatabaseData : DatabaseConfigBase
    {
        [YamlMember(Order = 7)] public string TablePrefix { get; set; } = "tahs_";
    }
}
