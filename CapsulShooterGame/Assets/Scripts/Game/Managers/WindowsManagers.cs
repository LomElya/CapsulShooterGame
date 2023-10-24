using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManagers : MonoBehaviour
{
    public static Transform GuiHolder { get { return FindObjectOfType<GUIHolder>().transform; } }

    private const string PrefabsFilePath = "Canvases/";

    //При создании новых окон добавлть их сюда
    private static readonly Dictionary<Type, string> PrefabsDictionary = new Dictionary<Type, string>()
        {
            {typeof(AlertWindow),"AlertWindowCanvas"},
            {typeof(ControlsWindow),"ControlsWindow"},
            {typeof(ConfirmWindow),"ConfirmWindow"},
        };

    public static T ShowDialog<T>() where T : WindowCore
    {
        var go = GetPrefabByType<T>();
        if (go == null)
        {
            Debug.LogError("Show window - object not found");
            return null;
        }

        return GameObject.Instantiate(go, GuiHolder);
    }

    private static T GetPrefabByType<T>() where T : WindowCore
    {
        var prefabName = PrefabsDictionary[typeof(T)];
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("cant find prefab type of " + typeof(T) + "Do you added it in PrefabsDictionary?");
        }

        var path = PrefabsFilePath + PrefabsDictionary[typeof(T)];
        var dialog = Resources.Load<T>(path);
        if (dialog == null)
        {
            Debug.LogError("Cant find prefab at path " + path);
        }

        return dialog;
    }
}
