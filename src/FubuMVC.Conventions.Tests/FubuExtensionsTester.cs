using System.Collections.Generic;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Conventions.Tests
{
    [TestFixture]
    public class FubuExtensionsTester
    {
        [Test]
        public void should_include_handlers()
        {
            var graph = new FubuRegistry(x => x.ApplyHandlerConventions(typeof(Handlers.HandlersMarker))).BuildGraph();
            var routes = new List<string>
                             {
                                 "posts/create",
                                 "posts/complex-route",
                                 "some-crazy-url/as-a-subfolder",
                                 "posts/{Year}/{Month}/{Title}",
                                 "posts/category/{CategoryName}"
                             };

            routes
                .Each(route => graph
                                   .Routes
                                   .ShouldContain(r => r.Pattern.Equals(route)));
        }
    }
}