using UnityEngine;
using UnityEngine.Events;

public abstract class UsableItemEffect : ScriptableObject
{
    [SerializeField] protected UnityEvent<GameObject> onUse;

    public void Use(GameObject user)
    {
        onUse.Invoke(user);
    }
}
