using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Crpg.Persistence.Converters;
public class EnumListJsonValueConverter<T> : ValueConverter<IList<T>, string> where T : Enum
{
    public EnumListJsonValueConverter()
        : base(
        v => JsonConvert
            .SerializeObject(v.Select(e => e.ToString()).ToList()),
        v => JsonConvert
            .DeserializeObject<IList<string>>(v)!
            .Select(e => (T)Enum.Parse(typeof(T), e)).ToList())
    {
    }
}
