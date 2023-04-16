using System.Net;
using System.Net.Mime;
using MPay.Core.DTO;
using MPay.Core.Services;
using MPay.Infrastructure.DAL.UnitOfWork;
using MPay.Infrastructure.ErrorHandling;
using MPay.Infrastructure.Validation;

namespace MPay.API.Endpoints;

internal static class PurchaseEndpoints
{
    private const string BasePath = "/purchases";

    private const string PurchaseTag = "Purchase";
    private const string PaymentTag = "Payment";

    internal static void MapPurchaseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath);

        group.MapPost("/", async (IPurchaseService purchaseService, AddPurchaseDto addPurchaseDto) =>
            {
                var id = await purchaseService.AddAsync(addPurchaseDto);
                return TypedResults.Created($"{BasePath}/{id}", id);
            })
            .WithTags(PurchaseTag)
            .WithDescription("Adds new purchase")
            .Accepts<AddPurchaseDto>(MediaTypeNames.Application.Json)
            .Produces<string>((int)HttpStatusCode.Created)
            .Produces<ErrorDetails>((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.InternalServerError)
            .WithOpenApi()
            .AddEndpointFilter<ValidationEndpointFilter>();

        group.MapGet("/{id}", async (IPurchaseService purchaseService, string id)
                => TypedResults.Ok(await purchaseService.GetPendingAsync(id)))
            .WithTags(PurchaseTag)
            .WithDescription("Gets pending purchase")
            .Produces<PurchaseDto>()
            .Produces<ErrorDetails>((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.InternalServerError)
            .WithOpenApi();

        group.MapPost("/{id}/cancel", async (IPurchaseService purchaseService, string id) =>
            {
                await purchaseService.CancelPurchaseAsync(id);
                return TypedResults.NoContent();
            })
            .WithTags(PurchaseTag)
            .WithDescription("Cancels pending purchase")
            .Produces((int)HttpStatusCode.NoContent)
            .Produces<ErrorDetails>((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.InternalServerError)
            .WithOpenApi();

        group.MapPost("/{id}/payment",
                async (IPurchasePaymentService purchasePaymentService, string id,
                    PurchasePaymentDto purchasePaymentDto) =>
                {
                    var result = await purchasePaymentService.ProcessPaymentAsync(id, purchasePaymentDto);
                    return result.IsCompleted
                        ? (IResult)TypedResults.Ok()
                        : TypedResults.BadRequest(ErrorDetailsExtensions.MapFrom(result));
                })
            .WithTags(PaymentTag)
            .WithDescription("Processes payment for pending purchase")
            .Accepts<PurchasePaymentDto>(MediaTypeNames.Application.Json)
            .Produces((int)HttpStatusCode.OK)
            .Produces<ErrorDetails>((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.InternalServerError)
            .WithOpenApi()
            .AddEndpointFilter<ValidationEndpointFilter>()
            .AddEndpointFilter<TransactionalEndpointFilter>();
    }
}