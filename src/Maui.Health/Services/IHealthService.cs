using Maui.Health.Enums;
using Maui.Health.Models;
using Maui.Health.Models.Metrics;

namespace Maui.Health.Services;

public interface IHealthService
{
    /// <summary>
    /// Get health data for a specific metric type within a time range
    /// </summary>
    /// <typeparam name="TDto">The type of health metric DTO to retrieve</typeparam>
    /// <param name="timeRange">The time range for data retrieval</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of health metric DTOs</returns>
    Task<TDto[]> GetHealthDataAsync<TDto>(HealthTimeRange timeRange, CancellationToken cancellationToken = default)
        where TDto : HealthMetricBase;

    /// <summary>
    /// Write health data to the health store
    /// </summary>
    /// <typeparam name="TDto">The type of health metric DTO to write</typeparam>
    /// <param name="data">The health data to write</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> WriteHealthDataAsync<TDto>(TDto data, CancellationToken cancellationToken = default)
        where TDto : HealthMetricBase;

    Task<RequestPermissionResult> RequestPermission(HealthPermissionDto healthPermission, bool canRequestFullHistoryPermission = false, CancellationToken cancellationToken = default);
    Task<RequestPermissionResult> RequestPermissions(IList<HealthPermissionDto> healthPermissions, bool canRequestFullHistoryPermission = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start a live workout session
    /// </summary>
    /// <param name="activityType">The type of workout activity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if session started successfully, false otherwise</returns>
    Task<bool> StartWorkoutSessionAsync(ActivityType activityType, CancellationToken cancellationToken = default);

    /// <summary>
    /// End the active workout session and save to health store
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The completed workout DTO if successful, null otherwise</returns>
    Task<WorkoutDto?> EndWorkoutSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if there is an active workout session
    /// </summary>
    /// <returns>True if a session is active, false otherwise</returns>
    bool IsWorkoutSessionActive();
}
