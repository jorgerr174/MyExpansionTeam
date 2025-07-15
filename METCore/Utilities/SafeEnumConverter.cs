using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace METCore.Utilities
{
    public class SafeEnumConverter<T> : DefaultTypeConverter where T : struct
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (Enum.TryParse<T>(text, true, out var result))
                return result;
            return null;
        }
    }

}
