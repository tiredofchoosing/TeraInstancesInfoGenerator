using System.Xml.Linq;

namespace TeraInstancesInfoGenerator
{
    class Battleground : Instance
    {
        private static readonly Battleground _empty = new(0, 0, 0, "");

        public Battleground(int id, int min, int max, string name)
        {
            Id = id;
            MinLevel = min;
            MaxLevel = max;
            Name = name;
        }

        public static Battleground Empty => _empty;

        public static bool TryParse(XElement elem, out Instance battleground, Func<string, string> namer)
        {
            battleground = Empty;
            var attrs = elem.Attributes();
            if (attrs == null || !attrs.Any())
                return false;

            var ns = elem.Name.Namespace;
            var dataAttrs = elem.Element(ns + "CommonData")?.Attributes();
            if (dataAttrs == null || !dataAttrs.Any())
                return false;

            var idStr = attrs.SingleOrDefault(a => a.Name == "id")?.Value;
            var minStr = dataAttrs.SingleOrDefault(a => a.Name == "minLevel")?.Value;
            var maxStr = dataAttrs.SingleOrDefault(a => a.Name == "maxLevel")?.Value;

            if (string.IsNullOrWhiteSpace(idStr) ||
                string.IsNullOrWhiteSpace(minStr) ||
                string.IsNullOrWhiteSpace(maxStr))
                return false;

            var success = int.TryParse(idStr, out int id);
            if (!success)
                return false;

            int.TryParse(minStr, out int min);
            int.TryParse(maxStr, out int max);

            var nameId = attrs.SingleOrDefault(a => a.Name == "name")?.Value ?? "";

            battleground = new Battleground(id, min, max, namer(nameId));
            return true;
        }
    }
}
