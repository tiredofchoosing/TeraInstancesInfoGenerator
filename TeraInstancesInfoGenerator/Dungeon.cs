using System.Xml.Linq;

namespace TeraInstancesInfoGenerator
{
    class Dungeon : Instance
    {
        private static readonly Dungeon _empty = new(0, 0, 0, 0, "");

        public int MinItemLevel { get; init; }

        public Dungeon(int id, int min, int max, int ilvl, string name)
        {
            Id = id;
            MinLevel = min;
            MaxLevel = max;
            MinItemLevel = ilvl;
            Name = name;
        }

        public static Dungeon Empty => _empty;

        public static bool TryParse(XElement elem, out Instance dungeon, Func<string, string> namer)
        {
            dungeon = Empty;
            var attrs = elem.Attributes();
            if (attrs == null || !attrs.Any())
                return false;

            var idStr = attrs.SingleOrDefault(a => a.Name == "id")?.Value;
            var minStr = attrs.SingleOrDefault(a => a.Name == "dungeonMinLevel")?.Value;
            var maxStr = attrs.SingleOrDefault(a => a.Name == "dungeonMaxLevel")?.Value;
            var ilvlStr = attrs.SingleOrDefault(a => a.Name == "minItemLevel")?.Value;

            if (string.IsNullOrWhiteSpace(idStr) ||
                string.IsNullOrWhiteSpace(minStr) ||
                string.IsNullOrWhiteSpace(maxStr) ||
                string.IsNullOrWhiteSpace(ilvlStr))
                return false;

            var success = int.TryParse(idStr, out int id);
            if (!success)
                return false;

            int.TryParse(minStr, out int min);
            int.TryParse(maxStr, out int max);
            int.TryParse(ilvlStr, out int ilvl);

            dungeon = new Dungeon(id, min, max, ilvl, namer(id.ToString()));
            return true;
        }
    }
}
