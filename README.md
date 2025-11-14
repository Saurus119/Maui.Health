[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Maui.Health
![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Maui.Health)
![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Maui.Health)

Abstraction around `Android Health Connect` and `iOS HealthKit` with unified DTO-based API
⚠️ Beware, this package is currently just as **Proof of concept**. There is a lot of work required for proper stability and ease of use.
[Issues](https://github.com/Kebechet/Maui.Health/issues) will contain future tasks that should be implemented.

Feel free to contribute ❤️

## Features

- **Read Health Data**: Use `GetHealthDataAsync<TDto>()` for type-safe health data retrieval
- **Write Health Data**: Use `WriteHealthDataAsync<TDto>()` to write health metrics to the platform store
- **Live Workout Sessions**: Start and end workout sessions with automatic duration tracking
- **Unified DTOs**: Platform-agnostic data transfer objects with common properties
- **Time Range Support**: Duration-based metrics implement `IHealthTimeRange` interface
- **Cross-Platform**: Full read/write support for Android Health Connect and iOS HealthKit
- **Permission Management**: Granular read/write permission handling

## Platform Support & Health Data Mapping

| Health Data Type | Read/Write | Android Health Connect | iOS HealthKit | Wrapper Implementation |
|-----------------|------------|------------------------|---------------|----------------------|
| **Steps** | ✅ Read/Write | StepsRecord | StepCount | ✅ [`StepsDto`](src/Maui.Health/Models/Metrics/StepsDto.cs) |
| **Weight** | ✅ Read/Write | WeightRecord | BodyMass | ✅ [`WeightDto`](src/Maui.Health/Models/Metrics/WeightDto.cs) |
| **Height** | ✅ Read/Write | HeightRecord | Height | ✅ [`HeightDto`](src/Maui.Health/Models/Metrics/HeightDto.cs) |
| **Heart Rate** | ✅ Read/Write | HeartRateRecord | HeartRate | ✅ [`HeartRateDto`](src/Maui.Health/Models/Metrics/HeartRateDto.cs) |
| **Active Calories** | ✅ Read/Write | ActiveCaloriesBurnedRecord | ActiveEnergyBurned | ✅ [`ActiveCaloriesBurnedDto`](src/Maui.Health/Models/Metrics/ActiveCaloriesBurnedDto.cs) |
| **Exercise Session** | ✅ Read/Write | ExerciseSessionRecord | Workout | ✅ [`WorkoutDto`](src/Maui.Health/Models/Metrics/WorkoutDto.cs) |
| **Blood Glucose** | 📖 Read-only | BloodGlucoseRecord | BloodGlucose | ❌ N/A |
| **Body Temperature** | 📖 Read-only | BodyTemperatureRecord | BodyTemperature | ❌ N/A |
| **Oxygen Saturation** | 📖 Read-only | OxygenSaturationRecord | OxygenSaturation | ❌ N/A |
| **Respiratory Rate** | 📖 Read-only | RespiratoryRateRecord | RespiratoryRate | ❌ N/A |
| **Basal Metabolic Rate** | 📖 Read-only | BasalMetabolicRateRecord | BasalEnergyBurned | ❌ N/A |
| **Body Fat** | 📖 Read-only | BodyFatRecord | BodyFatPercentage | 🚧 WIP (commented out) |
| **Lean Body Mass** | 📖 Read-only | LeanBodyMassRecord | LeanBodyMass | ❌ N/A |
| **Hydration** | 📖 Read-only | HydrationRecord | DietaryWater | ❌ N/A |
| **VO2 Max** | 📖 Read-only | Vo2MaxRecord | VO2Max | 🚧 WIP (commented out) |
| **Resting Heart Rate** | 📖 Read-only | RestingHeartRateRecord | RestingHeartRate | ❌ N/A |
| **Heart Rate Variability** | 📖 Read-only | HeartRateVariabilityRmssdRecord | HeartRateVariabilitySdnn | ❌ N/A |
| **Blood Pressure** | 📖 Read-only | BloodPressureRecord | Split into Systolic/Diastolic | 🚧 WIP (commented out) |

## Usage

### 1. Registration
Register the health service in your `MauiProgram.cs`:
```csharp
builder.Services.AddHealth();
```

### 2. Platform Setup
Follow the [platform setup guide](https://github.com/Kebechet/Maui.Health/commit/139e69fade83f9133044910e47ad530f040b8021):

**Android:**
- Google Play console Health permissions declaration
- Privacy policy requirements (mandatory for Health Connect)
- AndroidManifest.xml permissions (see below)
- Minimum Android API 26 (Android 8.0)

**Required Android Permissions:**
```xml
<!-- Health Connect Permissions -->
<uses-permission android:name="android.permission.health.READ_EXERCISE"/>
<uses-permission android:name="android.permission.health.WRITE_EXERCISE"/>
<uses-permission android:name="android.permission.health.READ_STEPS"/>
<uses-permission android:name="android.permission.health.WRITE_STEPS"/>
<uses-permission android:name="android.permission.health.READ_WEIGHT"/>
<uses-permission android:name="android.permission.health.WRITE_WEIGHT"/>
<!-- Activity Recognition for workout tracking -->
<uses-permission android:name="android.permission.ACTIVITY_RECOGNITION" />
```

**iOS:**
- Provisioning profile with HealthKit capability enabled
- Entitlements.plist with HealthKit entitlement
- Info.plist privacy descriptions:
  - `NSHealthShareUsageDescription` - for reading health data
  - `NSHealthUpdateUsageDescription` - for writing health data

### 3. Reading Health Data

```csharp
public class HealthExampleService
{
    private readonly IHealthService _healthService;

    public HealthExampleService(IHealthService healthService)
    {
        _healthService = healthService;
    }

    public async Task<List<StepsDto>> GetTodaysStepsAsync()
    {
        var timeRange = HealthTimeRange.FromDateTime(DateTime.Today, DateTime.Now);

        var steps = await _healthService.GetHealthDataAsync<StepsDto>(timeRange);
        return steps.ToList();
    }

    public async Task<List<WeightDto>> GetRecentWeightAsync()
    {
        var timeRange = HealthTimeRange.FromDateTime(DateTime.Now.AddDays(-7), DateTime.Now);

        var weights = await _healthService.GetHealthDataAsync<WeightDto>(timeRange);
        return weights.ToList();
    }
}
```

### 4. Writing Health Data

```csharp
public class HealthWriteService
{
    private readonly IHealthService _healthService;

    public HealthWriteService(IHealthService healthService)
    {
        _healthService = healthService;
    }

    // Write steps data
    public async Task<bool> WriteStepsAsync(int stepCount, DateTime startTime, DateTime endTime)
    {
        var stepsDto = new StepsDto
        {
            Id = "",
            DataOrigin = "MyApp",
            Timestamp = startTime,
            Count = stepCount,
            StartTime = startTime,
            EndTime = endTime
        };

        return await _healthService.WriteHealthDataAsync(stepsDto);
    }

    // Write workout/exercise session
    public async Task<bool> WriteWorkoutAsync(ActivityType activityType, DateTime startTime, DateTime endTime, double? caloriesBurned = null)
    {
        var workoutDto = new WorkoutDto
        {
            Id = "",
            DataOrigin = "MyApp",
            Timestamp = startTime,
            ActivityType = activityType,
            Title = activityType.ToString(),
            StartTime = startTime,
            EndTime = endTime,
            EnergyBurned = caloriesBurned
        };

        return await _healthService.WriteHealthDataAsync(workoutDto);
    }
}
```

### 5. Live Workout Session Tracking

Track workouts in real-time with automatic duration calculation:

```csharp
public class WorkoutSessionService
{
    private readonly IHealthService _healthService;

    public WorkoutSessionService(IHealthService healthService)
    {
        _healthService = healthService;
    }

    // Start a workout session
    public async Task<bool> StartWorkoutAsync(ActivityType activityType)
    {
        // Request write permission first
        var permission = new HealthPermissionDto
        {
            HealthDataType = HealthDataType.ExerciseSession,
            PermissionType = PermissionType.Write
        };
        await _healthService.RequestPermissions([permission]);

        // Start the session - platform will track start time
        return await _healthService.StartWorkoutSessionAsync(activityType);
    }

    // End the workout session and save to health store
    public async Task<WorkoutDto?> EndWorkoutAsync()
    {
        // This will calculate duration and save to the platform health store
        var completedWorkout = await _healthService.EndWorkoutSessionAsync();

        if (completedWorkout != null)
        {
            Console.WriteLine($"Workout completed! Duration: {completedWorkout.Duration.TotalMinutes:F1} minutes");
            Console.WriteLine($"Activity: {completedWorkout.ActivityType}");
        }

        return completedWorkout;
    }

    // Check if a session is currently active
    public bool IsSessionActive()
    {
        return _healthService.IsWorkoutSessionActive();
    }
}
```

**Supported Activity Types:**
- `TraditionalStrengthTraining`, `FunctionalStrengthTraining`
- `Running`, `Cycling`, `Walking`, `Swimming`, `Hiking`
- `Yoga`, `Pilates`, `Elliptical`, `Rowing`
- `Soccer`, `Basketball`, `Tennis`, `Golf`, and 20+ more

**Platform Behavior:**
- **iOS**: Workouts are saved to Apple Health and contribute to Activity rings
- **Android**: Workouts are saved to Google Health Connect and tracked in fitness apps

### 6. Working with Time Ranges

Duration-based metrics implement `IHealthTimeRange`:

```csharp
public async Task AnalyzeStepsData()
{
    var timeRange = HealthTimeRange.FromDateTime(DateTime.Today, DateTime.Now);
    var steps = await _healthService.GetHealthDataAsync<StepsDto>(timeRange);
    
    foreach (var stepRecord in steps)
    {
        // Common properties from BaseHealthMetricDto
        Console.WriteLine($"ID: {stepRecord.Id}");
        Console.WriteLine($"Source: {stepRecord.DataOrigin}");
        Console.WriteLine($"Recorded: {stepRecord.Timestamp}");
        
        // Steps-specific data
        Console.WriteLine($"Steps: {stepRecord.Count}");
        
        // Time range data (IHealthTimeRange)
        Console.WriteLine($"Period: {stepRecord.StartTime} to {stepRecord.EndTime}");
        Console.WriteLine($"Duration: {stepRecord.Duration}");
        
        // Type-safe duration checking
        if (stepRecord is IHealthTimeRange timeRange)
        {
            Console.WriteLine($"This measurement lasted {timeRange.Duration.TotalMinutes} minutes");
        }
    }
}

public async Task AnalyzeWeightData()
{
    var timeRange = HealthTimeRange.FromDateTime(DateTime.Today.AddDays(-30), DateTime.Now);
    var weights = await _healthService.GetHealthDataAsync<WeightDto>(timeRange);
    
    foreach (var weightRecord in weights)
    {
        // Instant measurements only have Timestamp
        Console.WriteLine($"Weight: {weightRecord.Value} {weightRecord.Unit}");
        Console.WriteLine($"Measured at: {weightRecord.Timestamp}");
        Console.WriteLine($"Source: {weightRecord.DataOrigin}");
    }
}
```

### 7. Permission Handling

```csharp
public async Task RequestPermissions()
{
    var permissions = new List<HealthPermissionDto>
    {
        new() { HealthDataType = HealthDataType.Steps, PermissionType = PermissionType.Read },
        new() { HealthDataType = HealthDataType.Weight, PermissionType = PermissionType.Read | PermissionType.Write },
        new() { HealthDataType = HealthDataType.ExerciseSession, PermissionType = PermissionType.Write }
    };

    var result = await _healthService.RequestPermissions(permissions);

    if (result.IsSuccess)
    {
        Console.WriteLine("Permissions granted!");
    }
    else
    {
        Console.WriteLine($"Permission error: {result.Error}");
    }
}
```

**Permission Types:**
- `PermissionType.Read` - Read health data
- `PermissionType.Write` - Write health data

## DTO Architecture

### Base Classes and Interfaces

All health metric DTOs inherit from [`BaseHealthMetricDto`](src/Maui.Health/Models/Metrics/BaseHealthMetricDto.cs):

```csharp
public abstract class BaseHealthMetricDto
{
    public required string Id { get; init; }
    public required string DataOrigin { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public string? RecordingMethod { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}
```

Duration-based metrics also implement [`IHealthTimeRange`](src/Maui.Health/Models/Metrics/IHealthTimeRange.cs):

```csharp
public interface IHealthTimeRange
{
    DateTimeOffset StartTime { get; }
    DateTimeOffset EndTime { get; }
    TimeSpan Duration => EndTime - StartTime;
}
```

### Metric Categories

**Duration-Based Metrics** (implement `IHealthTimeRange`):
- Steps - counted over time periods
- Exercise sessions - have start/end times
- Sleep sessions - duration-based

**Instant Metrics** (timestamp only):
- Weight - measured at specific moment
- Height - measured at specific moment  
- Blood pressure - instant reading
- Heart rate - point-in-time measurement

## API Reference

### IHealthService Methods

**Reading Health Data:**
```csharp
Task<TDto[]> GetHealthDataAsync<TDto>(HealthTimeRange timeRange, CancellationToken cancellationToken = default)
    where TDto : HealthMetricBase;
```

**Writing Health Data:**
```csharp
Task<bool> WriteHealthDataAsync<TDto>(TDto data, CancellationToken cancellationToken = default)
    where TDto : HealthMetricBase;
```

**Workout Session Management:**
```csharp
Task<bool> StartWorkoutSessionAsync(ActivityType activityType, CancellationToken cancellationToken = default);
Task<WorkoutDto?> EndWorkoutSessionAsync(CancellationToken cancellationToken = default);
bool IsWorkoutSessionActive();
```

**Permission Management:**
```csharp
Task<RequestPermissionResult> RequestPermission(HealthPermissionDto healthPermission, ...);
Task<RequestPermissionResult> RequestPermissions(IList<HealthPermissionDto> healthPermissions, ...);
```

## What's New

### ✨ Version 1.0 Features

**Write Support (Both Platforms):**
- Full write functionality for iOS HealthKit and Android Health Connect
- Write steps, weight, height, heart rate, calories, and workouts
- Type-safe write API using the same DTOs as reading

**Live Workout Session Tracking:**
- Start/end workout sessions with automatic duration tracking
- No need to manually calculate workout duration
- Session state management with `IsWorkoutSessionActive()`
- Works on both iOS and Android
- Saved workouts appear in native health apps (Apple Health / Health Connect)

**Enhanced Permission System:**
- Separate read/write permission handling
- Bitwise permission flags (`PermissionType.Read | PermissionType.Write`)
- Platform-specific permission prompts

**30+ Activity Types Supported:**
- Strength training (traditional and functional)
- Cardio activities (running, cycling, swimming, rowing)
- Sports (soccer, basketball, tennis, golf, etc.)
- Other fitness activities (yoga, pilates, hiking, etc.)

## Testing Tips

**iOS Simulator/Device:**
- If no health data exists, open the Health app
- Navigate to the desired metric (e.g., Steps)
- Tap "Add Data" in the top-right corner
- Manually add test data for development
- For write operations, ensure "MyApp" appears in Health data sources

**Android Emulator/Device:**
- Install Google Health Connect app from Play Store
- Add sample health data for testing
- Ensure proper permissions are granted in Health Connect
- For write operations, verify data appears in Health Connect app

## Credits
- @aritchie - `https://github.com/shinyorg/Health`
- @0xc3u - `https://github.com/0xc3u/Plugin.Maui.Health`
- @EagleDelux - `https://github.com/EagleDelux/androidx.health-connect-demo-.net-maui`
- @b099l3 - `https://github.com/b099l3/ios-samples/tree/65a4ab1606cfd8beb518731075e4af526c4da4ad/ios8/Fit/Fit`

## Other Sources
- https://pub.dev/packages/health
- [Android Health Connect Documentation](https://developer.android.com/health-and-fitness/guides/health-connect)
- [iOS HealthKit Documentation](https://developer.apple.com/documentation/healthkit)
