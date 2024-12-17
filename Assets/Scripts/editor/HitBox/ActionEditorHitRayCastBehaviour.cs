using System.Collections.Generic;
using ACTTools;
using Log;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ActionEditorHitRayCastBehaviour : PlayableBehaviour
{
    //private readonly double frameTime = 0.03333333;
    public bool Record = false;
    public GameObject weapon;
    public GameObject characterRoot;
    public FrameIndexRange activeFrameRange;
    public ActionEditorHitRayCastClip clipAsset;
    public PlayableDirector director;

    public List<ActionEditorHitRayCastClip.AttackRayTurnOnInfo> AttackRayTurnOnInfoList;

    public List<GameObject> rayPoints = new List<GameObject>();
    private Dictionary<string, Transform> mRayPointTransformMap = new();
    public List<Vector3> lastFrameRayPoints = new List<Vector3>();
    private Dictionary<string, Vector3> mLastFramePointPosMap = new();
    private int mCurrentFrameIndex = 0;
    private int mLastFrameIndex = 0;

    public void OnPlayableCreateOverride()
    {
        // if (weapon != null)
        // {
        //     foreach (Transform child in weapon.transform)
        //     {
        //         if (child.CompareTag("ActionEditorRayCastPoint"))
        //         {
        //             //rayPoints.Add(child.gameObject);
        //             //lastFrameRayPoints.Add(child.position);

        //             mRayPointTransformMap.Add(child.name, child);
        //             mLastFramePointPosMap.Add(child.name, child.position);
        //         }
        //     }
        // }
        mLastFramePointPosMap.Clear();
        mRayPointTransformMap.Clear();
        if (director != null)
        {
            for (int i = 0; i < AttackRayTurnOnInfoList.Count; i++)
            {
                for (int j = 0; j < AttackRayTurnOnInfoList[i].PointNameList.Count; j++)
                {
                    string name  = AttackRayTurnOnInfoList[i].PointNameList[j];
                    Transform go = weapon.transform.Find(name);
                    //mRayPointTransformMap.Add(go.name, go.transform);
                    mRayPointTransformMap.Add(name, go);
                    if (go != null)
                    {
                        mLastFramePointPosMap[go.name] = go.position;
                    }
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
        if (temp < mCurrentFrameIndex)
        {
            //SimpleLog.Info("mCurrentFrameIndex: ", mCurrentFrameIndex);
            for (int i = 0; i < AttackRayTurnOnInfoList.Count; i++)
            {
                if (mCurrentFrameIndex >= (int)AttackRayTurnOnInfoList[i].ActiveFrame.min &&
                mCurrentFrameIndex <= (int)AttackRayTurnOnInfoList[i].ActiveFrame.max)
                {
                    List<ActionEditorHitRayCastClip.RayPointInfoPreFrame> list = new(rayPoints.Count);
                    for (int j = 0; j < AttackRayTurnOnInfoList[i].PointNameList.Count; j++)
                    {
                        SerializableTransformNoScale tmp = new();
                        string name = AttackRayTurnOnInfoList[i].PointNameList[j];
                        //Transform go = weapon.transform.Find(name);
                        Transform go = mRayPointTransformMap[name];
                        if (go == null)
                        {
                            continue;
                        }
                        if (Record)
                        {
                            ActionEditorHitRayCastClip.RayPointInfoPreFrame pointInfo = new();
                            pointInfo.Name = name;
                            if (characterRoot != null) // 记录在根节点下的变换
                            {
                                tmp.Position = characterRoot.transform.InverseTransformPoint(go.position);
                                tmp.Rotation = (Quaternion.Inverse(characterRoot.transform.rotation) * go.rotation).eulerAngles;
                            }
                            else
                            {
                                tmp.Position = go.position;
                                tmp.Rotation = go.rotation.eulerAngles;
                            }
                            pointInfo.transform = tmp;
                            list.Add(pointInfo);
                        }

                        //
                        if (mCurrentFrameIndex > (int)AttackRayTurnOnInfoList[i].ActiveFrame.min)
                        {
                            Debug.DrawLine(go.transform.position, mLastFramePointPosMap[go.name], Color.blue, 1.0f);
                        }
                        mLastFramePointPosMap[go.name] = go.transform.position;
                    }
                    if (mCurrentFrameIndex >= (int)AttackRayTurnOnInfoList[i].ActiveFrame.min)
                    {
                        string name0  = AttackRayTurnOnInfoList[i].PointNameList[0];
                        Transform go0 = mRayPointTransformMap[name0];
                        string name1  = AttackRayTurnOnInfoList[i].PointNameList[AttackRayTurnOnInfoList[i].PointNameList.Count - 1];
                        Transform go1 = mRayPointTransformMap[name1];
                        Debug.DrawLine(go0.transform.position, go1.transform.position, Color.red, 1.0f);
                    }
                    if (Record)
                    {
                        //int frameIndex = mCurrentFrameIndex - (int)AttackRayTurnOnInfoList[i].ActiveFrame.min;
                        //SimpleLog.Info("frameIndex: ", frameIndex, mCurrentFrameIndex);
                        clipAsset.RecordPointTransform(i, mCurrentFrameIndex - (int)AttackRayTurnOnInfoList[i].ActiveFrame.min, list);
                    }
                }
            }
        }
    }

    public override void OnGraphStart(Playable playable)
    {
        mCurrentFrameIndex = 0;
        mLastFrameIndex    = 0;
        Application.targetFrameRate = 30;
    }

    public override void OnGraphStop(Playable playable)
    {
        mCurrentFrameIndex = 0;
        mLastFrameIndex    = 0;
        Application.targetFrameRate = 60;
    }

}
