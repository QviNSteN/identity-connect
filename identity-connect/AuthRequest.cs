using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using identity_connect.Models.Response;
using identity_connect.Models.Request;
using identity_connect.Http;
using identity_connect.Configurations;
using identity_connect.Expansions;
using identity_connect.Models.Auth;

namespace identity_connect.Authentication
{
    public class AuthRequest
    {
        private readonly Config _config;

        private Auth AuthModel { get; }

        public AuthRequest(TokenAuth token, Config config)
        {
            AuthModel = token;
            _config = config;
        }

        public AuthRequest(SessionAuth keys, Config config)
        {
            AuthModel = keys;
            _config = config;
        }

        internal async Task<AuthReturn> Auth()
        {
            var db = new DBContext(_config.ConnectionStrings.DataBase, GetTable());
            if (!await db.SessionExist(AuthModel))
            {
                var result = AuthModel switch
                {
                    TokenAuth => await new Request().Get<OkResponseToken>(GetUrl(), _config.Token),
                    SessionAuth => await new Request().Get<OkResponseLogin>(GetUrl(), _config.Token),
                    _ => new Response()
                };

                if (!result.IsAuthorized)
                    return AuthReturnError(result);

                if (result is OkResponseToken)
                    (result as OkResponseToken).Token = (AuthModel as TokenAuth).Token;

                await db.AddSession(result);
            }

            return AuthModel switch
            {
                TokenAuth => await GetServiceReturn(db),
                SessionAuth => await GetUserReturn(db),
                _ => AuthReturnError()
            };
        }

        private async Task<ServiceAuthReturn> GetServiceReturn(DBContext db)
        {
            var service = (AuthModel as TokenAuth);
            var result = await db.GetService(service.Token);
            return new ServiceAuthReturn()
            {
                IsAuthorized = true,
                ExternalId = result.ExternalId,
                Name = result.Name
            };
        }

        private async Task<UserAuthReturn> GetUserReturn(DBContext db)
        {
            var user = (AuthModel as SessionAuth);
            var result = await db.GetPermissions(user.AcceptKey);
            return new UserAuthReturn()
            {
                IsAuthorized = true,
                Permissions = result.Permissions,
                AcceptKey = user.AcceptKey,
                RefreshKey = user.RefreshKey,
                Info = new Models.Information.SimpleUserInfo() { UserId = result.UserId }
            };
        }

        private string GetUrl() =>
            AuthModel switch
            {
                TokenAuth => _config.AuthUrl.GenQuerty((nameof(TokenAuth.Token), (AuthModel as TokenAuth).Token)),
                SessionAuth => _config.AuthUrl.GenQuerty((nameof(SessionAuth.AcceptKey), (AuthModel as SessionAuth).AcceptKey), (nameof(SessionAuth.RefreshKey), (AuthModel as SessionAuth).RefreshKey)),
                _ => null
            };

        private string GetTable() =>
            AuthModel switch
            {
                TokenAuth => _config.ConnectionStrings.TableToken,
                SessionAuth => _config.ConnectionStrings.TableSession,
                _ => null
            };

        private AuthReturn AuthReturnError() =>
            new AuthReturn()
            {
                ErrorMessage = "Вариант авторизации неопознан",
                IsAuthorized = false
            };

        private AuthReturn AuthReturnError(Response response) =>
            new AuthReturn()
            {
                ErrorMessage = response.ErrorMessage,
                IsAuthorized = false
            };
    }
}
