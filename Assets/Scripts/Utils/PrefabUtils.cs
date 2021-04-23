using UnityEngine;
using UnityEditor;
public static class PrefabUtils
{
    /// <summary>
    /// When changing value through editor script, store the change in as
    /// modification to the Prefab. Otherwise the change will be lost when the
    /// prefab is reloaded.
    /// </summary>
    /// <param name="obj"></param>
    public static void RecordPrefab(this Object obj)
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }
    }
}