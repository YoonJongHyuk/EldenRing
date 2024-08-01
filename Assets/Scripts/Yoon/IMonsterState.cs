
/// <summary>
/// 몬스터 기본 기능들을 모은 인터페이스
/// </summary>
interface IMonsterState
{
    /// <summary>
    /// 몬스터가 사망했을때 사용하는 함수
    /// </summary>
    public void Die();
    /// <summary>
    /// 몬스터가 이동할때 사용하는 함수
    /// </summary>
    public void Move();
    /// <summary>
    /// 몬스터가 공격할때 사용하는 함수
    /// </summary>
    public void Attack();
}