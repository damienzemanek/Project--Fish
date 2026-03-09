using EMILtools.Core;
using UnityEngine;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using static EMILtools.Systems.IInputSubordinate<PlayerController.PlayerInputMap,FishInputAuthority.Subordinates>;
using static FishInputAuthority;

public class PlayerController : MonoFacade<
    PlayerController,
    PlayerFunctionality,
    PlayerConfig,
    PlayerStructure,
    PlayerController.ActionMap>,
    IInputSubordinate<PlayerController.PlayerInputMap, Subordinates>
{
    public class ActionMap : IActionMap
    {
        // Add Available Actions here as PersistentActions
        // Actions are separate from InputActions
    }

    public class PlayerInputMap : InputMap
    {
        public PersistentAction<bool, Vector2> Move = new();

    }

    public PlayerInputMap Input { get; set; }
    [field: SerializeField] [field: PropertyOrder(-1)] public SubordinateContext inputSubordinateContext { get; set; }

    public PlayerInputMap InjectInputMap() => new PlayerInputMap();

    public void InitSubordinate()
    {
        InitializeFacade();
    }

    public void OnAuthorityReceived()
    {
        // no op atm
    }

    public void OnAuthorityLost()
    {
        // no op atm
    }
}