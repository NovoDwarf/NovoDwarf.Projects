#if IOS

using Foundation;
using HealthKit;
using SmartHome.Core;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.Platforms.iOS;

/// <summary>
/// Reads health metrics from Apple HealthKit.
/// Requires NSHealthShareUsageDescription in Info.plist and the HealthKit entitlement.
/// </summary>
public sealed class HealthKitProvider : IHealthDataProvider
{
    private readonly HKHealthStore _store = new();

    public bool IsAvailable => HKHealthStore.IsHealthDataAvailable;

    public async Task<bool> RequestPermissionsAsync()
    {
        if (!IsAvailable) return false;

        var readTypes = new HKObjectType[]
        {
            HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount)!,
            HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate)!,
            HKQuantityType.Create(HKQuantityTypeIdentifier.RestingHeartRate)!,
            HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned)!,
            HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass)!,
            HKQuantityType.Create(HKQuantityTypeIdentifier.OxygenSaturation)!,
            HKCategoryType.Create(HKCategoryTypeIdentifier.SleepAnalysis)!,
        };

        var (success, _) = await _store.RequestAuthorizationToShareAsync(
            typesToShare: new NSSet<HKSampleType>(),
            typesToRead: NSSet.MakeNSObjectSet(readTypes));

        return success;
    }

    public async Task<HealthMetrics> ReadTodayAsync()
    {
        var startOfDay = NSCalendar.CurrentCalendar.StartOfDayForDate(NSDate.Now);
        var now = NSDate.Now;

        var steps      = await ReadSumAsync(HKQuantityTypeIdentifier.StepCount,          HKUnit.Count,                  startOfDay, now);
        var heartRate  = await ReadLatestAsync(HKQuantityTypeIdentifier.HeartRate,        HKUnit.FromString("count/min"), startOfDay, now);
        var restingHr  = await ReadLatestAsync(HKQuantityTypeIdentifier.RestingHeartRate, HKUnit.FromString("count/min"), startOfDay, now);
        var calories   = await ReadSumAsync(HKQuantityTypeIdentifier.ActiveEnergyBurned,  HKUnit.Kilocalorie,            startOfDay, now);
        var weight     = await ReadLatestAsync(HKQuantityTypeIdentifier.BodyMass,         HKUnit.Gram,
                             (NSDate)DateTime.UtcNow.AddDays(-30), now);   // last 30 days
        var spo2Raw    = await ReadLatestAsync(HKQuantityTypeIdentifier.OxygenSaturation, HKUnit.Percent,                startOfDay, now);
        var sleepHours = await ReadSleepHoursAsync(startOfDay, now);

        return new HealthMetrics
        {
            Steps            = steps.HasValue ? (int)steps.Value : null,
            HeartRate        = heartRate,
            RestingHeartRate = restingHr,
            ActiveCalories   = calories,
            Weight           = weight,
            SpO2             = spo2Raw.HasValue ? spo2Raw.Value * 100.0 : null,  // HealthKit хранит 0–1
            SleepHours       = sleepHours,
        };
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    private Task<double?> ReadSumAsync(
        HKQuantityTypeIdentifier id, HKUnit unit, NSDate from, NSDate to)
    {
        var tcs = new TaskCompletionSource<double?>();
        var quantityType = HKQuantityType.Create(id)!;
        var predicate = HKQuery.GetPredicateForSamples(from, to, HKQueryOptions.StrictStartDate);

        var query = new HKStatisticsQuery(
            quantityType, predicate,
            HKStatisticsOptions.CumulativeSum,
            (_, result, error) =>
            {
                if (error is not null || result?.SumQuantity() is not { } qty)
                    tcs.SetResult(null);
                else
                    tcs.SetResult(qty.GetDoubleValue(unit));
            });

        _store.ExecuteQuery(query);
        return tcs.Task;
    }

    private Task<double?> ReadLatestAsync(
        HKQuantityTypeIdentifier id, HKUnit unit, NSDate from, NSDate to)
    {
        var tcs = new TaskCompletionSource<double?>();
        var quantityType = HKQuantityType.Create(id)!;
        var predicate = HKQuery.GetPredicateForSamples(from, to, HKQueryOptions.StrictStartDate);
        var sortDesc = new[] { NSSortDescriptor.FromKey(HKSample.SortIdentifierEndDate, false) };

        var query = new HKSampleQuery(
            quantityType, predicate, 1, sortDesc,
            (_, results, error) =>
            {
                if (error is not null || results is not [HKQuantitySample sample, ..])
                    tcs.SetResult(null);
                else
                    tcs.SetResult(sample.Quantity.GetDoubleValue(unit));
            });

        _store.ExecuteQuery(query);
        return tcs.Task;
    }

    private Task<double?> ReadSleepHoursAsync(NSDate from, NSDate to)
    {
        var tcs = new TaskCompletionSource<double?>();
        var sleepType = HKCategoryType.Create(HKCategoryTypeIdentifier.SleepAnalysis)!;
        var predicate = HKQuery.GetPredicateForSamples(from, to, HKQueryOptions.StrictStartDate);

        var query = new HKSampleQuery(
            sleepType, predicate, HKSampleQuery.NoLimit, null,
            (_, results, error) =>
            {
                if (error is not null || results is null)
                {
                    tcs.SetResult(null);
                    return;
                }

                // Суммируем только "asleep" интервалы (value == Asleep / AsleepCore / AsleepDeep / AsleepREM)
                var asleepSeconds = results
                    .OfType<HKCategorySample>()
                    .Where(s => s.Value == (nint)HKCategoryValueSleepAnalysis.Asleep)
                    .Sum(s => s.EndDate.SecondsSince1970 - s.StartDate.SecondsSince1970);

                tcs.SetResult(asleepSeconds > 0 ? asleepSeconds / 3600.0 : null);
            });

        _store.ExecuteQuery(query);
        return tcs.Task;
    }
}

#endif
