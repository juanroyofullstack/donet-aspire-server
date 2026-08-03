using NetAspireServer.Api.Contracts.Products;
using NetAspireServer.Application.Services;
using NetAspireServer.Domain.Entities;

namespace NetAspireServer.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        group.MapGet(string.Empty, async (ProductService service, CancellationToken cancellationToken) =>
            {
                var products = await service.GetAllAsync(cancellationToken);
                var response = products.Select(MapToResponse).ToArray();
                return TypedResults.Ok(response);
            })
            .WithName("GetProducts")
            .WithSummary("Gets all products")
            .WithDescription("Returns all products currently stored by the active repository implementation.")
            .Produces<ProductResponse[]>(StatusCodes.Status200OK);

        group.MapPost(string.Empty, async (CreateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
            {
                var product = await service.CreateAsync(request.Name, request.Price, cancellationToken);
                var response = MapToResponse(product);
                return TypedResults.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .WithSummary("Creates a new product")
            .WithDescription("Creates a new product and returns the created resource.")
            .Produces<ProductResponse>(StatusCodes.Status201Created);
    }

    private static ProductResponse MapToResponse(Product product)
        => new(product.Id, product.Name, product.Price);
}
