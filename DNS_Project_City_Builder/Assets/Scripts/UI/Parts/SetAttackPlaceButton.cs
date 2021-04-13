using System;
using UnityEngine;

public class SetAttackPlaceButton : MonoBehaviour
{
    public event Action OnSetAttackPlace;

    public void SetAttackPlace()
    {
        OnSetAttackPlace?.Invoke();
    }

}
