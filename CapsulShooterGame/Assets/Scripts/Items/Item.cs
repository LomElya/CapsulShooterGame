using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Основное:")]
    [SerializeField] protected GameObject _root;
    [SerializeField] private AssetItem _asset;
    [SerializeField] protected Transform _muzzle;

    [Space(5)]
    [Header("Звук и визуал")]
    [SerializeField] protected AudioSource _useAudioSource;
    [SerializeField] protected AudioClip _useSound;
    [SerializeField] protected AudioClip _changeItemSound;

    [Space(15)]
    [SerializeField] protected bool _unparentMuzzleFlash;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected GameObject _muzzleFlashPrefab;

    public GameObject Owner { get; private set; }
    public GameObject SourcePrefab { get; private set; }

    protected Vector3 _lastMuzzlePosition { get; set; }
    protected bool _wantsToUse = false;

    public GameObject Root => _root;
    public AssetItem Asset => _asset;
    public Transform Muzzle => _muzzle;

    protected float _lastTimeUse = Mathf.NegativeInfinity;
    protected abstract string _animUseParameter { get; }

    public void ShowItem(bool show)
    {
        _root.SetActive(show);

        /// Звук смены предмета
        if (show && _changeItemSound)
        {
            _useAudioSource?.PlayOneShot(_changeItemSound);
        }

        _wantsToUse = show;
    }

    public virtual bool HandleUseInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        return false;
    }

    public virtual bool TryUse()
    {
        HandleUse();
        return true;
    }

    public virtual void HandleUse()
    {
        /// Активировать звук использования, если она есть
        _useAudioSource?.PlayOneShot(_useSound);

        /// Активировать анимацию использования, если она есть
        _animator?.SetTrigger(_animUseParameter);
    }

    public virtual void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void Add(GameObject owner, Item item, Transform itemParent)
    {
        Owner = owner.gameObject;
        SourcePrefab = item.gameObject;

        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;

        ShowItem(false);
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner.gameObject;
    }
}
