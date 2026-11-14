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
        
        
        public class SyncPlaylistsBody
        {
            public List<PlaylistItem> Playlists { get; set; } = new();
        }
        
        /// <summary>
        /// syncs playlists.
        ///
        /// playlists are modified on the mobile client,
        /// and mobile routinely sends updated copies of them.
        /// 
        /// the API takes these updated copies as truth,
        /// and replaces its own version with said copies.
        ///
        /// however, call this endpoint without any playlists,
        /// and it returns all the playlists the API has.
        ///
        /// so it doubles as "tell me what you have",
        /// or "take these and replace what you have".
        /// </summary>
        public async Task<SyncPlaylistsResponse?> SyncPlaylists(
            List<PlaylistItem> playlists
        )
        {
            var body = new SyncPlaylistsBody
            {
                Playlists = playlists,
            };

            var response = await _client.PostAsJsonAsync(
                "api/flow/playlists/sync",
                body
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SyncPlaylistsResponse>();
        }
        
        public async Task<GetPlaylistNextSongResponse?> GetPlaylistNextSong(int playlistId)
        {
            var response = await _client.PostAsync(
                $"api/flow/playlists/{playlistId}/next-song",
                null
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GetPlaylistNextSongResponse>();
        }
    }
}
    