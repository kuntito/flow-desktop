using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using flow_desktop.Models;

namespace flow_desktop.Services
{
    /// <summary>
    /// <para>a wrapper round the flow API service.</para>
    /// <para>ensures all API calls are safe —
    /// any errors they throw are caught and logged.
    /// they never reach the caller.</para>
    /// <para>the caller gets null.</para>
    /// </summary>
    public class FlowApiDataSource
    {
        private readonly FlowApiService _flowApi;

        public FlowApiDataSource()
        {
            _flowApi = new FlowApiService();
        }

        public async Task<GetNextSongResponse?> SafeFetchNextSong()
        {
            return await SafeApiCall(
                "getNextSong returns the next song from API",
                () => _flowApi.GetNextSong()
            );
        }

        public async Task<SearchSongResponse?> SafeSearchSong(string query)
        {
            return await SafeApiCall(
                "searchSong returns songs that match the given query",
                () => _flowApi.SearchSong(query)
            );
        }

        public async Task<GetSongByIdResponse?> SafeGetSongById(int songId)
        {
            return await SafeApiCall(
                "fetches song by id",
                () => _flowApi.GetSongById(songId)
            );
        }


        /// <summary>
        /// <para>wraps an API call in a try-catch block.</para>
        /// <para>if the call fails, it logs the error along with the
        /// description of what the call does.</para>
        /// <para>the caller gets the type's default value instead of an
        /// exception — null for classes, 0 for int, false for bool.</para>
        /// </summary>
        private async Task<T?> SafeApiCall<T>(
            string description,
            Func<Task<T?>> call
        )
        {
            try
            {
                return await call();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"API call failed: {description}");
                Debug.WriteLine($"Error: {e.Message}");
                return default;
            }
        }
    }
}
