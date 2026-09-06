using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace SmartLoan.Application.Dashboard.Queries;

public record AdminDashboardDto(
    int TotalCustomers,
    int PendingLoans,
    decimal TotalMoneyCollected,
    decimal ProjectedProfit);

public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>;

public class GetAdminDashboardHandler(IApplicationDbContext context, HybridCache cache,ILogger<GetAdminDashboardHandler> logger)
    : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        // Define a unique key for the cache
        string cacheKey = "admin_dashboard_stats";

        // HybridCache: It checks the "L1" (Memory) first. 
        // If not there, it runs the "Factory" (our logic).
       return await cache.GetOrCreateAsync(
          key: cacheKey,
          state: context,
           factory: async(ctx, token) =>
           {
               logger.LogInformation("--- CACHE MISS: Fetching fresh data from PostgreSQL ---");
               var totalCustomers =
                   await context.Customers.CountAsync(token);

               var pendingLoans =
                   await context.LoanApplications
                       .CountAsync(l => l.Status == "Saving", token);

               var totalCollected =
                   await context.Payments
                       .SumAsync(p => p.Amount, token);

               var totalFees =
                   await context.LoanApplications
                       .SumAsync(l => l.ServiceFee, token);

               return new AdminDashboardDto(
                   totalCustomers,
                   pendingLoans,
                   totalCollected,
                   totalFees);
           },
           tags: ["dashboard"],
           cancellationToken: cancellationToken);
    }
}