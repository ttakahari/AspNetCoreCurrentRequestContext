using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace AspNetCoreCurrentRequestContext.Tests
{
    public class AspNetCoreCurrentRequestContextTests
    {
        private const string Path = "/Home/Index";
        private const string HeaderValue = "TestHeader";
        private const string CacheValue = "TestCache";

        [Fact]
        public void Tests()
        {
            var builder = new ApplicationBuilder(null);

            builder.UseCurrentRequestContext();

            builder.Run(async _ =>
            {
                var threadId  = Thread.CurrentThread.ManagedThreadId;
                var requestId = Guid.NewGuid();

                Assert.NotNull(AspNetCoreHttpContext.Current);
                Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);

                AspNetCoreHttpContext.Current.Items["RequestId"] = requestId;
                AspNetCoreHttpContext.Current.Items["Cache"]     = CacheValue;

                await Task.Delay(100);

                Assert.NotNull(AspNetCoreHttpContext.Current);
                Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);

                await Task.Delay(100).ConfigureAwait(false);

                Assert.NotNull(AspNetCoreHttpContext.Current);
                Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);

                await Task.Run(() =>
                {
                    Assert.NotEqual(threadId, Thread.CurrentThread.ManagedThreadId);

                    Assert.NotNull(AspNetCoreHttpContext.Current);
                    Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                    Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                    Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                    Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);
                });

                await Task.Factory.StartNew(() =>
                {
                    Assert.NotEqual(threadId, Thread.CurrentThread.ManagedThreadId);

                    Assert.NotNull(AspNetCoreHttpContext.Current);
                    Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                    Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                    Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                    Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);
                });

                var semaphore = new SemaphoreSlim(1);

                ThreadPool.QueueUserWorkItem(__ =>
                {
                    Assert.NotEqual(threadId, Thread.CurrentThread.ManagedThreadId);

                    Assert.NotNull(AspNetCoreHttpContext.Current);
                    Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                    Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                    Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                    Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);

                    semaphore.Release();
                });

                await semaphore.WaitAsync();

                var thread = new Thread(__ =>
                {
                    Assert.NotEqual(threadId, Thread.CurrentThread.ManagedThreadId);

                    Assert.NotNull(AspNetCoreHttpContext.Current);
                    Assert.Equal(Path, AspNetCoreHttpContext.Current.Request.Path.Value);
                    Assert.Equal(HeaderValue, AspNetCoreHttpContext.Current.Request.Headers["test-header"]);
                    Assert.Equal(requestId, AspNetCoreHttpContext.Current.Items["RequestId"]);
                    Assert.Equal(CacheValue, AspNetCoreHttpContext.Current.Items["Cache"]);

                    semaphore.Release();
                });

                thread.Start();

                await semaphore.WaitAsync();
            });

            var app = builder.Build();

            app.Invoke(CreateHttpContext(Path, HeaderValue, CacheValue)).Wait();
        }

        private HttpContext CreateHttpContext(string path, string headerValue, object cacheValue)
        {
            var context = new DefaultHttpContext();

            context.Request.Path = new PathString(path);
            context.Request.Headers.Add("test-header", headerValue);

            return context;
        }
    }
}
