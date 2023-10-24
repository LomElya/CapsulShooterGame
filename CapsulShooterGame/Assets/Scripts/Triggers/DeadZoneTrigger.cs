using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneTrigger : Trigger<Health>
{
    protected override void OnEnter(Health triggered)
    {
        triggered.Kill();
    }
}
