using UnityEngine;

public class GameMain : MonoBehaviour
{
    public Character player;
    void Start()
    {
    }

    void Update()
    {
        if (player.CanAttackTargetNow(out AttackInfo attackPhase))
        {
            DoAttack(attackPhase);
        }
    }

    void DoAttack(AttackInfo attackPhase)
    {
        //CancelTag开启
        // foreach (string cTag in attackPhase.tempBeCancelledTagTurnOn)
        // {
        //     player.GetActionController().AddTempBeCanceledTag(cTag);
        // }
    }
}