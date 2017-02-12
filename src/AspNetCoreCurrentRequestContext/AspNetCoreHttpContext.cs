using Microsoft.AspNetCore.Http;
using System;
using System.Threading;

namespace AspNetCoreCurrentRequestContext
{
    /// <summary>
    /// The class that manages <see cref="HttpContext"/> in the current request.
    /// </summary>
    public class AspNetCoreHttpContext
    {
        private static readonly AsyncLocal<HttpContext> _context = new AsyncLocal<HttpContext>();

        /// <summary>
        /// Get <see cref="HttpContext"/> in this request.
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                if (_context.Value == null)
                {
                    throw new InvalidOperationException($"Could not acuire {nameof(HttpContext)} in the current request. You may not add {nameof(CurrentRequestContextMiddleware)} in Startup.");
                }

                return _context.Value;
            }
            internal set { _context.Value = value; }
        }
    }
}
