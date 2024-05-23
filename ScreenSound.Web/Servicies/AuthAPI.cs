using ScreenSound.Web.Requests;
using ScreenSound.Web.Response;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;


namespace ScreenSound.Web.Servicies
{
    public class AuthAPI
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

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

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login?useCookies=true", new
            {
                email = username,
                password = password
            });
            if (response.IsSuccessStatusCode)
            {
                return new AuthResponse { Success = false, Error = new[] { "Autenticação bem-sucedida, mas nenhum cookie foi recebido." } };
            }
            else
            {
                return new AuthResponse { Success = false, Error = ["Login ou senha invalidos"] };
            }
        }
    }
}
