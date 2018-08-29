using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Web
{
    /// <summary>
    ///     HttpRequestExtensions provides a means of retrieving domain-specific metadata from HTTP requests.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        ///     TryGetBrandCode returns <c>true</c> if the HTTP request contains a header with header-key equal to
        ///     <see cref="brandCodeHeaderKey" />.
        /// </summary>
        /// <param name="httpRequest">The underlying HTTP request</param>
        /// <param name="brandCodeHeaderKey">The header-key associated with the brand-code</param>
        /// <param name="brandCode">The brand-code</param>
        public static bool TryGetBrandCode(
            this HttpRequest httpRequest,
            string brandCodeHeaderKey,
            out string brandCode)
        {
            var gotHeader = httpRequest.Headers.TryGetValue(brandCodeHeaderKey, out var headerValues);

            if (gotHeader)
            {
                brandCode = headerValues.LastOrDefault();
                return true;
            }

            brandCode = null;
            return false;
        }

        /// <summary>
        ///     GetCorrelationId returns <c>true</c> if the HTTP request contains a cookie with cookie-key equal to
        ///     <see cref="correlationIdCookieKey" />.
        /// </summary>
        /// <param name="httpRequest">The underlying HTTP request</param>
        /// <param name="correlationIdCookieKey">The header-key associated with the brand-code</param>
        /// <param name="correlationId">The correlation-id</param>
        /// <returns></returns>
        public static bool GetCorrelationId(
            this HttpRequest httpRequest,
            string correlationIdCookieKey,
            out string correlationId)
        {
            return httpRequest.Cookies.TryGetValue(correlationIdCookieKey, out correlationId);
        }
    }
}