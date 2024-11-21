using UnityEngine;
using UnityEditor;

public class AnimationPreviewWindow : EditorWindow
{
    private AnimationClip animationClip; // 要预览的动画剪辑
    private GameObject previewObject; // 预览对象
    private Animator animator; // Animator 组件

    private AnimatorOverrideController aoc;

    private bool isPlaying = false; // 动画播放状态
    private float playbackTime = 0f; // 播放时间
    private float sliderValue = 0f;

    [MenuItem("Window/Animation Preview")]
    public static void ShowWindow()
    {
        GetWindow<AnimationPreviewWindow>("Animation Preview");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation Preview", EditorStyles.boldLabel);

        // 选择动画剪辑
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
        
        // 选择预览对象
        previewObject = (GameObject)EditorGUILayout.ObjectField("Preview Object", previewObject, typeof(GameObject), true);

        if (previewObject != null)
        {
            // 确保预览对象有 Animator 组件
            if (animator == null)
            {
                animator = previewObject.GetComponent<Animator>();
            }

            // 确保动画状态名称正确
            if (animationClip != null)
            {
                // 播放按钮
                if (GUILayout.Button(isPlaying ? "Stop Animation" : "Play Animation"))
                {
                    isPlaying = !isPlaying;
                    if (isPlaying)
                    {
                        playbackTime = (float)EditorApplication.timeSinceStartup;
                        Debug.Log("Playing animation: " + animationClip.name);
                        //animator.Play(animationClip.name, 0, 0f); // 替换为您的动画状态名称
                        animationClip.SampleAnimation(previewObject, 1);

                        var a = animator.GetCurrentAnimatorClipInfo(0);

                        var binds = AnimationUtility.GetCurveBindings(a[0].clip);
                        var curves = AnimationUtility.GetEditorCurve(a[0].clip, binds[0]);
                        var keyFrames = curves.keys;
                        int b = 0;
                    }
                }
            }
            else
            {
                GUILayout.Label("Please assign an animation clip.");
            }
            sliderValue = GUILayout.HorizontalSlider(sliderValue, 0, animationClip.length);
            if (animationClip != null && !isPlaying)
            {
                //AnimationCurve
                animationClip.SampleAnimation(previewObject, sliderValue);
            }
        }
        else
        {
            GUILayout.Label("Please assign a preview object.");
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
        // 清理资源
        if (previewObject != null && animator != null)
        {
            //DestroyImmediate(animator);
            animator = null;
            aoc = null;
        }
    }

    private void OnEnable()
    {
        isPlaying = false;
        playbackTime = (float)EditorApplication.timeSinceStartup;
        // 在编辑模式下更新
        EditorApplication.update += Update;
    }

    private void Update()
    {
        if (isPlaying && animator != null && animationClip != null)
        {
            // playbackTime += Time.deltaTime;
            // if (playbackTime > animationClip.length)
            // {
            //     playbackTime = 0f;
            // }
            // Debug.Log("playbackTime: " + (playbackTime / animationClip.length));
            // animator.Play(animationClip.name, 0, playbackTime / animationClip.length);

            // 更新 Animator 状态
            //float next_time = (float)EditorApplication.timeSinceStartup - playbackTime;
            //playbackTime = (float)EditorApplication.timeSinceStartup;
            //animator.Update(next_time);
        }
    }

    private void ExportBox()
    {
        //animationClip.length;
    }
}