using EMILtools.Extensions;
using EMILtools.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using static IA_Player;
using static PrimaryInputAuthority;
using static PlayerController;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        if(ia.Player.Move.IsPressed() && context.ReadValue<UnityEngine.Vector2>().x != 0)
            Input.Move.Publish((true, context.ReadValue<UnityEngine.Vector2>().x));
        else
            Input.Move.Publish((false, NumEX.ZeroF));
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed: Input.Jump.Publish(true); break;
            case InputActionPhase.Canceled: Input.Jump.Publish(false); break;
        }

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 pixelPos = Mouse.current.position.ReadValue();
#if UNITY_EDITOR
        pixelPos /= EditorGUIUtility.pixelsPerPoint;
#endif
        Input.Look.Publish(pixelPos);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed: Input.Attack.Publish(true); break;
            case InputActionPhase.Canceled: Input.Attack.Publish(false); break;
        }
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }
    

    public void OnPrevious(InputAction.CallbackContext context)
    {
        
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        
    }

    public void OnHook(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed: Input.Hook.Publish(true); break;
            case InputActionPhase.Canceled: Input.Hook.Publish(false); break;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        
    }
}