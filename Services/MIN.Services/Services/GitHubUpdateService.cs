using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Updates;
using System.Text.Json;

namespace MIN.Services.Services
{
    /// <inheritdoc cref="IUpdateService"/>
    public class GitHubUpdateService : IUpdateService
    {
        private readonly ILoggerProvider logger;
        private readonly string githubApiUrl = "https://api.github.com/repos/{owner}/{repo}/releases/latest";
        private readonly Version currentVersion;

        public GitHubUpdateService(ILoggerProvider logger, Version currentVersion)
        {
            this.logger = logger;
            this.currentVersion = currentVersion;
        }

        public async Task<UpdateCheckResult> CheckForUpdatesAsync(string owner, string repo, CancellationToken ct = default)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "MIN-Desktop-App");

                var url = githubApiUrl.Replace("{owner}", owner).Replace("{repo}", repo);
                var response = await httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var latestVersionStr = root.GetProperty("tag_name").GetString()?.TrimStart('v', 'V');
                if (latestVersionStr == null)
                {
                    return new UpdateCheckResult();
                }

                var latestVersion = new Version(latestVersionStr);
                var releaseUrl = root.GetProperty("html_url").GetString();
                var releaseNotes = root.GetProperty("body").GetString();

                return new UpdateCheckResult
                {
                    IsUpdateAvailable = latestVersion > currentVersion,
                    LatestVersion = latestVersionStr,
                    ReleaseUrl = releaseUrl,
                    ReleaseNotes = releaseNotes
                };
            }
            catch (Exception ex)
            {
                logger.Log($"Что то неполучилось проверить на обновы: {ex.Message}",
                    LogLevel.Error);
                return new UpdateCheckResult();
            }
        }
    }
}
