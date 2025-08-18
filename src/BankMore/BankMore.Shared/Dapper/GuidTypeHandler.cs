using Dapper;
using System.Data;

namespace BankMore.Shared.Dapper
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            if (value is string str && Guid.TryParse(str, out var guid))
                return guid;

            if (value is byte[] bytes)
                return new Guid(bytes);

            return Guid.Empty;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }
    }
}
