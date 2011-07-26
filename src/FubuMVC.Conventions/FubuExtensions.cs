using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.DSL;

namespace FubuMVC.Conventions
{
    public static class FubuExtensions
    {
        public static void ApplyHandlerConventions(this FubuRegistry registry, params Type[] markerTypes)
        {
            markerTypes
                .Each(t => registry
                               .Applies
                               .ToAssembly(t.Assembly));

            registry
                .Actions
                .IncludeHandlers(markerTypes);

            registry
                .Routes
                .UrlPolicy(new HandlersUrlPolicy(markerTypes));
        }

        public static ActionCallCandidateExpression IncludeHandlers(this ActionCallCandidateExpression actions, params Type[] markerTypes)
        {
            markerTypes.Each(markerType => actions.IncludeTypes(t => t.Namespace.StartsWith(markerType.Namespace)));
            return actions.IncludeMethods(action => action.Method.Name == HandlersUrlPolicy.METHOD);
        }
    }
}