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
        // Add Available Actions here as PersistentActions
        // Actions are separate from InputActions
    }

    // If using InputAuthority, put InitializeFacade in InitSubordinate
    protected void Awake()
    {
        InitializeFacade();
    }
}