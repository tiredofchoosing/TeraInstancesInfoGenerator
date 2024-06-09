using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Linq;

namespace TeraInstancesInfoGenerator
{
    public class Program
    {
        static readonly string dungeonsXmlFile = "DungeonMatching.xml";
        static readonly string battlegroundsXmlFile = "BattleFieldData.xml";
        static readonly string dungeonsStrFile = "StrSheet_Dungeon-00000.xml";
        static readonly string battlegroundsStrFile = "StrSheet_BattleField-00000.xml";
        static readonly string dungeonsJsonFile = "DungeonsInfo.json";
        static readonly string battlegroundsJsonFile = "BattlegroundsInfo.json";

        static void Main(string[] args)
        {
            try
            {
                var dgInfoPath = GetPath(dungeonsXmlFile);
                var dgStrPath = GetPath(dungeonsStrFile);
                var bgInfoPath = GetPath(battlegroundsXmlFile);
                var bgStrPath = GetPath(battlegroundsStrFile);

                Process(dgInfoPath, dgStrPath);
                Process(bgInfoPath, bgStrPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void Process(string infoPath, string strPath)
        {
            if (!(File.Exists(infoPath) &&
                File.Exists(strPath)))
                throw new Exception("Not all required files were found. Check ReadMe.");

            var infoXml = XDocument.Load(infoPath);
            var strXml = XDocument.Load(strPath);

            if (infoXml == null || infoXml.Root == null)
                throw new Exception($"Could not parse xml file {infoPath}");

            if (strXml == null || strXml.Root == null)
                throw new Exception($"Could not parse xml file {strPath}");

            var isDungeons = Path.GetFileName(infoPath).Equals(dungeonsXmlFile);
            var elemName = isDungeons ? "Dungeon" : "BattleField";
            var filename = isDungeons ? dungeonsJsonFile : battlegroundsJsonFile;
            Instance.Parser parser = isDungeons ? Dungeon.TryParse : Battleground.TryParse;

            Dictionary<int, Instance> instances = new();

            var ns = infoXml.Root.Name.Namespace;
            IEnumerable<XElement> elems = infoXml.Root.Elements(ns + elemName);
            IEnumerable<XElement> support = infoXml.Root.Elements(ns + "SupportMatching");
            foreach (XElement elem in elems.Concat(support))
            {
                var success = parser(elem, out var inst, (string id) => GetNameById(strXml, id));
                if (success)
                    instances.Add(inst.Id, inst);
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(instances, serializerOptions);
            var path = GetPath(filename);
            File.WriteAllText(path, json);
            Console.WriteLine($"{filename} was generated.");
        }

        static string GetPath(string filename)
        {
            return Path.Combine(Environment.CurrentDirectory, filename);
        }

        static string GetNameById(XDocument strXml, string id)
        {
            var elem = strXml.Root?.Elements().SingleOrDefault(e => e.Attribute("id")?.Value == id);
            return elem?.Attribute("string")?.Value ?? "";
        }
    }
}