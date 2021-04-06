using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using identity_connect.Models.Information;
using identity_connect.Http;
using identity_connect.Configurations;
using identity_connect.Expansions;
using identity_connect.Data.Enums;


namespace identity_connect.Info
{
    public class User
    {
        private readonly Config _config;
        private int UserId { get; set; }

        public User(Config config, int userId)
        {
            _config = config;
            UserId = userId;
        }

        public async Task<UserInfo> Get() =>
            await new Request().Get<UserInfo>(_config.GetUserInfoUrl.FixUrl() + UserId, _config.Token);

        public async Task<IList<TypeIntegrationEnum>> Types() =>
            await new Request().Get<IList<TypeIntegrationEnum>>(_config.GetUserIntegrationTypesUrl.FixUrl() + UserId, _config.Token);
    }
}
