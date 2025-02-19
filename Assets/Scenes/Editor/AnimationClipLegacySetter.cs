using UnityEngine;
using UnityEditor;

public class AnimationClipLegacySetter : MonoBehaviour
{
    [MenuItem("Tools/Set Animation Clip to Legacy")]
    static void SetClipToLegacy()
    {
        // 선택한 애니메이션 클립 가져오기
        Object[] selectedClips = Selection.objects;
        foreach (Object obj in selectedClips)
        {
            if (obj is AnimationClip clip)
            {
                // Legacy 속성 활성화
                SerializedObject serializedClip = new SerializedObject(clip);
                serializedClip.FindProperty("m_Legacy").boolValue = true;
                serializedClip.ApplyModifiedProperties();

                Debug.Log($"AnimationClip '{clip.name}' is now set to Legacy.");
            }
        }
    }
}
