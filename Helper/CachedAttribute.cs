using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using TeamTaskManagement.Core.Service;

namespace TeamTaskManagement.Api.Helper
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSec;

        public CachedAttribute(int ExpireTimeInSec)
        {
            _expireTimeInSec = ExpireTimeInSec;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var cashedService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
           var casheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
           var casheResponse = await cashedService.GetCachedRsponse(casheKey);
            if (!string.IsNullOrEmpty(casheResponse))
            { 
                var contentResult = new ContentResult()
                { 
                    Content = casheResponse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }
          var ExecutedEndPointContext= await next.Invoke();
            if (ExecutedEndPointContext.Result is OkObjectResult result)
            {
               await cashedService.CacheResponseAsync(casheKey,result.Value,TimeSpan.FromSeconds(_expireTimeInSec));
            }
           
        }
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);
            foreach (var (key, value) in request.Query.OrderBy(x=>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
