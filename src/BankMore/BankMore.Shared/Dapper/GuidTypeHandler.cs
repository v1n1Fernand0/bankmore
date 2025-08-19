using Dapper;
using System.Data;

namespace BankMore.Shared.Dapper
{
    public sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value) =>
            value switch
            {
                Guid g => g,
                string s when Guid.TryParse(s, out var g) => g,
                byte[] b => new Guid(b),
                ReadOnlyMemory<byte> rm => new Guid(rm.ToArray()),
                _ => throw new DataException($"Não foi possível converter '{value}' para Guid.")
            };

        public override void SetValue(IDbDataParameter parameter, Guid value) =>
            parameter.Value = value.ToString();
    }

    public sealed class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
    {
        public override Guid? Parse(object value)
        {
            if (value is null || value is DBNull) return null;
            return value switch
            {
                Guid g => g,
                string s when Guid.TryParse(s, out var g) => g,
                byte[] b => new Guid(b),
                ReadOnlyMemory<byte> rm => new Guid(rm.ToArray()),
                _ => throw new DataException($"Não foi possível converter '{value}' para Guid?.")
            };
        }

        public override void SetValue(IDbDataParameter parameter, Guid? value) =>
            parameter.Value = value?.ToString();
    }

    public static class DapperGuidHandlersBootstrap
    {
        private static bool _registered;

        public static void EnsureRegistered()
        {
            if (_registered) return;
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new NullableGuidTypeHandler());
            _registered = true;
        }
    }
}
