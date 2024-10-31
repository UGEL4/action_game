using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ActionEditActionClip : PlayableAsset
{
    public ExposedReference<GameObject> target;
    [SerializeField]
    public bool record = true;
    [SerializeField]
    private ActionEditActionBehaviour template = new ActionEditActionBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable     = ScriptPlayable<ActionEditActionBehaviour>.Create(graph, template);
        var behaviour    = playable.GetBehaviour();
        behaviour.record = record;
        GameObject go = target.Resolve(graph.GetResolver());
        if (go != null)
        {
            var trasnform = go.transform.Find("box_root");
            if (trasnform != null)
            {
                var childGo = trasnform.gameObject;
                BoxManager mgr = childGo.GetComponent<BoxManager>();
                behaviour.boxManager = mgr;
            }
        }
        behaviour.target = go;
        behaviour.director = owner.GetComponent<PlayableDirector>();
        return playable;
    }
}
