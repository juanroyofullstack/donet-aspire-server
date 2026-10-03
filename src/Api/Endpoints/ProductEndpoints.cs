using NetAspireServer.Api.Contracts.Products;
using NetAspireServer.Application.Services;
using NetAspireServer.Domain.Entities;

namespace NetAspireServer.Api.Endpoints;

public static class ProductEndpoints
{
    private const string ProductsRoute = "/products";
    private const string ProductsTag = "Products";
    private const string GetProductsOperationName = "GetProducts";
    private const string GetProductByIdOperationName = "GetProductById";
    private const string GetProductsSummary = "Gets all products";
    private const string GetProductsDescription = "Returns all products currently stored by the active repository implementation.";
    private const string CreateProductOperationName = "CreateProduct";
    private const string CreateProductSummary = "Creates a new product";
    private const string CreateProductDescription = "Creates a new product and returns the created resource.";

    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ProductsRoute)
            .WithTags(ProductsTag);

        group.MapGet(string.Empty, async (ProductService service, CancellationToken cancellationToken) =>
            {
                var products = await service.GetAllAsync(cancellationToken);
                var response = products.Select(MapToResponse).ToArray();
                return TypedResults.Ok(response);
            })
            .WithName(GetProductsOperationName)
            .WithSummary(GetProductsSummary)
            .WithDescription(GetProductsDescription)
            .Produces<ProductResponse[]>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetProductByIdAsync)
            .WithName(GetProductByIdOperationName)
            .WithSummary("Gets a product by ID")
            .WithDescription("Retrieves a product by its ID.")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost(string.Empty, async (CreateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                var product = await service.CreateAsync(request.Name, request.Price, cancellationToken);
                var response = MapToResponse(product);
                return TypedResults.Created($"{ProductsRoute}/{response.Id}", response);
            })
            .WithName(CreateProductOperationName)
            .WithSummary(CreateProductSummary)
            .WithDescription(CreateProductDescription)
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetProductByIdAsync(Guid id, ProductService service, CancellationToken cancellationToken)
    {
        var product = await service.GetByIdAsync(id, cancellationToken);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(MapToResponse(product));
    }

    private static ProductResponse MapToResponse(Product product)
        => new(product.Id, product.Name, product.Price);

    private static Dictionary<string, string[]> Validate(CreateProductRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors[nameof(request.Name)] = ["Name is required."];
        }

        if (request.Price < 0)
        {
            errors[nameof(request.Price)] = ["Price cannot be negative."];
        }

        return errors;
    }
}
