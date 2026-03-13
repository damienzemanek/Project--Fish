using EMILtools.Systems;
using UnityEngine.InputSystem;
using static IA_Player;
using static PrimaryInputAuthority;
using static PlayerController;

public class PlayerInputReader :
    IPlayerActions,
    IInputReaderSubordinate<PlayerInputMap, Subordinates>
{
    public IA_Player ia;

    public void Init()
    {
        if(ia == null) ia = new IA_Player();
        ia.Player.Disable();
        ia.Player.SetCallbacks(this);
        ia.Player.Enable();
    }

    public PlayerInputMap Input => subordinate.Input;
    public IInputSubordinate<PlayerInputMap, Subordinates> subordinate { get; set; }
    public void OnAuthorityChange() { }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(ia.Player.Move.IsPressed())
            Input.Move.Publish(true, context.ReadValue<UnityEngine.Vector2>());
        else
            Input.Move.Publish(false, UnityEngine.Vector2.zero);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        
    }
}