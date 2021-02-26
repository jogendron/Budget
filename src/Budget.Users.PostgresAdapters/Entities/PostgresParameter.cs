#pragma warning disable CS8632

using System.Data;
using Npgsql;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresParameter : IPostgresParameter
    {
        public PostgresParameter(NpgsqlParameter parameter)
        {
            Parameter = parameter;
        }

        public NpgsqlParameter Parameter { get; }

        public DbType DbType 
        { 
            get { return Parameter.DbType; } 
            set { Parameter.DbType = value; }
        }
        
        public ParameterDirection Direction 
        { 
            get { return Parameter.Direction; }
            set { Parameter.Direction = value; }
        }

        public string ParameterName 
        { 
            get { return Parameter.ParameterName; } 
            set { Parameter.ParameterName = value; }
        }

        public object Value 
        { 
            get { return Parameter.Value; } 
            set { Parameter.Value = value; }
        }
    }
}