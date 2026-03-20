using UnityEngine;
using EMILtools.Systems;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    EnemyController.ActionMap>
{
    public class ActionMap : IActionMap
    {
    }

    protected void Awake()
    {
        InitializeFacade();
    }
}