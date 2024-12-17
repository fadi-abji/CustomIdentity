using Blazor.Web.Models;
using Blazor.Web.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Blazor.Web.Components.Account
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        //public ILocalStorageService _localStorageService { get; }
        public ISessionStorageService _sessionStorageService { get; }
        public IUserService _userService { get; set; }
        private readonly HttpClient _httpClient;

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorageService, IUserService userService, HttpClient httpClient)
        {
            _sessionStorageService = sessionStorageService;
            _userService = userService;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            var accessToken = await _sessionStorageService.GetItemAsync<string>("accessToken");

            ClaimsIdentity identity;

            if (accessToken != null && accessToken != string.Empty)
            {
                User user = await _userService.GetUserByAccessTokenAsync(accessToken);
                identity = GetClaimsIdentity(user);
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsAuthenticated(User user)
        {
            await _sessionStorageService.SetItemAsync("accessToken", user.AccessToken);
            await _sessionStorageService.SetItemAsync("refreshToken", user.RefreshToken);

            var identity = GetClaimsIdentity(user);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorageService.RemoveItemAsync("refreshToken");
            await _sessionStorageService.RemoveItemAsync("accessToken");

            var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (user.EmailAddress != null)
            {
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, user.EmailAddress),
                                    new Claim(ClaimTypes.Role, user.Role.RoleDesc),
                                    new Claim("IsUserEmployedBefore1990", IsUserEmployedBefore1990(user))
                                }, "apiauth_type");
            }

            return claimsIdentity;
        }

        private string IsUserEmployedBefore1990(User user)
        {
            if (user.HireDate.Value.Year < 1990)
                return "true";
            else
                return "false";
        }
    }
}
