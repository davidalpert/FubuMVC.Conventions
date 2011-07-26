using FubuMVC.Conventions.Tests.Handlers.Posts;
using FubuMVC.Conventions.Tests.Handlers.Posts.Create;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Conventions.Tests
{
    public class ObjectMother
    {
        public static ActionCall HandlerCall()
        {
            return ActionCall.For<GetHandler>(h => h.Execute(new CreatePostRequestModel()));
        }
        public static ActionCall ComplexHandlerCall()
        {
            return ActionCall.For<Handlers.Posts.ComplexRoute.GetHandler>(h => h.Execute());
        }

        public static ActionCall NonHandlerCall()
        {
            return ActionCall.For<SampleController>(c => c.Hello());
        }

        public static ActionCall HandlerWithAttributeCall()
        {
            return ActionCall.For<UrlPatternHandler>(h => h.Execute());
        }

        public static ActionCall HandlerWithRouteInput()
        {
            return ActionCall.For<get_Year_Month_Title_handler>(h => h.Execute(new ViewPostRequestModel()));
        }
    }
}