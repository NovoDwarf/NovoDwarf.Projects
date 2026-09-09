#if ANDROID

using Android.Content;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Permission;
using AndroidX.Health.Connect.Client.Records;
using AndroidX.Health.Connect.Client.Records.Metadata;
using AndroidX.Health.Connect.Client.Request;
using AndroidX.Health.Connect.Client.Response;
using AndroidX.Health.Connect.Client.Time;
using Kotlin.Coroutines;
using Kotlin.Jvm;
using SmartHome.Core;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.Platforms.Android;

/// <summary>
/// Reads health metrics from Android Health Connect.
/// Raw Kotlin suspend functions are bridged to Task via IContinuation.
/// </summary>
public sealed class HealthConnectProvider : IHealthDataProvider
{
    private IHealthConnectClient? _client;
    
    /// <summary>
    /// Bridges a Kotlin suspend function (Object? Method(Request, IContinuation))
    /// to a .NET Task.
    /// </summary>
    private static Task<T?> SuspendAsync<T>(
        Func<IContinuation, Java.Lang.Object?> call) where T : class
    {
        var tcs  = new TaskCompletionSource<T?>();
        var cont = new SuspendContinuation<T>(tcs);
        try
        {
            var syncResult = call(cont);
            
            if (syncResult is T direct)
                tcs.TrySetResult(direct);
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        
        return tcs.Task;
    }

    private static Kotlin.Reflect.IKClass KClass<T>() => JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(typeof(T)));

    private static readonly DataOrigin[] NoOrigins = [];
    
    public bool IsAvailable =>
        HealthConnectClient.GetSdkStatus(Platform.AppContext) == HealthConnectClient.SdkAvailable;

    public async Task<bool> RequestPermissionsAsync()
    {
        if (!IsAvailable)
            return false;
        
        _client = HealthConnectClient.GetOrCreate(Platform.AppContext);

        var needed = new HashSet<string>
        {
            HealthPermission.GetReadPermission(KClass<StepsRecord>()),
            HealthPermission.GetReadPermission(KClass<HeartRateRecord>()),
            HealthPermission.GetReadPermission(KClass<RestingHeartRateRecord>()),
            HealthPermission.GetReadPermission(KClass<ActiveCaloriesBurnedRecord>()),
            HealthPermission.GetReadPermission(KClass<WeightRecord>()),
            HealthPermission.GetReadPermission(KClass<OxygenSaturationRecord>()),
            HealthPermission.GetReadPermission(KClass<SleepSessionRecord>())
        };

        var granted = await GetGrantedAsync();
        
        if (needed.IsSubsetOf(granted)) 
            return true;

        Platform.CurrentActivity!.StartActivity(new Intent("androidx.health.ACTION_HEALTH_CONNECT_SETTINGS"));
        
        await Task.Delay(500);

        var grantedAfter = await GetGrantedAsync();
        return needed.IsSubsetOf(grantedAfter);
    }

    public async Task<HealthMetrics> ReadTodayAsync()
    {
        _client ??= HealthConnectClient.GetOrCreate(Platform.AppContext);

        var todayMs = new DateTimeOffset(DateTime.Today, TimeZoneInfo.Local.GetUtcOffset(DateTime.Today)).ToUnixTimeMilliseconds();
        var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var ago30Ms = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeMilliseconds();

        var startOfDay = Java.Time.Instant.OfEpochMilli(todayMs);
        var now = Java.Time.Instant.OfEpochMilli(nowMs);
        var ago30Days = Java.Time.Instant.OfEpochMilli(ago30Ms);

        var today     = TimeRangeFilter.Between(startOfDay, now);
        var last30d   = TimeRangeFilter.Between(ago30Days, now);

        var steps = await ReadStepsAsync(today);
        var hr = await ReadLatestAsync<HeartRateRecord>(today, r => r.Cast<HeartRateRecord>().FirstOrDefault()?.Samples?.LastOrDefault()?.BeatsPerMinute);
        var rhr = await ReadLatestAsync<RestingHeartRateRecord>(today, r => r.Cast<RestingHeartRateRecord>().FirstOrDefault()?.BeatsPerMinute);
        var calories = await ReadSumAsync<ActiveCaloriesBurnedRecord>(today, r => r.Cast<ActiveCaloriesBurnedRecord>().Sum(x => x.Energy.Kilocalories));
        var weight = await ReadLatestAsync<WeightRecord>(last30d, r => r.Cast<WeightRecord>().FirstOrDefault()?.Weight.Kilograms);
        var spo2 = await ReadLatestAsync<OxygenSaturationRecord>(today, r => r.Cast<OxygenSaturationRecord>().FirstOrDefault()?.Percentage.Value);
        var sleep = await ReadSleepAsync(today);

        return new HealthMetrics
        {
            Steps            = steps,
            HeartRate        = hr,
            RestingHeartRate = rhr,
            ActiveCalories   = calories > 0 ? calories : null,
            Weight           = weight,
            SpO2             = spo2,
            SleepHours       = sleep,
        };
    }


    private async Task<HashSet<string>> GetGrantedAsync()
    {
        var raw = await SuspendAsync<Java.Lang.Object>(c => _client!.PermissionController.GetGrantedPermissions(c));
        
        if (raw is not Java.Util.ICollection jCol) 
            return [];
        
        var result = new HashSet<string>();
        var it = jCol.Iterator();
        
        while (it.HasNext)
            if (it.Next()?.ToString() is { } s) result.Add(s);
        
        return result;
    }

    private async Task<int?> ReadStepsAsync(TimeRangeFilter range)
    {
        var resp = await ReadRecordsAsync<StepsRecord>(range, true, 1000);
        var total = resp?.OfType<StepsRecord>().Sum(r => r.Count) ?? 0;
        
        return total > 0 ? (int)total : null;
    }

    private async Task<double?> ReadLatestAsync<T>(TimeRangeFilter range, Func<IEnumerable<Java.Lang.Object>, double?> selector) where T : Java.Lang.Object
    {
        var resp = await ReadRecordsAsync<T>(range, false, 1);
        return resp is null ? null : selector(resp);
    }

    private async Task<double> ReadSumAsync<T>(
        TimeRangeFilter range,
        Func<IEnumerable<Java.Lang.Object>, double> selector) where T : Java.Lang.Object
    {
        var resp = await ReadRecordsAsync<T>(range, true, 1000);
        return resp is null ? 0.0 : selector(resp);
    }

    private async Task<double?> ReadSleepAsync(TimeRangeFilter range)
    {
        var resp = await ReadRecordsAsync<SleepSessionRecord>(range, true, 1000);
        var totalMin = resp?.OfType<SleepSessionRecord>()
                           .Sum(r => (r.EndTime.ToEpochMilli() - r.StartTime.ToEpochMilli()) / 60_000.0);
        
        return totalMin > 0 ? totalMin / 60.0 : null;
    }
    
    private async Task<IEnumerable<Java.Lang.Object>?> ReadRecordsAsync<T>(
        TimeRangeFilter range, bool ascending, int pageSize) where T : Java.Lang.Object
    {
        var request = new ReadRecordsRequest(
            KClass<T>(), range, NoOrigins, ascending, pageSize, null);
        var response = await SuspendAsync<ReadRecordsResponse>(
            c => _client!.ReadRecords(request, c));
        return response?.Records.Cast<Java.Lang.Object>();
    }
    
    private sealed class SuspendContinuation<T> : Java.Lang.Object, IContinuation
        where T : class
    {
        private readonly TaskCompletionSource<T?> _tcs;

        public SuspendContinuation(TaskCompletionSource<T?> tcs) => _tcs = tcs;

        public ICoroutineContext Context => EmptyCoroutineContext.Instance;

        public void ResumeWith(Java.Lang.Object? result)
        {
            if (result?.Class.Name.EndsWith("$Failure", StringComparison.Ordinal) == true)
            {
                _tcs.TrySetException(new Exception($"Health Connect error: {result}"));
            }
            else
            {
                _tcs.TrySetResult(result as T);
            }
        }
    }
}

#endif
