using System.Net.Http;

namespace OctoshiftCLI.AdoToGithub
{
    public class GithubApiFactory
    {
        private const string DEFAULT_API_URL = "https://api.github.com";

        private readonly OctoLogger _octoLogger;
        private readonly HttpClient _client;
        private readonly EnvironmentVariableProvider _environmentVariableProvider;
        private readonly RetryPolicy _retryPolicy;
        private readonly IVersionProvider _versionProvider;

        public GithubApiFactory(OctoLogger octoLogger, HttpClient client, EnvironmentVariableProvider environmentVariableProvider, RetryPolicy retryPolicy, IVersionProvider versionProvider)
        {
            _octoLogger = octoLogger;
            _client = client;
            _environmentVariableProvider = environmentVariableProvider;
            _retryPolicy = retryPolicy;
            _versionProvider = versionProvider;
        }

        public virtual GithubApi Create(string personalAccessToken, string commandName) => Create(null, personalAccessToken, commandName);

        public virtual GithubApi Create(string apiUrl, string personalAccessToken, string commandName)
        {
            apiUrl ??= DEFAULT_API_URL;
            personalAccessToken ??= _environmentVariableProvider.GithubPersonalAccessToken();
            var githubClient = new GithubClient(_octoLogger, _client, personalAccessToken, _versionProvider?.GetProductVersionHeaderValue(commandName));
            return new GithubApi(githubClient, apiUrl, _retryPolicy);
        }
    }
}
