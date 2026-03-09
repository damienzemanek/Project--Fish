using UnityEngine;
using EMILtools.Systems;

public class PlayerController : MonoFacade<
    PlayerController,
    PlayerFunctionality,
    PlayerConfig,
    PlayerStructure,
    PlayerController.ActionMap>
{
    public class ActionMap : IActionMap
    {
        // PersistentActions
    }

    // If using InputAuthority, put InitializeFacade in InitSubordinate
    protected void Awake()
    {
        InitializeFacade();
    }
}