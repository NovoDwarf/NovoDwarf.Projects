using System.Text;
using Mathematics.SourceGenerators.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mathematics.SourceGenerators.Generators.Distributions;

public class DistributionControllerGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var distributionClasses = context.CompilationProvider.Select(Selector);
        context.RegisterSourceOutput(distributionClasses, Execute);
    }

    private static void Execute(SourceProductionContext context, List<INamedTypeSymbol> distributions)
    {
        foreach (var distributionSymbol in distributions)
        {
            try
            {
                var controllerCode = GenerateController(distributionSymbol);
                context.AddSource($"{distributionSymbol.Name}_Controller.g.cs", SourceText.From(controllerCode, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticUtils.ErrorGeneratingDistributionController(), 
                    Location.None, 
                    distributionSymbol.Name, 
                    ex.Message
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static List<INamedTypeSymbol> Selector(Compilation compilation, CancellationToken cancellationToken)
    {
        var distributionTypes = new List<INamedTypeSymbol>();

        foreach (var module in compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(a => a.Modules))
        {
            distributionTypes.AddRange(module.GlobalNamespace.GetNamespaceMembers()
                .SelectMany(GetAllTypes)
                .Where(GeneratorUtils.IsDistributionSubclass));
        }

        return distributionTypes;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
        }

        foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(nestedNamespace))
            {
                yield return type;
            }
        }
    }

    private static string GenerateController(INamedTypeSymbol distributionSymbol)
    {
        var className = distributionSymbol.Name;
        var controllerName = $"{className}Controller";
        var distributionNamespace = distributionSymbol.ContainingNamespace.ToDisplayString();

        var distributionName = className.Replace("Distribution", "");
        var routeName = StringUtils.ToSnakeCase(distributionName);

        var constructors = distributionSymbol.Constructors;
        var parameterizedConstructor = constructors.FirstOrDefault(c => 
            c.Parameters.Length > 0 && !c.Parameters.Any(p => p.Type.SpecialType == SpecialType.System_String));
        
        var getDistributionMethod = GenerateGetDistributionMethod(className, parameterizedConstructor);

        var endpoints = new[]
        {
            new { Route = "", Method = "Distribute", Action = "Distribute()", Summary = "Generate distribution", EndpointName = $"{distributionName} Generate Distribution" },
            new { Route = "pdf/{x}", Method = "ProbabilityDensity", Action = "ProbabilityDensity(x)", Summary = "Calculate probability density function", EndpointName = $"{distributionName} Probability Density" },
            new { Route = "cdf/{x}", Method = "CumulativeDistribution", Action = "CumulativeDistribution(x)", Summary = "Calculate cumulative distribution function", EndpointName = $"{distributionName} Cumulative Distribution" },
            new { Route = "expected", Method = "Expected", Action = "Expected", Summary = "Get expected value", EndpointName = $"{distributionName} Expected Value" },
            new { Route = "mean", Method = "Mean", Action = "Mean", Summary = "Get mean value", EndpointName = $"{distributionName} Mean" },
            new { Route = "variance", Method = "Variance", Action = "Variance", Summary = "Get variance", EndpointName = $"{distributionName} Variance" },
            new { Route = "standard-deviation", Method = "StandardDeviation", Action = "StandardDeviation", Summary = "Get standard deviation", EndpointName = $"{distributionName} Standard Deviation" }
        };

        var endpointsCode = string.Join("\n\n", endpoints.Select(endpoint => 
            $$"""
                  /// <summary>
                  /// {{endpoint.Summary}}
                  /// </summary>
                  /// <response code="200">Returns calculation result</response>
                  /// <response code="400">Invalid parameters provided</response>
                  [HttpPost("{{endpoint.Route}}")]
                  [Produces("application/json")]
                  [ProducesResponseType(typeof(double), 200)]
                  [ProducesResponseType(typeof(ProblemDetails), 400)]
                  [EndpointName("{{endpoint.EndpointName}}")]
                  public IActionResult {{endpoint.Method}}([FromBody] CalculationRequest request{{(endpoint.Route.Contains("{x}") ? ", double x" : "")}})
                  {
                      try
                      {
                          var distribution = GetDistribution(request);
                          var result = distribution.{{endpoint.Action}};
                          
                          return Ok(result);
                      }
                      catch (Exception ex)
                      {
                          _logger.LogError(ex, "Error in {{endpoint.Method}}");
                          
                          return BadRequest(ex.Message);
                      }
                  }
              """));

        return $$"""
                 // <auto-generated/>
                 using Microsoft.AspNetCore.Mvc;
                 using Mathematics.Server.Base;
                 using Mathematics.Core;
                 using {{distributionNamespace}};

                 namespace Mathematics.Server.Controllers;

                 /// <summary>
                 /// {{distributionName}} distribution operations
                 /// </summary>
                 [ApiController]
                 [Route("distributions/{{routeName}}")]
                 [Produces("application/json")]
                 [EndpointGroupName("{{distributionName}} Distribution")]
                 public class {{controllerName}} : ControllerBase
                 {
                     private readonly ILogger<{{controllerName}}> _logger;

                     /// <summary>
                     /// {{distributionName}} distribution operations
                     /// </summary>
                     public {{controllerName}}(ILogger<{{controllerName}}> logger) => _logger = logger;
                     
                     {{endpointsCode}}

                     {{getDistributionMethod}}
                 }
                 """;
    }
    
    private static string GenerateGetDistributionMethod(string className, IMethodSymbol? parameterizedConstructor)
    {
        if (parameterizedConstructor == null)
        {
            return $$"""
                         private {{className}} GetDistribution(CalculationRequest request)
                         {
                             return new {{className}}();
                         }
                     """;
        }

        var parameterAssignments = new List<string>();
        var constructorParameters = parameterizedConstructor.Parameters;

        foreach (var parameter in constructorParameters)
        {
            var paramName = parameter.Name;
            var paramType = parameter.Type.ToDisplayString();
            
            switch (paramType)
            {
                case "double":
                    parameterAssignments.Add(ParseUtils.GenerateDouble(paramName));
                    break;
                case "double[]" or "System.Double[]":
                    parameterAssignments.Add(ParseUtils.GenerateDoubleArray(paramName));
                    break;
                case "double[]?":
                    parameterAssignments.Add(ParseUtils.GenerateNullableDoubleArray(paramName));
                    break;
                case "int":
                    parameterAssignments.Add(ParseUtils.GenerateInt(paramName));
                    break;
                default:
                    parameterAssignments.Add(ParseUtils.GenerateGeneric(paramName, paramType));
                    break;
            }
        }

        var parameterDeclarations = string.Join(";\n", constructorParameters.Select(p => 
            $"{p.Type.ToDisplayString()} {p.Name} = default"));

        var parameterNames = string.Join(", ", constructorParameters.Select(p => p.Name));

        return $$"""
                     private {{className}} GetDistribution(CalculationRequest request)
                     {
                         if (request.Params == null || request.Params.Count == 0)
                             return new {{className}}();
                      
                         {{parameterDeclarations}};
                         
                         bool hasParams = false;
                         
                         {{string.Join("\n  ", parameterAssignments)}}

                         if (hasParams)
                             return new {{className}}({{parameterNames}});
                         return new {{className}}();
                     }
                 """;
    }
}