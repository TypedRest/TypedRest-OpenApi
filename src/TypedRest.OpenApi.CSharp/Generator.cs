﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Builders;
using TypedRest.OpenApi.CSharp.Builders.Generic;
using TypedRest.OpenApi.CSharp.Builders.Raw;
using TypedRest.OpenApi.CSharp.Builders.Reactive;
using TypedRest.OpenApi.CSharp.Builders.Rpc;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class Generator
    {
        private readonly INamingConvention _naming;
        private readonly IDictionary<Type, IBuilder> _builders = new Dictionary<Type, IBuilder>();

        public Generator([NotNull] INamingConvention naming)
        {
            _naming = naming;

            Add(new DefaultBuilder());
            Add(new ElementBuilder());
            Add(new IndexerBuilder());
            Add(new CollectionBuilder());
            Add(new ActionBuilder());
            Add(new ProducerBuilder());
            Add(new ConsumerBuilder());
            Add(new FunctionBuilder());
            Add(new UploadBuilder());
            Add(new BlobBuilder());
            Add(new PollingBuilder());
            Add(new StreamingBuilder());
            Add(new StreamingCollectionBuilder());
        }

        public void Add<TEndpoint>(IBuilder<TEndpoint> builder)
            where TEndpoint : IEndpoint
            => _builders.Add(typeof(TEndpoint), builder);

        [NotNull, ItemNotNull]
        public ITypeList Generate([NotNull] EndpointList endpoints, IDictionary<string, OpenApiSchema> schemas)
        {
            var typeList = new TypeList();

            GenerateSchemas(schemas, typeList);

            var entryEndpoint = GenerateEntryEndpoint();
            entryEndpoint.Properties.AddRange(GenerateEndpoints(endpoints, typeList));
            typeList.Add(new Endpoint(), entryEndpoint);

            return typeList;
        }

        private void GenerateSchemas(IDictionary<string, OpenApiSchema> schemas, TypeList typeList)
        {
            foreach (var pair in schemas)
            {
                var type = GenerateSchema(_naming.SchemaType(pair.Key), typeList);
                typeList.Add(pair.Value, type);
            }
        }

        private static CSharpClass GenerateSchema(CSharpIdentifier identifier, TypeList typeList)
        {
            // TODO: Proper implementation
            return new CSharpClass(identifier);
        }

        // TODO: Make entry endpoint configurable
        private CSharpClass GenerateEntryEndpoint()
            => new CSharpClass(_naming.EndpointType("myEntry", new Endpoint()))
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters = {new CSharpParameter(CSharpIdentifier.Uri, "uri")}
                }
            };

        private IEnumerable<CSharpProperty> GenerateEndpoints(EndpointList endpoints, TypeList typeList)
            => endpoints.Select(x => GenerateEndpoint(x.Key, x.Value, typeList));

        private CSharpProperty GenerateEndpoint(string key, IEndpoint endpoint, TypeList typeList)
        {
            GenerateElementEndpoint(key, endpoint, typeList);
            var children = GenerateEndpoints(endpoint.Children, typeList).ToList();

            return children.Count == 0
                ? GenerateEndpointWithoutChildren(key, endpoint, typeList)
                : GenerateEndpointWithChildren(key, endpoint, typeList, children);
        }

        private CSharpProperty GenerateEndpointWithoutChildren(string key, IEndpoint endpoint, TypeList typeList)
        {
            var builder = _builders[endpoint.GetType()];

            return new CSharpProperty(builder.GetInterface(endpoint, typeList), _naming.Property(key))
            {
                GetterExpression = builder.GetConstruction(endpoint, typeList),
                Description = endpoint.Description
            };
        }

        private CSharpProperty GenerateEndpointWithChildren(string key, IEndpoint endpoint, TypeList typeList, List<CSharpProperty> children)
        {
            var builder = _builders[endpoint.GetType()];
            var endpointType = _naming.EndpointType(key, endpoint);

            var endpointInterface = new CSharpInterface(endpointType.ToInterface())
            {
                Interfaces = {builder.GetInterface(endpoint, typeList)},
                Description = endpoint.Description
            };
            endpointInterface.Properties.AddRange(children);
            typeList.Add(endpoint, endpointInterface);

            var endpointImplementation = new CSharpClass(endpointType)
            {
                BaseClass = builder.GetConstruction(endpoint, typeList),
                Interfaces = {endpointInterface.Identifier},
                Description = endpoint.Description
            };
            endpointImplementation.Properties.AddRange(children);
            typeList.Add(endpoint, endpointImplementation);

            return new CSharpProperty(endpointInterface.Identifier, key)
            {
                GetterExpression = endpointImplementation.GetConstruction(),
                Description = endpoint.Description
            };
        }

        private void GenerateElementEndpoint(string key, IEndpoint endpoint, TypeList typeList)
        {
            if (endpoint is IndexerEndpoint indexerEndpoint && indexerEndpoint.Element != null)
            {
                if (indexerEndpoint is CollectionEndpoint collectionEndpoint && collectionEndpoint.Element is ElementEndpoint elementEndpoint && elementEndpoint.Schema == null)
                    elementEndpoint.Schema = collectionEndpoint.Schema;

                GenerateEndpoint(key.TrimEnd('s'), indexerEndpoint.Element, typeList);
            }
        }
    }
}
