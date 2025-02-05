using UnityEngine;
using UnityEngine.InputSystem;

public interface IController
{
    public void SetOwner(CharacterObj owner)
    {
    }

    public void InitializeInput()
    {
    }

    public void BeginPlay()
    {
    }

    public void UpdateLogic()
    {
    }

    public void OnDestroy()
    {
    }

    public void SetInputReader(InputReader input)
    {
    }

    void OnMove(Vector2 xy, InputActionPhase phase)
    {
    }

    void ProduceInputDir()
    {
    }

    void OnAttackA(InputActionPhase phase)
    {
    }

    void OnAttackB(InputActionPhase phase)
    {
    }

    void OnButtonA(InputActionPhase phase)
    {
    }

    void OnButtonB(InputActionPhase phase)
    {
    }

    public void Jump(InputActionPhase phase)
    {
    }
}
