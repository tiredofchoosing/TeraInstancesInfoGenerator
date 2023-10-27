using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TeraInstancesInfoGenerator
{
    [JsonDerivedType(typeof(Dungeon))]
    [JsonDerivedType(typeof(Battleground))]
    abstract class Instance
    {
        public delegate bool Parser(XElement elem, out Instance inst, Func<string, string> namer);

        [JsonIgnore]
        public int Id { get; init; }
        public int MinLevel { get; init; }
        public int MaxLevel { get; init; }
        public string Name { get; init; }
    }
}