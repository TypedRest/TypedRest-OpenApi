using System.Collections.Generic;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// A list of named <see cref="IEndpoint"/>s.
    /// </summary>
    public class EndpointList : Dictionary<string, IEndpoint>, IOpenApiSerializable, IOpenApiExtension
    {
        /// <summary>
        /// Adds <see cref="IEndpoint"/>s parsed from an OpenAPI Object.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="parser">The endpoint parser to use.</param>
        public void Parse(OpenApiObject data, IEndpointParser parser)
        {
            foreach (var property in data)
            {
                if (property.Value is OpenApiObject objData)
                    Add(property.Key, parser.Parse(objData));
            }
        }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            foreach (var item in this)
                writer.WriteOptionalObject(item.Key, item.Value, specVersion);
            writer.WriteEndObject();
        }

        public void SerializeAsV2(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi2_0);

        public void SerializeAsV3(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi3_0);

        /// <summary>
        /// Adds all <paramref name="endpoints"/> to the list.
        /// </summary>
        public void AddRange(IDictionary<string, IEndpoint> endpoints)
        {
            foreach (var pair in endpoints)
                Add(pair.Key, pair.Value);
        }
    }
}
