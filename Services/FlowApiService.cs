using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using flow_desktop.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace flow_desktop.Services
{

    class FlowApiService
    {
        private readonly HttpClient _client;


        public FlowApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://flow-api-o6gg.onrender.com")
            };
        }

        public async Task<GetNextSongResponse?> GetNextSong()
        {
            return await _client.GetFromJsonAsync<GetNextSongResponse>(
                "api/flow/next-song"
            );
        }
        
        public async Task<SearchSongResponse?> SearchSong(
            string query,
            // cancelToken, is used client side to cancel a request on demand.
            CancellationToken cancelToken
        )
        {
            var url = $"api/flow/search?q={query}";
            var response = await _client.GetFromJsonAsync<SearchSongResponse>(
                url,
                cancelToken
            );
            
            return response;
        }

        public async Task<GetSongByIdResponse?> GetSongById(int songId)
        {
            return await _client.GetFromJsonAsync<GetSongByIdResponse>(
                $"api/flow/song/{songId}"
            );
        }
    }
}
