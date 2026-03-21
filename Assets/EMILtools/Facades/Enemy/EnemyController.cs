using System;
using UnityEngine;
using EMILtools.Systems;
using static EMILtools.Timers.TimerUtility;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    EnemyController.ActionMap>, ITimerUser
{
    public class ActionMap : IActionMap
    {
        public readonly Publisher Idle = new();
    }

    protected void Awake()
    {
        InitializeFacade();
    }

    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();
}