﻿using ScreenSound.WebApp.Response;
using System.Net.Http.Json;

namespace ScreenSound.WebApp.Servicies
{
    public class GeneroAPI
    {
        private readonly HttpClient _httpClient;

        public GeneroAPI(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }

        public async Task<List<GeneroResponse>?> GetGenerosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GeneroResponse>>("Generos");
        }
        public async Task<GeneroResponse?> GetGeneroPorNomeAsync(string nome)
        {
            return await _httpClient.GetFromJsonAsync<GeneroResponse>($"Generos/{nome}");
        }
    }
}
