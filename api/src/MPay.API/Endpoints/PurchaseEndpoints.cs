using FluentValidation;

namespace MPay.API.Endpoints;

internal static class PurchaseEndpoints
{
    private const string BasePath = "/purchases";

    internal static void MapPurchaseEndpoints(this WebApplication app)
    {
        app.MapPost(BasePath, async (IPurchaseService purchaseService, IValidator<AddPurchaseDto> validator, 
            AddPurchaseDto addPurchaseDto) =>
        {
            var validationResult = await validator.ValidateAsync(addPurchaseDto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var id = await purchaseService.AddAsync(addPurchaseDto);

            return Results.Created($"{BasePath}/{id}", id);
        });

        app.MapGet($"{BasePath}/{{id}}", async (IPurchaseService purchaseService, string id) =>
        {
            var purchase = await purchaseService.GetPendingAsync(id);
            
            return Results.Ok(purchase);
        });
    }
}