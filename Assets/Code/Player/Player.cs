using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool _dead = false;

    public bool dead => _dead;

    public void Kill()
    {
        _dead = true;
    }

    private void Update()
    {
        if (transform.localPosition.z <= -1 || transform.localPosition.y <= -1)
            Kill();

        if (!_dead) return;
        DeathEffect.Instance.Death();
    }

}
