using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace ET.WebAPI.Api.SwaggerSettings
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;
            
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(n => schema.Enum.Add(new OpenApiString(n)));
        }
    }
}