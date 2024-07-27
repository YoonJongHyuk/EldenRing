/// <summary>
/// ����, ������Ʈ, ������ ���Ǵ� �Լ�
/// </summary>
interface IState
{
    /// <summary>
    /// ���۽� ���Ǵ� �Լ�
    /// </summary>
    public void EnterState();
    /// <summary>
    /// ������Ʈ�� ���Ǵ� �Լ�
    /// </summary>
    public void UpdateState();
    /// <summary>
    /// ������ ���Ǵ� �Լ�
    /// </summary>
    public void ExitState();
}