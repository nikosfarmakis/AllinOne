using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteEntities;
using AllinOne.Utils.Mappers.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AllinOne.Utils.Mappers
{
    internal class OrderMapper : IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest>
    {
        public Order ToEntity(CreateOrderRequest request)
        {
            return new Order
            {
                Description = request.Description
            };
        }

        public OrderResponse ToResponse(Order entity)
        {
            return new OrderResponse
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Description = entity.Description
            };
        }

        public void UpdateEntity(UpdateOrderRequest request, Order entity)
        {
            if (!string.IsNullOrEmpty(request.Description))
            {
                entity.Description = request.Description;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
    /*        public static OrderResponse ToOrderResponse(this Order order)
            {
                return new OrderResponse()
                {
                    CreatedAt = order.CreatedAt,
                    Description = order.Description,
                    Id = order.Id
                };
            }

            public static Order ToOrderDbModel(this CreateOrderRequest request)
            {
                return new Order()
                {
                    Description = request.Description
                };
            }

            public static void UpdateToOrder(this UpdateOrderRequest request, Order entity)
            {
                if (request.Description is not null)
                {
                    entity.Description = request.Description;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }*/



