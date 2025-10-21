using System.ComponentModel;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleResponse
{
    /// <summary>
    /// Represents the unique identifier of the newly created sale
    /// </summary>
    public Guid Id { get; set; }
}
