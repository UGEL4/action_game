using System.Collections.Generic;
using ACTTools;
using Log;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ActionEditorHitRayCastBehaviour : PlayableBehaviour
{
    //private readonly double frameTime = 0.03333333;
    public GameObject weapon;
    public GameObject characterRoot;
    public FrameIndexRange activeFrameRange;
    public ActionEditorHitRayCastClip clipAsset;
    public PlayableDirector director;

    public List<GameObject> rayPoints = new List<GameObject>();
    public List<Vector3> lastFrameRayPoints = new List<Vector3>();
    private int mCurrentFrameIndex = 0;
    private int mLastFrameIndex = 0;

    public void OnPlayableCreateOverride()
    {
        if (weapon != null)
        {
            foreach (Transform child in weapon.transform)
            {
                if (child.CompareTag("ActionEditorRayCastPoint"))
                {
                    rayPoints.Add(child.gameObject);
                    lastFrameRayPoints.Add(child.transform.position);
                }
            }
        }
    }

    private void DebugDrawRay()
    {
        if (rayPoints.Count > 1)
        {
            var begin = rayPoints[0].transform;
            var end   = rayPoints[rayPoints.Count - 1].transform;
            Debug.DrawLine(begin.position, end.position, Color.red, 1.0f);
        }
    }

    private void DebugDrawRayQuad()
    {
        if (mCurrentFrameIndex > activeFrameRange.min)
        {
            for (int i = 0; i < rayPoints.Count; ++i)
            {
                if (i >= lastFrameRayPoints.Count) break;
                var newPos = rayPoints[i].transform.position;
                var oldPos = lastFrameRayPoints[i];
                Debug.DrawLine(newPos, oldPos, Color.blue, 1.0f);
            }
        }
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //var time = playable.GetTime();
        //var previousTiome = playable.GetPreviousTime();
        //var leadTime = playable.GetLeadTime();
        //TimelinePlayable
        TimelineAsset asset = director.playableAsset as TimelineAsset;
        double frameRate = asset.editorSettings.frameRate;
        //mCurrentFrameIndex = (int)(director.time / frameTime);
        mCurrentFrameIndex = (int)(director.time * frameRate);
        int temp = mLastFrameIndex;
        mLastFrameIndex = mCurrentFrameIndex;
        //if (temp > mCurrentFrameIndex) return;
        if (mCurrentFrameIndex >= activeFrameRange.min && mCurrentFrameIndex <= activeFrameRange.max)
        {
            DebugDrawRay();
            DebugDrawRayQuad();
            //记录当前帧的变换
            for (int i = 0; i < rayPoints.Count; ++i)
            {
                lastFrameRayPoints[i] = rayPoints[i].transform.position;
            }
        }
        if (temp < mCurrentFrameIndex)
        {
            List<SerializableTransformNoScale> list = new(rayPoints.Count);
            for (int i = 0; i < rayPoints.Count; ++i)
            {
                SerializableTransformNoScale tmp = new();
                if (characterRoot != null) //记录在根节点下的变换
                {
                    tmp.Position = characterRoot.transform.InverseTransformPoint(rayPoints[i].transform.position);
                    tmp.Rotation = (Quaternion.Inverse(characterRoot.transform.rotation) * rayPoints[i].transform.rotation).eulerAngles;
                }
                else
                {
                    tmp.Position = rayPoints[i].transform.position;
                    tmp.Rotation = rayPoints[i].transform.rotation.eulerAngles;
                }
                list.Add(tmp);
            }
            clipAsset.RecordPointTransform(mCurrentFrameIndex, list);
        }
    }

    public override void OnGraphStart(Playable playable)
    {
        mCurrentFrameIndex = 0;
        mLastFrameIndex    = 0;
    }

    public override void OnGraphStop(Playable playable)
    {
        mCurrentFrameIndex = 0;
        mLastFrameIndex    = 0;
    }

}
