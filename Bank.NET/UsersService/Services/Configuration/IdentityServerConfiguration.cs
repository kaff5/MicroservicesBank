using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using System.Security.Claims;

namespace UsersService.Services.Configuration
{
    public class IdentityServerConfiguration
    {
        private static string portEmployeeClient = Environment.GetEnvironmentVariable("portEmployeeClient");
        private static string ipEmployeeClient = Environment.GetEnvironmentVariable("ipEmployeeClient");


        public static IEnumerable<ApiScope> ApiScope =>
            new List<ApiScope>
            {
                new ApiScope("api1", "Access to Api1")
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" }
                },
                new IdentityResource()
                {
                    Name = ClaimTypes.Role,
                    UserClaims = new List<string>
                    {
                        JwtClaimTypes.Role,
                    }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>()
            {
                new ApiResource
                {
                    Name = "api1",
                    DisplayName = "Api1",
                    Description = "Allow the application to access Api1",
                    Scopes = new List<string> { "api1" },
                    UserClaims = new List<string> { JwtClaimTypes.Role },
                    ApiSecrets = new List<Secret> { new Secret("anystringyoulike".Sha256()) },
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>()
            {
                new Client()
                {
                    ClientId = "EmployeeClient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = false,
                    RedirectUris =
                    {
                        $"http://{ipEmployeeClient}:{portEmployeeClient}/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"http://{ipEmployeeClient}:{portEmployeeClient}/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1", ClaimTypes.Role
                    },
                    AllowAccessTokensViaBrowser = true,
                },
                new Client
                {
                    ClientId = "client.android",
                    RequireClientSecret = true,
                    ClientSecrets = { new Secret("anystringyoulike".Sha256()) },
                    ClientName = "Android app client",
                    RequirePkce = false,
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris =
                        { "https://oauth.pstmn.io/v1/callback", "mobile://gevorkyanoffbank.ru/auth/success" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                }
            };
    }
}