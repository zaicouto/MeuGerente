using AutoMapper;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Domain.Entities;

namespace Modules.Orders.Application.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderItem, OrderItemResponseDto>().ReverseMap();

        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ReverseMap();
    }
}
