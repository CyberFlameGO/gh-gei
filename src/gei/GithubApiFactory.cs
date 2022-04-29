using System.Net.Http;

namespace OctoshiftCLI.GithubEnterpriseImporter
{
    public sealed class GithubApiFactory : ISourceGithubApiFactory, ITargetGithubApiFactory
    {
        private const string DEFAULT_API_URL = "https://api.github.com";

        private readonly OctoLogger _octoLogger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly EnvironmentVariableProvider _environmentVariableProvider;
        private readonly RetryPolicy _retryPolicy;
        private readonly IVersionProvider _versionProvider;

        public GithubApiFactory(OctoLogger octoLogger, IHttpClientFactory clientFactory, EnvironmentVariableProvider environmentVariableProvider, RetryPolicy retryPolicy, IVersionProvider versionProvider)
        {
            _octoLogger = octoLogger;
            _clientFactory = clientFactory;
            _environmentVariableProvider = environmentVariableProvider;
            _retryPolicy = retryPolicy;
            _versionProvider = versionProvider;
        }

        GithubApi ISourceGithubApiFactory.Create(string apiUrl, string sourcePersonalAccessToken, string commandName)
        {
            apiUrl ??= DEFAULT_API_URL;
            sourcePersonalAccessToken ??= _environmentVariableProvider.SourceGithubPersonalAccessToken();
            var githubClient = new GithubClient(_octoLogger, _clientFactory.CreateClient("Default"), sourcePersonalAccessToken, _versionProvider?.GetProductVersionHeaderValue(commandName));
            return new GithubApi(githubClient, apiUrl, _retryPolicy);
        }

        GithubApi ISourceGithubApiFactory.CreateClientNoSsl(string apiUrl, string sourcePersonalAccessToken, string commandName)
        {
            apiUrl ??= DEFAULT_API_URL;
            sourcePersonalAccessToken ??= _environmentVariableProvider.SourceGithubPersonalAccessToken();
            var githubClient = new GithubClient(_octoLogger, _clientFactory.CreateClient("NoSSL"), sourcePersonalAccessToken, _versionProvider?.GetProductVersionHeaderValue(commandName));
            return new GithubApi(githubClient, apiUrl, _retryPolicy);
        }

        GithubApi ITargetGithubApiFactory.Create(string targetPersonalAccessToken, string commandName) => (this as ITargetGithubApiFactory).Create(null, targetPersonalAccessToken, commandName);

        GithubApi ITargetGithubApiFactory.Create(string apiUrl, string targetPersonalAccessToken, string commandName)
        {
            apiUrl ??= DEFAULT_API_URL;
            targetPersonalAccessToken ??= _environmentVariableProvider.TargetGithubPersonalAccessToken();
            var githubClient = new GithubClient(_octoLogger, _clientFactory.CreateClient("Default"), targetPersonalAccessToken, _versionProvider?.GetProductVersionHeaderValue(commandName));
            return new GithubApi(githubClient, apiUrl, _retryPolicy);
        }
    }
}
