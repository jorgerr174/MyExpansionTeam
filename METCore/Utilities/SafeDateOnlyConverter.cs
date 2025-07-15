using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace METCore.Utilities
{
    public class SafeDateOnlyConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim().ToUpper().Equals("NA"))
                return null;

            if (DateOnly.TryParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            throw new TypeConverterException(this, memberMapData, text, row.Context, $"Cannot convert '{text}' to DateOnly.");
        }
    }
}