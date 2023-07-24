using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6
{
    public class XmlApiClient
    {
        private readonly string _baseUrl;
        private readonly CredentialStore _credStore;
        private readonly ICliLogger _logger;
        private readonly HttpClient _httpClient;

        private string _token;

        public XmlApiClient(
            Settings settings,
            CredentialStore credStore,
            ICliLogger logger
        )
        {
            _baseUrl = settings.Vertec.BaseUrl;
            _credStore = credStore;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task Authenticate()
        {
            var credentials = _credStore.VertecCredentials;
            var login = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("vertec_username", credentials.UserName),
                new KeyValuePair<string, string>("password", credentials.Password),
            });

            var authUrl = $"{_baseUrl}/auth/xml";
            _logger.LogInfo($"Calling {authUrl}");

            var response = await _httpClient.PostAsync(authUrl, login);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _token = responseText;
                _logger.LogInfo($"Authentication token: {_token}");
            }
            else
            {
                _logger.LogError($"Authentication failed: {responseText}");
                throw new Exception($"Vertec authentication failed with response: {responseText}");
            }
        }

        public async Task<ApiResult> Request(Request request)
        {
            var envelope = new Envelope();
            envelope.Header.BasicAuth.Token = _token;
            envelope.Body.Request = request;

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(envelope.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false), // to not generate BOM character
                OmitXmlDeclaration = true,
            };
            using var writer = XmlWriter.Create(sb, settings);

            var ns = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(null) });
            serializer.Serialize(writer, envelope, ns);

            var content = new StringContent(sb.ToString());

            var xmlUrl = $"{_baseUrl}/xml";
            _logger.LogInfo($"Calling {xmlUrl}");

            _logger.LogDebug(new DebugContent("Request", () => sb.ToString()));

            var response = await _httpClient.PostAsync(xmlUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Log(_logger.CreateError(result));
                throw new Exception("Request failed");
            }
            
            _logger.LogDebug(new DebugContent("Response", () => result));

            return new ApiResult(result);
        }
    }
}
