using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using flow_desktop.Models;
using System.Net.Http.Headers;

namespace flow_desktop.Services
{

    class FlowApiService
    {
        private readonly HttpClient _client;


        public FlowApiService()
        {
            _client = new HttpClient
            {
                // TODO replace with stable base url
                BaseAddress = new Uri("https://efe3-102-88-54-74.ngrok-free.app")
            };
        }

        public async Task<GetNextSongResponse?> GetNextSong()
        {
            return await _client.GetFromJsonAsync<GetNextSongResponse>(
                "api/flow/next-song"
            );
        }

        public async Task<SearchSongResponse?> SearchSong(string query)
        {
            return await _client.GetFromJsonAsync<SearchSongResponse>(
                $"api/flow/search?q={query}"
            );
        }

        public async Task<GetSongByIdResponse?> GetSongById(int songId)
        {
            return await _client.GetFromJsonAsync<GetSongByIdResponse>(
                $"api/flow/song/{songId}"
            );
        }
    }
}
