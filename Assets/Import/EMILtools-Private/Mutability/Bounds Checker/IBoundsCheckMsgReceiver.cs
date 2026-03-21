using UnityEngine;

public interface IBoundsCheckMsgReceiver<TColliderSpace>
    where TColliderSpace : Component
{
    public virtual void OnEnterBounds(TColliderSpace collidedWith, BoundsChecker sender) { }
    public virtual void OnExitBounds(TColliderSpace collidedWith, BoundsChecker sender) { }
    public virtual void OnStayBounds(TColliderSpace collidedWith, BoundsChecker sender) { }
}