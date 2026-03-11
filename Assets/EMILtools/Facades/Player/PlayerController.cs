using EMILtools.Systems;
using UnityEngine;
using Sirenix.OdinInspector;
using static PrimaryInputAuthority;
using static EMILtools.Systems.IInputSubordinate<PlayerController.PlayerInputMap,PrimaryInputAuthority.Subordinates>;
using EMILtools.Core;


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
    }

    public class PlayerInputMap : InputMap
    {
        public PersistentDelegate<bool, Vector2> Move = new();
    }

    public PlayerInputMap Input { get; set; }

    [field: SerializeField] [field: PropertyOrder(-1)]
    public SubordinateContext inputSubordinateContext { get; set; }

    public PlayerInputMap InjectInputMap() => new PlayerInputMap();

    public void InitSubordinate()
    {
        InitializeFacade();
    }

    public void OnAuthorityReceived()
    {
        Functionality.Bind();
    }

    public void OnAuthorityLost()
    {
        Functionality.Unbind();
    }
}