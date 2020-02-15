
public interface IRoomDisplayer
{
    // 화면에 표시.
    void OnVisible();
    // 화면에서 제거.
    void OnInvisible();

    // 최초에 한해 호출. 멤버 변수들을 초기화
    void Init();

    // 초기 상태로 되돌림.
    void ResetDisplayer();
}
