using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using identity_connect.Models.Request;
using identity_connect.Configurations;
using identity_connect.Expansions;
using identity_connect.Models.Auth;
using identity_connect.SystemResourses;

namespace identity_connect.Authentication
{
    public class uMarkitAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Config _config;

        public uMarkitAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, Config config
            )
                : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Cookies.ContainsKey(Naming.COOKIES_ACCEPT_KEY) && Request.Cookies.ContainsKey(Naming.COOKIES_REFRESH_KEY))
                return await AuthenticateKeysAsync();
            else if (Request.Headers.ContainsKey(Naming.HEADERS_TOKEN) || Request.Cookies.ContainsKey(Naming.COOKIES_TOKEN))
                return await AuthenticateTokenAsync();

            return AuthenticateResult.Fail("Authentication scheme is not supported.");
        }

        protected async Task<AuthenticateResult> AuthenticateKeysAsync()
        {
            var authModel = new SessionAuth()
            {
                AcceptKey = Request.Cookies[Naming.COOKIES_ACCEPT_KEY],
                RefreshKey = Request.Cookies[Naming.COOKIES_REFRESH_KEY]
            };

            if (!authModel.AcceptKey.CorrectRegex(TokenMask)
                    || !authModel.RefreshKey.CorrectRegex(TokenMask)
                    || String.IsNullOrEmpty(authModel.AcceptKey))
                return AuthenticateResult.Fail("Authenticate key is don't valid!");

            var auth = await new AuthRequest(authModel, _config).Auth();

            if (auth.IsAuthorized)
            {
                var authInfo = auth as UserAuthReturn;
                if(authInfo.AcceptKey != Request.Cookies[Naming.COOKIES_ACCEPT_KEY])
                {
                    Response.Cookies.Append(Naming.COOKIES_ACCEPT_KEY, authInfo.AcceptKey);
                    Response.Cookies.Append(Naming.COOKIES_REFRESH_KEY, authInfo.RefreshKey);
                }

                var claims = new List<Claim>();
                claims.Add(new Claim(Naming.CLAIM_USER_ID, authInfo.Info.UserId.ToString()));
                foreach(var p in authInfo.Permissions)
                    claims.Add(new Claim(Naming.CLAIM_PERMISSION, p));
                return AuthenticateResult.Success(GetTicket(claims));
            }

            Response.Cookies.Delete(Naming.COOKIES_ACCEPT_KEY);
            Response.Cookies.Delete(Naming.COOKIES_REFRESH_KEY);

            return AuthenticateResult.Fail(auth.ErrorMessage);
        }

        protected async Task<AuthenticateResult> AuthenticateTokenAsync()
        {
            var authModel = new TokenAuth()
            {
                Token = Request.Headers.ContainsKey(Naming.HEADERS_TOKEN) ? Request.Headers[Naming.HEADERS_TOKEN] : Request.Cookies[Naming.COOKIES_TOKEN]
            };

            if (!authModel.Token.CorrectRegex(TokenMask)
                    || String.IsNullOrEmpty(authModel.Token))
                return AuthenticateResult.Fail("Authenticate key is don't valid!");

            var auth = await new AuthRequest(authModel, _config).Auth();

            if (auth.IsAuthorized)
            {
                var authInfo = auth as ServiceAuthReturn;

                var claims = new List<Claim>();
                claims.Add(new Claim(Naming.CLAIM_EXTERNAL_ID, authInfo.ExternalId));
                claims.Add(new Claim(Naming.CLAIM_SERVICE_NAME, authInfo.Name));
                return AuthenticateResult.Success(GetTicket(claims));
            }

            Response.Cookies.Delete(Naming.COOKIES_TOKEN);

            return AuthenticateResult.Fail(auth.ErrorMessage);
        }

        private string TokenMask => "^[a-zA-Z0-9.\\-_]*$";

        private AuthenticationTicket GetTicket(List<Claim> claims) =>
            new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name);

        private AuthenticationTicket GetTicket() => GetTicket(new List<Claim>());
    }
}
