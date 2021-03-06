﻿using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos
{
    public class DtoGenerator
    {
        private readonly INamingStrategy _naming;

        public DtoGenerator(INamingStrategy naming)
        {
            _naming = naming;
        }

        public IEnumerable<ICSharpType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
        {
            foreach ((string key, var schema) in schemas)
            {
                var builder = DtoBuilder.For(key, schema, _naming);
                if (builder != null)
                {
                    foreach (var type in builder.BuildTypes())
                        yield return type;
                }
            }
        }
    }
}
