using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Dashboard.Application.Jobs.Filters;

internal class SkipAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
