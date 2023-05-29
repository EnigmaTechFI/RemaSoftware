using System.Reflection;

namespace RemaSoftware.Domain.Constants
{
    public static class WarehouseStockMeasure
    {
        public const string Metro = "Metro";
        public const string Scatola = "Scatola";
        public const string Litro = "Litro";
        public const string Kilogrammo = "Kilogrammo";

        public static List<string> GetUnitMeasure()
        {
            List<string> constants = new List<string>();
            
            FieldInfo[] fieldInfos = typeof(WarehouseStockMeasure).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var item in fieldInfos)
            {
                constants.Add(item.Name);
            }

            return constants;
        }
    }
}