using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PhysicCanon : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private PhysicSimulate physicSimulate;
    [SerializeField]
    private float startForce;
    [SerializeField]
    private BulletType bulletType;

    [SerializeField]
    private GameObjectPoolReference locationIndictePrefab;

    [Header("Gizmos Simulate")]
    [SerializeField]
    private float deltaTime = 0.2f;
    [SerializeField]
    private float simulateCount = 50;
    [SerializeField]
    private LayerMask hitLayers;

    [Header("Editor Only")]
    [SerializeField]
    private bool drawDizmos;

    public void TriggerFire()
    {
        physicSimulate.SetPositionAndVelocity(transform.position, startForce * transform.forward);
        physicSimulate.Update(Time.fixedDeltaTime);
        var canonShell = bulletType.Pool.Get();
        canonShell.Shoot(physicSimulate);

        if (locationIndictePrefab)
        {
            StartCoroutine(C_ForecastHitPosition(canonShell));
        }
    }

    public void TriggerFireWithParameter(int parameter)
    { }

    IEnumerator C_ForecastHitPosition(BulletBehaviour canonShell, int maxTryTime=100)
    {
        var physicSimulate = this.physicSimulate.Clone();
        physicSimulate.SetPositionAndVelocity(transform.position, startForce * transform.forward);

        Vector3 lastPosition = physicSimulate.Position;
        bool isHit = false;
        RaycastHit hit = new RaycastHit();
        int loopCap = 0;

        for (int i = 0; i < maxTryTime; i++)
        {
            physicSimulate.Update(deltaTime);

            Vector3 newPosition = physicSimulate.Position;
            if (Physics.Linecast(lastPosition, newPosition, out RaycastHit hit2, hitLayers))
            {
                isHit = true;
                hit = hit2;
                break;
            }
            lastPosition = newPosition;

            if (++loopCap > 10)
            {
                loopCap = 0;
                yield return null;
            }
        }
        if (!isHit)
            yield break;

        GameObject indicator = locationIndictePrefab.Get();
        indicator.transform.SetPositionAndRotation(hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal, Vector3.up));

        if (canonShell is CanonShell)
            ((CanonShell)canonShell).OnCollide += delegate { locationIndictePrefab.Put(indicator); };
    }

    void CloseLocationIndicate(GameObject locationIndicate)
    {
        locationIndicate.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawDizmos)
            return;

        physicSimulate.SetPositionAndVelocity(transform.position, startForce * transform.forward);

        Gizmos.DrawSphere(physicSimulate.Position, 0.1f);
        for (int i = 0; i < simulateCount; i++)
        {
            physicSimulate.Update(deltaTime);
            Gizmos.DrawSphere(physicSimulate.Position, 0.1f);
        }
    }
}
