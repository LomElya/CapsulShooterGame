using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class ProjectileLaser : ProjectileBase
{
    [SerializeField] private float _laserMaxLenght;
    [SerializeField] private AudioSource _laserAudioSource;

    private void OnEnable()
    {
        _projectileBase.OnShoot += OnShoot;

        _laserAudioSource.PlayOneShot(_impactSfxClip);

        //Destroy(this.gameObject, _maxLifeTime);
        StartCoroutine(WaitBefore(_maxLifeTime));
    }

    public new void OnShoot()
    {
        _last_rootPosition = _root.position;
        _velocity = transform.forward * _speed;
        _ignoredColliders = new List<Collider>();
        Collider[] selfColliders = _root.GetComponentsInParent<Collider>();
        Collider[] ownerColliders = _projectileBase.Owner.GetComponentsInParent<Collider>();

        _ignoredColliders.AddRange(selfColliders);
        _ignoredColliders.AddRange(ownerColliders);
    }

    private void FixedUpdate()
    {
        _root.localPosition = new Vector3(_root.localPosition.x, _root.localPosition.y, _laserMaxLenght / 2);
        _root.localScale = new Vector3(_root.localScale.x, _laserMaxLenght / 2, _root.localScale.z);

        // Обнаружение попадания
        RaycastHit hit;
        Ray ray = new(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, _laserMaxLenght, _hittableLayers, _triggerInteraction))
        {
            if (!_ignoredColliders.Contains(hit.collider))
            {
                if (_impactVfx)
                {
                    GameObject _impactVfxInstance = Instantiate(_impactVfx, hit.point + (hit.normal * _impactVfxSpawnOffset),
                        Quaternion.LookRotation(hit.normal));
                    if (_impactVfxLifetime > 0)
                    {
                        Destroy(_impactVfxInstance.gameObject, _impactVfxLifetime);
                    }
                }

                _root.localPosition = new Vector3(_root.localPosition.x, _root.localPosition.y, hit.distance / 2);
                _root.localScale = new Vector3(_root.localScale.x, hit.distance / 2, _root.localScale.z);
            }

            if (IsHitValid(hit))
            {
                OnHit(hit.point, hit.normal, hit.collider);
            }
        }

    }

    protected override void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        //base.OnHit(point, normal, collider);
        Damageable damageable = collider.GetComponent<Damageable>();
        if (damageable)
        {
            damageable.InflictDamage(_damage, _projectileBase.Owner);
        }
    }

    private IEnumerator WaitBefore(float duration)
    {
        yield return new WaitForSeconds(duration);

        OnDispose?.Invoke(this);
    }
}