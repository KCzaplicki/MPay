namespace MPay.API.Endpoints;

internal static class PurchaseEndpoints
{
    private const string BasePath = "/purchases";

    internal static void MapPurchaseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath);

        group.MapPost("/", async (IPurchaseService purchaseService, AddPurchaseDto addPurchaseDto) =>
        {
            var id = await purchaseService.AddAsync(addPurchaseDto);
            return TypedResults.Created($"{BasePath}/{id}", id);
        })
        .AddEndpointFilter<ValidationEndpointFilter>();

        group.MapGet("/{id}", async (IPurchaseService purchaseService, string id) 
            => TypedResults.Ok(await purchaseService.GetPendingAsync(id)));

        group.MapPost("/{id}/cancel", async (IPurchaseService purchaseService, string id) =>
        {
            await purchaseService.CancelPurchaseAsync(id);
            return TypedResults.NoContent();
        });

        group.MapPost("/{id}/payment", 
            async (IPurchasePaymentService purchasePaymentService, string id, PurchasePaymentDto purchasePaymentDto) =>
        {
            await purchasePaymentService.ProcessPaymentAsync(id, purchasePaymentDto);
            return TypedResults.Ok();
        })
        .AddEndpointFilter<ValidationEndpointFilter>();
    }
}