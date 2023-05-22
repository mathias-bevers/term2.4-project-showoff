using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public void OnControllerColliderHit(ControllerColliderHit collider)
    {
        Debug.Log("HIT!");
        Player p;
        if ((p = collider.gameObject.GetComponent<Player>()) == null) return;
        p.Kill();

    }
}
