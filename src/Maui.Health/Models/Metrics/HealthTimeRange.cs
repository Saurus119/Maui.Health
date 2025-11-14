namespace Maui.Health.Models.Metrics;

/// <summary>
/// Represents a time range for querying health data
/// </summary>
public record HealthTimeRange(DateTimeOffset StartTime, DateTimeOffset EndTime) : IHealthTimeRange
{
    /// <summary>
    /// Duration of the time range
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Gets the start time as a local DateTime for platform APIs
    /// </summary>
    public DateTime StartDateTime => StartTime.LocalDateTime;

    /// <summary>
    /// Gets the end time as a local DateTime for platform APIs
    /// </summary>
    public DateTime EndDateTime => EndTime.LocalDateTime;

    /// <summary>
    /// Creates a time range from DateTime values, preserving local time context
    /// </summary>
    public static HealthTimeRange FromDateTime(DateTime start, DateTime end)
    {
        // Ensure DateTime is treated as local time if unspecified
        var startKind = start.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(start, DateTimeKind.Local) : start;
        var endKind = end.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(end, DateTimeKind.Local) : end;

        return new(new DateTimeOffset(startKind), new DateTimeOffset(endKind));
    }
}
