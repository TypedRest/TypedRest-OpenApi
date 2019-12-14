﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public class TypeList : ITypeList
    {
        private readonly List<CSharpType> _types = new List<CSharpType>();

        public IEnumerator<CSharpType> GetEnumerator()
            => _types.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _types.GetEnumerator();

        private readonly Dictionary<IEndpoint, CSharpIdentifier> _endpointImplementations = new Dictionary<IEndpoint, CSharpIdentifier>();

        public void Add(IEndpoint endpoint, CSharpClass type)
        {
            _types.Add(type);
            _endpointImplementations.Add(endpoint, type.Identifier);
        }

        public CSharpIdentifier ImplementationFor(IEndpoint endpoint)
            => _endpointImplementations[endpoint];

        private readonly Dictionary<IEndpoint, CSharpIdentifier> _endpointInterfaces = new Dictionary<IEndpoint, CSharpIdentifier>();

        public void Add(IEndpoint endpoint, CSharpInterface type)
        {
            _types.Add(type);
            _endpointInterfaces.Add(endpoint, type.Identifier);
        }

        public CSharpIdentifier InterfaceFor(IEndpoint endpoint)
            => _endpointInterfaces.TryGetValue(endpoint, out var result)
                ? result
                : ImplementationFor(endpoint); // Fallback to implementation if there is no interface

        private readonly Dictionary<string, CSharpIdentifier> _dtos = new Dictionary<string, CSharpIdentifier>();

        public void Add(OpenApiSchema schema, CSharpType type)
        {
            _types.Add(type);

            string key = schema.Reference?.Id ?? schema.Type;
            if (!string.IsNullOrEmpty(key))
                _dtos.Add(key, type.Identifier);
        }

        public CSharpIdentifier DtoFor(OpenApiSchema schema)
            => _dtos[schema.Reference?.Id ?? schema.Type];
    }
}
