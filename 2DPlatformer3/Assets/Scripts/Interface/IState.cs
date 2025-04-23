using UnityEngine;

public interface IState // 사용 안함
{
    public void OnStateStart();
    public void OnStateUpdate();
    public void OnStateEnd();
}