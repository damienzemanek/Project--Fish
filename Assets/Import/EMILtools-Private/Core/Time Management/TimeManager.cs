using System.Threading;
using System.Threading.Tasks;
using DesignPatterns.CreationalPatterns;
using EMILtools.Design_Patterns.Creational_Patterns.CreationalPatterns;
using EMILtools.Systems;
using UnityEngine;

public class TimeManager : PersistantSingleton<TimeManager>
{
    CancellationTokenSource cts;

    public async void SlowTimeForSeconds(float seconds, float percentage)
    {
        percentage = Mathf.Clamp01(percentage);

        // Cancel previous
        cts?.Cancel();
        cts = new CancellationTokenSource();
        var token = cts.Token;

        float originalTimeScale = Time.timeScale;
        float originalFixedDelta = Time.fixedDeltaTime;

        Time.timeScale = percentage;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        try
        {
            await WaitForSecondsRealtimeAsync(seconds, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (!token.IsCancellationRequested)
        {
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDelta;
        }
    }

    async Task WaitForSecondsRealtimeAsync(float seconds, CancellationToken token)
    {
        float endTime = Time.realtimeSinceStartup + seconds;

        while (Time.realtimeSinceStartup < endTime)
        {
            if (token.IsCancellationRequested)
                return;

            await Task.Yield();
        }
    }
}
