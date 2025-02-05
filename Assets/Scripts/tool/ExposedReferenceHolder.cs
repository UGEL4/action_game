
using UnityEngine;

[System.Serializable]
public class ExposedReferenceHolder<T> where T : Object
{
    public ExposedReference<T> Reference;
}

[System.Serializable]
public class ExposedReferenceGameObject : ExposedReferenceHolder<GameObject>
{

}