using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>().ReverseMap();
        CreateMap<Sale, UpdateSaleResult>();
    }
}
