using Maui.Health.Enums;
using Maui.Health.Models;
using Maui.Health.Models.Metrics;

namespace Maui.Health.Services;

public partial class HealthService
{
    public partial bool IsSupported => false;

    public partial Task<RequestPermissionResult> RequestPermissions(IList<HealthPermissionDto> healthPermissions, bool canRequestFullHistoryPermission = false, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new RequestPermissionResult());
    }

    public partial Task<TDto[]> GetHealthDataAsync<TDto>(HealthTimeRange timeRange, CancellationToken cancellationToken = default)
        where TDto : HealthMetricBase
    {
        return Task.FromResult(Array.Empty<TDto>());
    }

    public partial Task<bool> WriteHealthDataAsync<TDto>(TDto data, CancellationToken cancellationToken = default)
        where TDto : HealthMetricBase
    {
        // Windows platform does not support health data writing
        return Task.FromResult(false);
    }

    public partial Task<bool> StartWorkoutSessionAsync(ActivityType activityType, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public partial Task<WorkoutDto?> EndWorkoutSessionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkoutDto?>(null);
    }

    public partial bool IsWorkoutSessionActive()
    {
        return false;
    }
}
