using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    public DeleteSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var deletedSale = await _saleRepository.DeleteAsync(request.Id, cancellationToken, "Products");
        if (deletedSale != null)
        {
            foreach (SaleItem saleItem in deletedSale.Products)
            {
                saleItem.MarkAsCancelled();
            }
            deletedSale.AddDomainEvent(new SaleCancelledEvent(deletedSale.Id, deletedSale.SaleNumber, deletedSale.CreatedAt));
            await _saleRepository.UpdateAsync(deletedSale, cancellationToken);
        }
        var result = new DeleteSaleResult { Success = deletedSale != null ? true : false };
        return result;
    }
}
