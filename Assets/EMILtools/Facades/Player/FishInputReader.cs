using EMILtools.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using static FishInputAuthority;
using static IA_Player;
using static PlayerController;


public class FishInputReader : 
    IPlayerActions,
    IInputReaderSubordinate<PlayerInputMap, Subordinates>
{

    IA_Player ia;
    
    public void Init()
    {
        if(ia == null) ia = new IA_Player();
        
        ia.Player.Disable();
        ia.Player.SetCallbacks(this);
        ia.Player.Enable();
    }

    public PlayerInputMap Input { get => subordinate.Input; }
    public IInputSubordinate<PlayerInputMap, Subordinates> subordinate { get; set; }
    public void OnAuthorityChange() { }


    public void OnMove(InputAction.CallbackContext context)
    {
        if(ia.Player.Move.IsPressed())
            Input.Move.Invoke(true, context.ReadValue<Vector2>());
        else
            Input.Move.Invoke(false, Vector2.zero);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }
}
