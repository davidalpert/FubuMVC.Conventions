using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Conventions.Tests
{
    [TestFixture]
    public class HandlersUrlPolicyTester
    {
        private HandlersUrlPolicy _policy;

        [SetUp]
        public void before_each()
        {
            _policy = new HandlersUrlPolicy(typeof(Handlers.HandlersMarker));
        }

        [Test]
        public void should_only_match_calls_with_handler_type_ending_with_handler()
        {
            var log = new NulloConfigurationObserver();
            _policy
                .Matches(ObjectMother.HandlerCall(), log)
                .ShouldBeTrue();
            _policy
                .Matches(ObjectMother.NonHandlerCall(), log)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_calls_with_url_pattern_attribute()
        {
            var log = new NulloConfigurationObserver();
            _policy
                .Matches(ObjectMother.HandlerWithAttributeCall(), log)
                .ShouldBeFalse();
        }

        [Test]
        public void should_strip_root_namespace_and_treat_child_namespaces_as_folders()
        {
            _policy
                .Build(ObjectMother.HandlerCall())
                .Pattern
                .ShouldEqual("posts/create");
        }

        [Test]
        public void should_constrain_routes_by_class_name_without_handler()
        {
            _policy
                .Build(ObjectMother.HandlerCall())
                .AllowedHttpMethods
                .ShouldContain("GET");
        }

        [Test]
        public void should_use_hyphen_to_break_up_camel_casing()
        {
            _policy
                .Build(ObjectMother.ComplexHandlerCall())
                .Pattern
                .ShouldEqual("posts/complex-route");
        }

        [Test]
        public void should_contrain_routes_by_MethodToUrlBuilder_if_they_match()
        {
            var input = _policy.Build(ObjectMother.HandlerWithRouteInput()).Input;
            var parameters = new RouteParameters();
            parameters["Year"] = "2011";
            parameters["Month"] = "7";
            parameters["Title"] = "hello-world";

            input
                .CreateUrlFromParameters(parameters)
                .ShouldEqual("posts/2011/7/hello-world");
        }

        [Test]
        public void should_match_constrained_route_even_if_parameters_are_not_provided()
        {
            var input = _policy.Build(ObjectMother.HandlerWithOptionalRouteInput()).Input;
            var parameters = new RouteParameters();

            input
                .CreateUrlFromParameters(parameters)
                .ShouldEqual("posts/category/");
        }
    }
}
