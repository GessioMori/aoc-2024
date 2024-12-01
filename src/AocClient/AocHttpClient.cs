using aoc_2024.Interfaces;
using System.Net;

namespace aoc_2024.AocClient
{
    public class AocHttpClient : IAocClient, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string? sessionCookie;
        private readonly Dictionary<int, ClientResponse> inputCache;
        private readonly Dictionary<int, DateTime> requestTimestamps;
        private readonly TimeSpan throttleDuration;

        public AocHttpClient()
        {
            this.inputCache = [];
            this.requestTimestamps = [];
            this.throttleDuration = TimeSpan.FromSeconds(15);

            this.sessionCookie = GetSessionCookie();

            CookieContainer cookieContainer = new();

            cookieContainer.Add(new Uri(Consts.baseUri),
                new Cookie("session", this.sessionCookie));

            HttpClientHandler httpClientHandler = new()
            {
                CookieContainer = cookieContainer,
            };

            this.httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri($"{Consts.baseUri}/{Consts.year}/")
            };

            this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("gm-aoc-2024/1.0 (https://github.com/GessioMori/aoc-2024; gessiomori@protonmail.com)");
        }

        public async Task<ClientResponse> GetPuzzleInput(int dayNumber)
        {
            if (this.inputCache.TryGetValue(dayNumber, out var cachedResponse))
            {
                return cachedResponse;
            }

            if (this.requestTimestamps.TryGetValue(dayNumber, out var lastRequestTime))
            {
                TimeSpan timeSinceLastRequest = DateTime.UtcNow - lastRequestTime;
                if (timeSinceLastRequest < this.throttleDuration)
                {
                    await Task.Delay(this.throttleDuration - timeSinceLastRequest);
                }
            }

            if (string.IsNullOrEmpty(this.sessionCookie))
            {
                return new ClientResponse
                {
                    ResponseType = ClientResponseType.Failure,
                    Content = $"Session cookie not found."
                };
            }

            HttpResponseMessage response = await this.httpClient
                .GetAsync($"day/{dayNumber}/input");

            string content = await response.Content.ReadAsStringAsync();

            ClientResponse clientResponse = new()
            {
                ResponseType = response.IsSuccessStatusCode ?
                    ClientResponseType.Success :
                    ClientResponseType.Failure,
                Content = content
            };

            if (response.IsSuccessStatusCode)
            {
                this.inputCache[dayNumber] = clientResponse;
            }

            this.requestTimestamps[dayNumber] = DateTime.UtcNow;

            return clientResponse;
        }

        private static string? GetSessionCookie()
        {
            string filePath = Path.Combine("ProgramUtils", "session-cookie.txt");

            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
