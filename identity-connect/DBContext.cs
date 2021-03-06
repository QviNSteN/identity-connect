using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using identity_connect.Models.Request;
using identity_connect.Models.Entity;
using identity_connect.Models.Response;
using identity_connect.Models.Information;
using Npgsql;

namespace identity_connect
{
    public class DBContext
    {
        private Table[] Tables = new Table[]
        {
            new Table()
            {
                Name = "TokensSessions",
                Collumns = new Collumn[]
                {
                   new Collumn()
                   {
                       Name = "Token",
                       Type = "text NOT NULL"
                   },
                   new Collumn()
                   {
                       Name = "Name",
                       Type = "text"
                   }
                }
            },
            new Table()
            {
                Name = "Permissions",
                Collumns = new Collumn[]
                {
                   new Collumn()
                   {
                       Name = "AcceptKey",
                       Type = "text NOT NULL"
                   },
                   new Collumn()
                   {
                       Name = "Permission",
                       Type = "text"
                   }
                }
            },
            new Table()
            {
                Name = "UsersSessions",
                Collumns = new Collumn[]
                {
                   new Collumn()
                   {
                       Name = "AcceptKey",
                       Type = "text NOT NULL"
                   },
                   new Collumn()
                   {
                       Name = "RefreshKey",
                       Type = "text NOT NULL"
                   },
                   new Collumn()
                   {
                       Name = "DeathTime",
                       Type = "timestamp"
                   },
                   new Collumn()
                   {
                       Name = "UserId",
                       Type = "integer"
                   }
                }
            }
        };

        private string Table { get; set; }

        private string InfoTable => "Permissions";

        private NpgsqlConnection Connection { get; set; }

        private bool IsOpen { get; set; }

        private NpgsqlCommand Command(string querty)
        {
            var command = new NpgsqlCommand(querty, Connection);
            Open();
            return command;
        }

        private void Open()
        {
            if (IsOpen)
                return;
            IsOpen = true;
            Connection.Open();
        }

        public DBContext(string connectionString, string table)
        {
            Table = table;
            Connection = new NpgsqlConnection(connectionString);
            CreateTable(table);
            CreateTable(InfoTable);
        }

        private void CreateTable(string table)
        {
            Command($"CREATE TABLE if not exists \"{table}\" (\"Id\" integer generated by default as identity constraint \"PK_{table}\" primary key, {String.Join(", ", Tables.Single(x => x.Name == table).Collumns.Select(x => $"\"{x.Name}\" {x.Type}"))})").ExecuteNonQuery();
        }

        public async Task<bool> AddSession(Response auth) =>
            auth switch
            {
                OkResponseToken => await AddToken(auth as OkResponseToken),
                OkResponseLogin => await AddKeys(auth as OkResponseLogin)
            };

        public async Task<bool> SessionExist(Auth auth) =>
            auth switch
            {
                TokenAuth => await TokenExist(auth as TokenAuth),
                SessionAuth => await KeysExist(auth as SessionAuth)
            };

        public async Task<TokenSession> GetService(string token)
        {
            var reader = await Command($"SELECT \"ExternalId\", \"Name\" FROM \"{Table}\" WHERE \"Token\" = '{token}'").ExecuteReaderAsync();

            var result = new TokenSession() { Token = token };

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.ExternalId = reader.GetString(0);
                    result.Name = reader.GetString(1);
                }
            }

            return result;
        }

        public async Task<Models.Entity.UserInfo> GetPermissions(string acceptKey)
        {
            var reader = await Command($"SELECT \"Permission\", \"UserId\" FROM \"{Table}\" AS S LEFT JOIN \"{InfoTable}\" AS I ON I.\"AcceptKey\" = S.\"AcceptKey\" WHERE S.\"AcceptKey\" = '{acceptKey}'").ExecuteReaderAsync();

            var result = new Models.Entity.UserInfo();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Permissions.Add(reader.GetString(0));
                    result.UserId = (int)reader.GetInt64(1);
                }
            }

            return result;
        }

        private async Task<bool> AddToken(OkResponseToken token) =>
            await Command($"INSERT INTO \"{Table}\" {GetCollumns()} VALUES(\'{token.Token}\', \'{token.Name}\')").ExecuteNonQueryAsync() == 1 ? true : false;

        private async Task<bool> AddKeys(OkResponseLogin session)
        {
            var result = await Command($"INSERT INTO \"{Table}\" {GetCollumns()} VALUES(\'{session.AcceptKey}\', \'{session.RefreshKey}\', \'{session.DeathTime.ToString("yyyy-MM-dd")}\', \'{session.Info.UserId}\')").ExecuteNonQueryAsync() == 1 ? true : false;
            foreach(var p in session.Permissions)
                await Command($"INSERT INTO \"{InfoTable}\" {GetCollumns(InfoTable)} VALUES(\'{session.AcceptKey}\', \'{p}\')").ExecuteNonQueryAsync();
            return Close(result);
        }

        private async Task<bool> TokenExist(TokenAuth token) =>
            (long)await Command($"SELECT count(*) FROM \"{Table}\" WHERE \"Token\" = '{token.Token}'").ExecuteScalarAsync() > 0 ? Close(true) : Close(false);

        private async Task<bool> KeysExist(SessionAuth session) =>
            (long)await Command($"SELECT count(*) FROM \"{Table}\" WHERE \"AcceptKey\" = '{session.AcceptKey}' and \"RefreshKey\" = '{session.RefreshKey}' and \"DeathTime\" <= '{DateTime.Now.ToString("yyyy-MM-dd")}'").ExecuteScalarAsync() > 0 ? Close(true) : Close(false);

        ~DBContext()
        {
            Close();
        }

        private string GetCollumns() => GetCollumns(Table);

        private string GetCollumns(string tableName) =>
            $"({String.Join(", ", Tables.Single(x => x.Name == tableName).Collumns.Select(x => $"\"{x.Name}\""))})";

        private void Close() => Close(true);

        private bool Close(bool value)
        {
            Connection.Close();
            IsOpen = false;
            return value;
        }
    }
}
