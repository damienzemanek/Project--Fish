using System;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskEX
{
    public static async void Forget(this Task task, string tag)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Debug.LogException(new Exception($"[FireAndForget:{tag}]", ex));
        }
    }
    
    public static async Task<bool> ForgetBool(this Task task, string tag)
    {
        try
        {
            await task;
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(new Exception($"[FireAndForget:{tag}]", ex));
            return false;
        }
    }
}