using System;
using UnityEngine;

public class ModelData
{
    public GameObject Model;

    private Animator mAnimator;

    public System.Action LoadCompleted;

    public ModelData(string prefabName)
    {
        var prefab = Resources.Load<GameObject>(prefabName);
        if (prefab)
        {
            Model = GameObject.Instantiate(prefab);
            Model.name = prefab.name;
            mAnimator = Model.GetComponent<Animator>();
        }
    }

    public ModelData(GameObject model)
    {
        Model = model;
        if (model)
        {
            mAnimator = Model.GetComponent<Animator>();
        }
    }

    public void Destroy()
    {
        mAnimator = null;
        Model     = null;
    }

    public void PlayAnimation(string animationName)
    {
        if (mAnimator)
        {
            mAnimator.CrossFade(animationName, 0, 0, 0);
        }
    }

    public void SetParent(Transform parent)
    {
        if (Model)
        {
            Model.transform.SetParent(parent);
        }
    }

    public void SetLocalPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (Model)
        {
            Model.transform.SetLocalPositionAndRotation(position, rotation);
        }
    }

    public void SetScale(Vector3 scale)
    {
        if (Model)
        {
            Model.transform.localScale = scale;
        }
    }

}