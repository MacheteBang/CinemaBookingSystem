using Microsoft.AspNetCore.Routing;

namespace CinemaBooking.Common;

public interface IEndpoint
{
    void AddRoutes(IEndpointRouteBuilder app);
}