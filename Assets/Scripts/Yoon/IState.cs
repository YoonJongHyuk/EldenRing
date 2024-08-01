/// <summary>
/// 시작, 업데이트, 삭제시 사용되는 함수
/// </summary>
interface IState
{
    /// <summary>
    /// 시작시 사용되는 함수
    /// </summary>
    public void EnterState();
    /// <summary>
    /// 업데이트시 사용되는 함수
    /// </summary>
    public void UpdateState();
    /// <summary>
    /// 삭제시 사용되는 함수
    /// </summary>
    public void ExitState();
}