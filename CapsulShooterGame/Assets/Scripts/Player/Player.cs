using UnityEngine;
using UnityEngine.Events;

public class Player : Perosnage
{

    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDamaged(float damage, GameObject damageSource)
    {
        base.OnDamaged(damage, damageSource);
    }

    protected override void OnDie()
    {
        EventManager.PlayerDeath(transform.position);
        base.OnDie();
    }

}
