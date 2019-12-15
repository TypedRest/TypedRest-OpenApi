﻿using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for <see cref="Endpoint"/>s.
    /// </summary>
    public class DefaultBuilder : BuilderBase<Endpoint>
    {
        protected override CSharpIdentifier GetImplementationType(Endpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(Namespace.Name, "EndpointBase");

        protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
            => new CSharpIdentifier(Namespace.Name, "IEndpoint");
    }
}
