using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : ProjectileBase
{

    private void OnEnable()
    {
        _projectileBase.OnShoot += OnShoot;
    }

    private new void OnShoot()
    {
        _last_rootPosition = _root.position;
        _velocity = transform.forward * _speed;
        _ignoredColliders = new List<Collider>();
        transform.position += _projectileBase.InheritedMuzzleVelocity * Time.deltaTime;

        // Игнорировать коллайдер хозяина
        Collider[] ownerColliders = _projectileBase.Owner.GetComponentsInParent<Collider>();
        _ignoredColliders.AddRange(ownerColliders);

        // Обработчик выстрела игрока. Чтобы снаряды не проходили сквозь стены и запомнить траектроию центра экрана(доп. камеры)
        ItemManager playerWeaponsManager = _projectileBase.Owner.GetComponent<ItemManager>();
        if (playerWeaponsManager)
        {
            Vector3 cameraToMuzzle = (_projectileBase.InitialPosition -
                                      playerWeaponsManager.ItemCamera.transform.position);

            _trajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                playerWeaponsManager.ItemCamera.transform.forward);

            _hasTrajectoryOverride = false;

            if (Physics.Raycast(playerWeaponsManager.ItemCamera.transform.position, cameraToMuzzle.normalized,
                out RaycastHit hit, cameraToMuzzle.magnitude, _hittableLayers, _triggerInteraction))
            {
                if (IsHitValid(hit))
                {
                    OnHit(hit.point, hit.normal, hit.collider);
                }
            }
        }
    }

    private void Update()
    {
        // Движение
        transform.position += _velocity * Time.deltaTime;

        // Центрировать снаряды, даже если оружие смещено
        if (_hasTrajectoryOverride && _consumedTrajectoryCorrectionVector.sqrMagnitude <
            _trajectoryCorrectionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = _trajectoryCorrectionVector - _consumedTrajectoryCorrectionVector;
            float distanceThisFrame = (_root.position - _last_rootPosition).magnitude;
            Vector3 correctionThisFrame =
                (distanceThisFrame / -1) * _trajectoryCorrectionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            _consumedTrajectoryCorrectionVector += correctionThisFrame;

            if (_consumedTrajectoryCorrectionVector.sqrMagnitude == _trajectoryCorrectionVector.sqrMagnitude)
            {
                _hasTrajectoryOverride = false;
            }

            transform.position += correctionThisFrame;
        }

        transform.forward = _velocity.normalized;

        // Обнаружение попадания
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = _tip.position - _last_rootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(_last_rootPosition, _radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, _hittableLayers,
                _triggerInteraction);
            foreach (var hit in hits)
            {
                if (IsHitValid(hit) && hit.distance < closestHit.distance)
                {
                    foundHit = true;
                    closestHit = hit;
                }
            }

            if (foundHit)
            {
                if (closestHit.distance <= 0f)
                {
                    closestHit.point = _root.position;
                    closestHit.normal = -transform.forward;
                }
                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }
        }

        _last_rootPosition = _root.position;
    }

    protected override void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        base.OnHit(point, normal, collider);
        EventManager.Dispose(this);
        //Destroy(this.gameObject);

    }


}
