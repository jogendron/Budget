#pragma warning disable CS8632

using System.Data;

namespace Budget.Users.PostgresAdapters.Entities
{
    public interface IPostgresParameter
    {
        DbType DbType { get; set; }

        ParameterDirection Direction { get; set; }

        string ParameterName { get; set; }

        object? Value { get; set; }
    }
}