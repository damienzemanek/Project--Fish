using UnityEngine;

public interface IBoundsCheckMsgReceiver
{
    public virtual void OnEnterBounds(Collider collidedWith, BoundsChecker sender) { }
    public virtual void OnExitBounds(Collider collidedWith, BoundsChecker sender) { }
    public virtual void OnStayBounds(Collider collidedWith, BoundsChecker sender) { }
}