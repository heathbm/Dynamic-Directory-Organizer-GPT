using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Models;

namespace Infrastructure
{
    public class OpenAiClientFactory(IOptions<Settings> options)
    {
        public OpenAIClient GetOpenApiClient()
        {
            Uri proxyUrl = new(options.Value.Endpoint!);
            AzureKeyCredential token = new(options.Value.Key!);
            return new(proxyUrl, token);
        }
    }
}
