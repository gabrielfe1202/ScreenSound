using ScreenSound.Web.Requests;
using ScreenSound.Web.Response;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace ScreenSound.Web.Servicies
{
    public class ArtistaAPI
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

        public ArtistaAPI(string baseAddress)
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

        public ArtistaAPI(IHttpClientFactory factory) 
        { 
                _httpClient = factory.CreateClient("API");
        }

        public async Task<ICollection<ArtistaResponse>?> GetArtistasAsync()
        {
            return await _httpClient.GetFromJsonAsync<ICollection<ArtistaResponse>>("artistas");
        }

        public async Task AddArtistaAsync(ArtistaRequest artista)
        {
            await _httpClient.PostAsJsonAsync("artistas", artista);
        }
        
        public async Task DeleteArtistaAsync(int id)
        {
            await _httpClient.DeleteAsync($"artistas/{id}");
        }
        public async Task UpdateArtistaAsync(ArtistaRequestEdit artista)
        {
            await _httpClient.PutAsJsonAsync($"artistas",artista);
        }
        public async Task<ArtistaResponse?> GetArtistaPorNomeAsync(string nome)
        {
            return await _httpClient.GetFromJsonAsync<ArtistaResponse>($"artistas/{nome}");
        }


        public async Task<List<GeneroResponse>?> GetGenerosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GeneroResponse>>("Generos");
        }

        public async Task<GeneroResponse?> GetGeneroPorNomeAsync(string nome)
        {
            return await _httpClient.GetFromJsonAsync<GeneroResponse>($"Generos/{nome}");
        }


        public async Task AddMusicaAsync(MusicaRequest musica)
        {
            await _httpClient.PostAsJsonAsync("Musicas", musica);
        }
        public async Task<ICollection<MusicaResponse>?> GetMusicasAsync()
        {
            return await _httpClient.GetFromJsonAsync<ICollection<MusicaResponse>>("musicas");
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
