using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Conventions
{
    public class HandlersUrlPolicy : IUrlPolicy
    {
        public const string HANDLER = "Handler";
        public const string METHOD = "Execute";
        public static readonly Regex HandlerExpression = new Regex("_[hH]andler", RegexOptions.Compiled);

        private readonly IEnumerable<Type> _markerTypes;

        public HandlersUrlPolicy(params Type[] markerTypes)
        {
            _markerTypes = markerTypes;
        }

        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if ((!call.HandlerType.Name.ToLower().EndsWith(HANDLER.ToLower()) && !HandlerExpression.IsMatch(call.HandlerType.Name))
                || call.Method.HasAttribute<UrlPatternAttribute>())
            {
                return false;
            }

            log.RecordCallStatus(call, "Matched on {0}".ToFormat(GetType().Name));
            return true;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var routeDefinition = call.ToRouteDefinition();
            string strippedNamespace = "";

            _markerTypes
                .Each(marker =>
                          {
                              strippedNamespace = call
                                  .HandlerType
                                  .Namespace
                                  .Replace(marker.Namespace + ".", string.Empty);
                          });

            if (strippedNamespace != call.HandlerType.Namespace)
            {
                if (!strippedNamespace.Contains("."))
                {
                    routeDefinition.Append(BreakUpCamelCaseWithHypen(strippedNamespace));
                }
                else
                {
                    var patternParts = strippedNamespace.Split(new[] { "." }, StringSplitOptions.None);
                    foreach (var patternPart in patternParts)
                    {
                        routeDefinition.Append(BreakUpCamelCaseWithHypen(patternPart.Trim()));
                    }
                }
            }

            var handlerName = call.HandlerType.Name;
            var match = HandlerExpression.Match(handlerName);
            if (match.Success && MethodToUrlBuilder.Matches(handlerName))
            {
                // We're forcing handlers to end with "_handler" in this case
                handlerName = handlerName.Substring(0, match.Index);
                var properties = call.HasInput
                                 ? new TypeDescriptorCache().GetPropertiesFor(call.InputType()).Keys
                                 : new string[0];

                // TODO: verify behavior when route parameters are optional
                MethodToUrlBuilder.Alter(routeDefinition, handlerName, properties, text => { });

                if (call.HasInput)
                {
                    routeDefinition.ApplyInputType(call.InputType());
                }
            }
            else
            {
                // Otherwise we're expecting something like "GetHandler"
                var httpMethod = call.HandlerType.Name.Replace(HANDLER, string.Empty);
                routeDefinition.ConstrainToHttpMethods(httpMethod.ToUpper());
            }

            return routeDefinition;
        }

        private static string BreakUpCamelCaseWithHypen(string input)
        {
            var routeBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                if (i != 0 && char.IsUpper(input[i]))
                {
                    routeBuilder.Append("-");
                }

                routeBuilder.Append(input[i]);
            }

            return routeBuilder
                .ToString()
                .ToLower();
        }
    }
}