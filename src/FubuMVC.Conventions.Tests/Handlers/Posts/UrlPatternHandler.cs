using FubuMVC.Core;

namespace FubuMVC.Conventions.Tests.Handlers.Posts
{
    public class UrlPatternHandler
    {
        [UrlPattern("some-crazy-url/as-a-subfolder")]
        public JsonResponse Execute()
        {
            return new JsonResponse();
        }
    }
}