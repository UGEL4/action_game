using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FixAnimationPath : EditorWindow
{
    private GameObject mtarget;
    private AnimationClip mClip;
    private string mError;

    static void FixAnimationPathMethod()
    {
        Rect rt = new Rect(0, 0, 500, 500);
        FixAnimationPath window = (FixAnimationPath)EditorWindow.GetWindowWithRect(typeof(FixAnimationPath), rt, true, "Fix Animation Path");
        window.Show();
    }

    bool DoFix()
    {
        if (mtarget == null) mError = "Target is null";
        if (mClip == null) mError = "Animation clip is null";

        if (mClip != null)
        {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(mClip);
            for (int i = 0; i < bindings.Length; ++i)
            {
                EditorCurveBinding binding = bindings[i];
                GameObject animatedObj = AnimationUtility.GetAnimatedObject(mtarget, binding) as GameObject;
                if (animatedObj == null)
                {
                    
                }
            }
        }

        return true;
    }
}
