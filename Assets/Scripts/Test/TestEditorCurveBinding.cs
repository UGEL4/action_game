using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEditorCurveBinding : MonoBehaviour
{
    public AnimationClip animationClip; // 要预览的动画剪
    private Animator animator; // Animator 组件
    // Start is called before the first frame update

    public GameObject go;
    public GameObject root;
    public Transform rootBone;
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null && animationClip == null)
        {
            AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(0);
            if (info.Length > 0)
            {
                animationClip = info[0].clip;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animationClip != null)
        {
            //var binds = AnimationUtility.GetAnimatableBindings(go, root);
            var binds = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
            if (binds.Length > 0)
            {
                Debug.Log("bind path: " + binds[0].path);
                //var curves = AnimationUtility.GetEditorCurve(animationClip, binds[0]);
                //var keyFrames = curves.keys;
            }
            var allCurves = AnimationUtility.GetCurveBindings(animationClip);
            if (allCurves.Length > 0)
            {
                //Debug.Log("curve path: " + allCurves[0].path);
                //GameObject animatedObj = AnimationUtility.GetAnimatedObject(go, allCurves[0]) as GameObject;
                //var curves = AnimationUtility.GetEditorCurve(animationClip, allCurves[0]);
                //var keyFrames = curves.keys;
                var pos = go.transform.Find("Bip001/Bip001 Pelvis").transform.position;
                animationClip.SampleAnimation(go, 1);
                pos = go.transform.Find("Bip001/Bip001 Pelvis").transform.position;
                int a = 0;
            }
            Debug.Log("rootBone path: " + rootBone.name);
        }
    }
}
