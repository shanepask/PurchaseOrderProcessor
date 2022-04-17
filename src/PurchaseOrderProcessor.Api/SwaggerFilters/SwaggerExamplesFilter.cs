using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PurchaseOrderProcessor.Api.SwaggerFilters
{
	/// <summary>
	/// Swagger filter to add useable examples to the output document.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class SwaggerExamplesFilter : IOperationFilter
	{
		private static XDocument Doc => XDocument.Load(Startup.GeneratedContentLocation);

		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
            var examples = Doc.XPathSelectElements($"//members//member[starts-with(@name,\"M:{context.MethodInfo.DeclaringType!.FullName}\")]//example");

            foreach (var example in examples)
                AddExample(operation.RequestBody.Content.FirstOrDefault().Value, example);
        }

		private void AddExample(OpenApiMediaType value, XElement example)
		{
			var json = JsonSerializer.Deserialize<object>(example.Value);
            if (json == null)
                return;
			var apiObject = ToApiObject((JsonElement)json);
			value.Examples.Add(example.Attribute("name")?.Value ?? "Example", new OpenApiExample() { Description = example.Attribute("description")?.Value ?? "No Description Set", Value = apiObject });
		}

		private IOpenApiAny ToApiObject(JsonElement json)
		{
			switch (json.ValueKind)
			{
				case JsonValueKind.Undefined:
				case JsonValueKind.Object:
					{
						var obj = new OpenApiObject();
						foreach (var e in json.EnumerateObject())
							obj.Add(e.Name, ToApiObject(e.Value));

						return obj;
					}
				case JsonValueKind.Array:
					{
						var arr = new OpenApiArray();
						foreach (var e in json.EnumerateArray())
							arr.Add(ToApiObject(e));
						return arr;
					}
				case JsonValueKind.String:
					return new OpenApiString(json.GetString());

				case JsonValueKind.Number:
					return new OpenApiDouble(json.GetDouble());

				case JsonValueKind.True:
					return new OpenApiBoolean(true);

				case JsonValueKind.False:
					return new OpenApiBoolean(false);

				case JsonValueKind.Null:
				default:
					return new OpenApiNull();
			}
		}
	}
}
 