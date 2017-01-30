using AspNetCoreCurrentRequestContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AspNetCoreCurrentRequestContext
{
    /// <summary>
    /// Enable using <see cref="AspNetCoreHttpContext.Current"/>. 
    /// </summary>
    public class CurrentRequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Create a new instance of <see cref="CurrentRequestContextMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public CurrentRequestContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Execute the application's request pipeline.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> in this request.</param>
        /// <returns>The task.</returns>
        public async Task Invoke(HttpContext context)
        {
            AspNetCoreHttpContext.Current = context;

            await _next(context);

            AspNetCoreHttpContext.Current = null;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions of <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enable <see cref="AspNetCoreHttpContext.Current"/> in the current request with adding <see cref="CurrentRequestContextMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The application's request pipeline.</param>
        /// <returns>The application's request pipeline added <see cref="CurrentRequestContextMiddleware"/>.</returns>
        public static IApplicationBuilder UseCurrentRequestContext(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<CurrentRequestContextMiddleware>();
        }
    }
}