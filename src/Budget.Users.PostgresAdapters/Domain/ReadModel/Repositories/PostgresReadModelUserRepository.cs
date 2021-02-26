using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.Users.Domain.ReadModel;
using Budget.Users.Domain.ReadModel.Repositories;
using Budget.Users.PostgresAdapters.Entities;

namespace Budget.Users.PostgresAdapters.Domain.ReadModel.Repositories
{
    public class PostgresReadModelUserRepository : IReadModelUserRepository
    {
        public PostgresReadModelUserRepository(IPostgresConnection connection)
        {
            Connection = connection;
        }

        private IPostgresConnection Connection { get; }

        private IPostgresTransaction Transaction { get; set; }

        private void AddParameter(IPostgresCommand command, DbType dbType, ParameterDirection direction, string name, object value)
        {
            IPostgresParameter param = command.CreateParameter();
            param.DbType = dbType;
            param.Direction = direction;
            param.ParameterName = name;
            param.Value = value;

            command.Parameters.Add(param);
        }

        private void AddSetClause(StringBuilder builder, IPostgresCommand command, DbType dbType, ParameterDirection direction, string columnName, object value)
        {
            var parameterName = $"p_{columnName}";

            if (! builder.ToString().Contains("SET"))
                builder.Append("SET ");          

            if (command.Parameters.Any())
                builder.Append(", ");
            
            builder.Append($"{columnName} = :{parameterName}");
            AddParameter(command, DbType.String, ParameterDirection.Input, parameterName, value);
        }

        internal void SetTransaction(IPostgresTransaction transaction)
        {
            Transaction = transaction;
        }

        public async Task<bool> Exists(string userName)
        {
            bool exists = false;

            using (IPostgresCommand query = Connection.CreateCommand())
            {
               query.CommandText = "SELECT count(0) AS count from users.users WHERE username = :p_username";
                
                AddParameter(query, DbType.String, ParameterDirection.Input, "p_username", userName);

                if (Transaction != null)
                    query.Transaction = Transaction;

                await query.PrepareAsync();

                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (reader.Read())
                        exists = reader.GetInt32(reader.GetOrdinal("count")) > 0;
                }
            }
            
            return exists;
        }

        public async Task<User> GetUser(string userName)
        {
            User user = null;

            using (IPostgresCommand query = Connection.CreateCommand())
            {
                query.CommandText = "SELECT id, username, firstname, lastname, email FROM users.users WHERE username = :p_username";

                AddParameter(query, DbType.String, ParameterDirection.Input, "p_username", userName);

                if (Transaction != null)
                    query.Transaction = Transaction;

                await query.PrepareAsync();

                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var id = reader.GetGuid(reader.GetOrdinal("id"));
                        var username = reader.GetString(reader.GetOrdinal("username"));
                        var firstname = reader.GetString(reader.GetOrdinal("firstname"));
                        var lastname = reader.GetString(reader.GetOrdinal("lastname"));
                        var email = reader.GetString(reader.GetOrdinal("email"));

                        user = new User(id, username, firstname, lastname, email);
                    }
                }
            }
            
            return user;
        }

        public async Task Save(User user)
        {
            var existingUser = await GetUser(user.UserName);

            if (existingUser != null)
            {
                if (! user.Equals(existingUser))
                    await Update(user, existingUser);
            }
            else 
            {
                await Insert(user);
            }
        }

        private async Task Insert(User user)
        {
            using (IPostgresCommand command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO users.users (
                        id, 
                        username, 
                        firstname, 
                        lastname, 
                        email
                    ) values (
                        :p_id, 
                        :p_username, 
                        :p_firstname, 
                        :p_lastname, 
                        :p_email
                    )
                ";

                AddParameter(command, DbType.Guid, ParameterDirection.Input, "p_id", user.Id);
                AddParameter(command, DbType.String, ParameterDirection.Input, "p_username", user.UserName);
                AddParameter(command, DbType.String, ParameterDirection.Input, "p_firstname", user.FirstName);
                AddParameter(command, DbType.String, ParameterDirection.Input, "p_lastname", user.LastName);
                AddParameter(command, DbType.String, ParameterDirection.Input, "p_email", user.Email);

                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task Update(User modifiedUser, User existingUser)
        {
            using (IPostgresCommand command = Connection.CreateCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("UPDATE users.users ");

                if (modifiedUser.UserName != existingUser.UserName)
                    AddSetClause(builder, command, DbType.String, ParameterDirection.Input, "username", modifiedUser.UserName);

                if (modifiedUser.FirstName != existingUser.FirstName)
                    AddSetClause(builder, command, DbType.String, ParameterDirection.Input, "firstname", modifiedUser.FirstName);

                if (modifiedUser.LastName != existingUser.LastName)
                    AddSetClause(builder, command, DbType.String, ParameterDirection.Input, "lastname", modifiedUser.LastName);

                if (modifiedUser.Email != existingUser.Email)
                    AddSetClause(builder, command, DbType.String, ParameterDirection.Input, "email", modifiedUser.Email);

                builder.Append(" WHERE id = :p_id");
                AddParameter(command, DbType.Guid, ParameterDirection.Input, "p_id", modifiedUser.Id);

                command.CommandText = builder.ToString();
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}