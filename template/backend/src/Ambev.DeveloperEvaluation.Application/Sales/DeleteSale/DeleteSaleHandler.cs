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

        var deleted = await _saleRepository.DeleteAsync(request.Id, cancellationToken);
        if (deleted)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException("Sale not found");
            foreach (SaleItem saleItem in sale.Products)
            {
                saleItem.MarkAsCancelled();
            }
            await _saleRepository.UpdateAsync(sale, cancellationToken);
        }
        var result = new DeleteSaleResult { Success = deleted };
        return result;
    }
}
