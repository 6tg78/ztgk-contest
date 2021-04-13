using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    public List<Missile> missilesTypes;

    private Missile missile;

    public void Fire(Transform missileSpawnPoint, int missileType, Vector3 direction, float speed, bool AoE = false)
    {
        Vector3 shootingDirection = direction.normalized;

        missile = Instantiate(missilesTypes[missileType]);
        missile.AoE = AoE;

        missile.transform.position = missileSpawnPoint.position;
        missile.transform.forward = shootingDirection;
        missile.Rb.velocity = shootingDirection.normalized * speed;
    }
}
