using SkullMp3Player.Scripts.Tools;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkullMp3Player.Scripts.Client.Controller
{
    static class HttpController
    {
        private static HttpClient _httpClient = new();

        public static async Task DownloadFile(string address, string fileName, string fileExtension)
        {
            Stream stream = await _httpClient.GetStreamAsync(address);
            FileStream fileStrean = new(Mp3PlayerFolder.GetPlayerFolder() + "\\" + fileName + fileExtension, FileMode.CreateNew);
            await stream.CopyToAsync(fileStrean);
            fileStrean.Close();
        }

        public static async Task<bool> HasConnection(string address)
        {
            try {
                HttpRequestMessage request = new(HttpMethod.Get, address);
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    return true;
                }
            } catch (Exception) {
                return false;
            }

            return false;
        }

        public static async Task<string> SendGetRequest(string address)
        {
            try {
                return await _httpClient.GetStringAsync(address);
            } catch (Exception) {
                return string.Empty;
            }
        }
    }
}
