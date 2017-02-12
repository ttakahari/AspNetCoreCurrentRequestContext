# AspNetCoreCurrentRequestContext

Enable HttpContext.Current on ASP.NET Core applications.

## Install

from NuGet - [AspNetCoreCurrentRequestContext](https://www.nuget.org/packages/AspNetCoreCurrentRequestContext/)

```ps1
PM > Install-Package AspNetCoreCurrentRequestContext
```

## How to use

At first, you have to add ```CurrentRequestContextMiddleware``` to ```IApplicationBuilder``` on ```Configure``` method in ```Startup``` class.

You can use the extension-method that is ```UseCurrentRequestContext``` for doing that.

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // add CurrentRequestContextMiddleware.
        app.UseCurrentRequestContext();
    }
}
```

After that, if you call ```AspNetCoreHttpContext.Current``` like ```HttpContext.Current``` on ASP.NET applications, you can use ```HttpContext``` everywhere.

```csharp
// you can get HttpContext in the current request.
var context = AspNetCoreHttpContext.Current;
```

## Lisence

under [MIT Lisence](https://opensource.org/licenses/MIT).