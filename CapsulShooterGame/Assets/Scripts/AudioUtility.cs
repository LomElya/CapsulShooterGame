using UnityEngine;
using UnityEngine.Audio;

public class AudioUtility
{
    static AudioManager _AudioManager;

    public enum AudioGroups
    {
        DamageTick,
        EnemyDetection,
        WeaponOverheat,
        WeaponChargeBuildup,
        WeaponChargeLoop,
        Use,
        Pickup,
        Shoot,
        Move,
        Main,
    }

    public static void CreateSFX(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend,
           float rolloffDistanceMin = 1f, float clipLength = 0f)
    {
        GameObject impactSfxInstance = new GameObject();
        impactSfxInstance.transform.position = position;
        AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = spatialBlend;
        source.minDistance = rolloffDistanceMin;
        source.Play();

        source.outputAudioMixerGroup = GetAudioGroup(audioGroup);

        TimedSelfDestruct timedSelfDestruct = impactSfxInstance.AddComponent<TimedSelfDestruct>();

        if (clipLength == 0f)
            clipLength = clip.length;

        timedSelfDestruct.LifeTime = clipLength;
    }

    public static AudioMixerGroup GetAudioGroup(AudioGroups group)
    {
        if (_AudioManager == null)
            _AudioManager = GameObject.FindObjectOfType<AudioManager>();

        var groups = _AudioManager.FindMatchingGroups(group.ToString());

        if (groups.Length > 0)
            return groups[0];

        Debug.LogWarning("Didn't find audio group for " + group.ToString());
        return null;
    }

    public static void SetMasterVolume(float value)
    {
        if (_AudioManager == null)
            _AudioManager = GameObject.FindObjectOfType<AudioManager>();

        if (value <= 0)
            value = 0.001f;
        float valueInDb = Mathf.Log10(value) * 20;

        _AudioManager.SetFloat("MasterVolume", valueInDb);
    }

    public static float GetMasterVolume()
    {
        if (_AudioManager == null)
            _AudioManager = GameObject.FindObjectOfType<AudioManager>();

        _AudioManager.GetFloat("MasterVolume", out var valueInDb);
        return Mathf.Pow(10f, valueInDb / 20.0f);
    }
}
