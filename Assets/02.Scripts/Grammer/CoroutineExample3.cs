using System.Collections;
using UnityEngine;

public class CoroutineExample3 : MonoBehaviour
{
    private void Start()
    {
        //1. 코루틴도 함수이므로 인자를 넘겨줄 수 있다.
        StartCoroutine(Ready_Coroutine(1f));
    }
    private IEnumerator Sequence_Coroutine()
    {
        //여러 코루틴을 실행할 경우 중첩 코루틴을 사용하지 말고 시퀀스 방식으로 해결
        yield return StartCoroutine(Ready_Coroutine(1f));
        yield return StartCoroutine(Start_Coroutine(1f));
        yield return StartCoroutine(End_Coroutine(1f));
    }

    private IEnumerator Ready_Coroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Debug.Log($"{seconds}초 대기");
        //2. 코루틴 내부에서도 코루틴을 호출할 수 있다. (중첩 코루틴은 사용 X)
        //StartCoroutine(Start_Coroutine(seconds));
    }
    private IEnumerator Start_Coroutine(float seconds)
    {
        Debug.Log($"{seconds}초 대기");
        yield return new WaitForSeconds(seconds);
        //StartCoroutine(End_Coroutine(seconds));
    }
    private IEnumerator End_Coroutine(float seconds)
    {
        Debug.Log($"{seconds}초 대기");
        yield return new WaitForSeconds(seconds);
        Debug.Log("종료");
    }
}
