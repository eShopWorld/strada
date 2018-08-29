using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Web
{
    public interface IHttpRequestFunctions
    {
        bool TryGetBrandCode(HttpRequest httpRequest, string brandCodeHeaderKey, out string brandCode);
        bool TryGetCorrelationId(HttpRequest httpRequest, string correlationIdCookieKey, out string correlationId);
    }
}