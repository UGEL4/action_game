using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelComponent : ComponentBase
{
    private bool mLoadCompleted;
    private int mLoadCount;
    private int ModelNum;
    public List<ModelData> ModelDataList;
    private GameObject mModelRoot;
    public ModelComponent(CharacterObj owner, GameObject modelRoot) : base(owner)
    {
        ModelNum = 0;
        mLoadCount = 0;
        mLoadCompleted = false;
        mModelRoot    = modelRoot;
        ModelDataList = new List<ModelData>();
    }

    public ModelComponent(CharacterObj owner) : base(owner)
    {
        ModelNum = 0;
        mLoadCount = 0;
        mLoadCompleted = false;
        ModelDataList = new List<ModelData>();
    }

    public void SetModelRoot(GameObject root)
    {
        mModelRoot = root;
    }

    public override void UpdateLogic(int frameIndex)
    {
        base.UpdateLogic(frameIndex);
    }

    public override void UpdateRender(float deltaTime)
    {
        base.UpdateRender(deltaTime);
    }

    public override void BeginPlay()
    {
        base.BeginPlay();
    }

    public override void EndPlay()
    {
        for (int i = 0; i < ModelDataList.Count; i++)
        {
            ModelDataList[i].Destroy();
        }
        mModelRoot = null;
        ModelDataList = null;
        base.EndPlay();
    }

    public void LoadModel(List<string> prefabs)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            ModelData model = new ModelData(prefabs[i]);
            ModelDataList.Add(model);
            if (mModelRoot)
            {
                model.SetParent(mModelRoot.transform);
                model.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                model.SetScale(Vector3.one);
            }
        }
    }

    public void PlayAnimation(string animationName)
    {
        for (int i = 0; i < ModelDataList.Count; i++)
        {
            ModelDataList[i].PlayAnimation(animationName);
        }
    }
}
