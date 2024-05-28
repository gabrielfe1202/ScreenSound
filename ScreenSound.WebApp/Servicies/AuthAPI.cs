using Microsoft.AspNetCore.Components.Authorization;
using ScreenSound.WebApp.Requests;
using ScreenSound.WebApp.Response;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;


namespace ScreenSound.WebApp.Servicies
{
    public class AuthAPI : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private bool autenticado = false;

        public AuthAPI(string baseAddress)
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                UseCookies = true, // Habilita o uso de cookies
                CookieContainer = new CookieContainer() // Cria um contêiner de cookies
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        public AuthAPI(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            autenticado = false;
            var pessoa = new ClaimsPrincipal();
            var response = await _httpClient.GetAsync("auth/manage/info");
            if (response.IsSuccessStatusCode)
            {
                var info = await response.Content.ReadFromJsonAsync<InfoPessoaResponse>();
                Claim[] dados = [
                    new Claim(ClaimTypes.Name, info.email),
                    new Claim(ClaimTypes.Email, info.email)
                ];
                var identity = new ClaimsIdentity(dados, "Cookies");
                pessoa = new ClaimsPrincipal(identity);
                autenticado = true;
            }
            return new AuthenticationState(pessoa);
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login?useCookies=true", new
            {
                email = username,
                password
            });
            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthResponse { Success = false, Error = new[] { "Autenticação bem-sucedida" } };
            }
            else
            {
                return new AuthResponse { Success = false, Error = ["Login ou senha invalidos"] };
            }
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("auth/Logout", null);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> VerifyAuthentication()
        {
            await GetAuthenticationStateAsync();
            return autenticado;
        }
    }
}
