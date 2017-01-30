using Microsoft.AspNetCore.Http;
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
            get { return _context.Value; }
            internal set { _context.Value = value; }
        }
    }
}
