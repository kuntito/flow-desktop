using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using flow_desktop.Services;

namespace flow_desktop.Helpers
{
    public static class FlowApiTester
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented = true,

            // without `Encoder`, the serializer escapes special characters
            // like '&' with their unicode versions.
            // i.e. '&' becomes '\u0026'

            // this prevents logged URLs from being copy-pasted into a browser.
            // adding,
            // `System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping`,
            // allows you see the raw JSON.
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static readonly FlowApiDataSource FlowDs = new();

        public static async Task LogNextSong()
        {
            var res = await FlowDs.SafeFetchNextSong();
            var resJson = JsonSerializer
                .Serialize(
                    res,
                    JsonOpts
                );

            Debug.WriteLine($"SafeFetchNextSong: {resJson}");
        }
    }
}
