using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace flow_desktop.Helpers;

public class FileDownloader
{
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// downloads a file from the given url and saves it to the destination path.
    /// returns the file if the download was successful, null otherwise.
    /// </summary>
    public static async Task<FileInfo?> DownloadFile(
        string url,
        string destPath
    )
    {
        var partPath = destPath + ".part";

        try
        {
            using var response = await _httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead
            );
            response.EnsureSuccessStatusCode();

            await using (var sourceStream = await response.Content.ReadAsStreamAsync())
            await using (var partFile = File.Create(partPath))
            {
                await sourceStream.CopyToAsync(partFile);
            }

            File.Move(
                partPath,
                destPath,
                overwrite: true
            );

            var downloadedFile = new FileInfo(destPath);

            return downloadedFile;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"downloadFile failed: {url}, {e.Message}");

            if (File.Exists(partPath))
            {
                File.Delete(partPath);
            }

            return null;
        }
    }
}