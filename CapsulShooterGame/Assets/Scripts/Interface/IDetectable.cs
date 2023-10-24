using UnityEngine;
using UnityEngine.Events;

public interface IDetectable
{
    public DetectionModule DetectionModule { get; }
    public GameObject KnownDetectedTarget { get; }
    public bool IsTargetInAttackRange { get; }
    public bool IsSeeingTarget { get; }
    public bool HadKnownTarget { get; }

    public UnityAction onDetectedTarget { get;  }
    public UnityAction onLostTarget { get;  }

    public void OnLostTarget();
    public void OnDetectedTarget();
}