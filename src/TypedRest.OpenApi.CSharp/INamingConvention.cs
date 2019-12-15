﻿using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface INamingConvention
    {
        string Property(string key);

        CSharpIdentifier EndpointType(string key, IEndpoint endpoint);

        CSharpIdentifier DtoType(string key);

        CSharpIdentifier DtoFor(OpenApiSchema schema);
    }
}
