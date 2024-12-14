using System.Numerics;

// https://blog.naver.com/vactorman/80167390138

namespace Udemy.MultiThreading.Lecture3 {
    public class APMExample_1
    {
        const int N = 10;

        // 5 "비동기 호출 작업이 끝날때 수행하는 APMFactorial 외부의 콜백 작성"
        #region 5 Done AsyncCallback 
            // 이제부터 작성할 코드는 실제 호출 위치에서 사용하는 코드이다.x
            // (위에서 만들었던 Factorial 클래스 내부가 아니다.)
        public static void FactorialDoneCallback(IAsyncResult asyncResult) {
            // BeginGetFactorial에서 var param = new object[] {state, p};를 받음
            var param = asyncResult.AsyncState as object[];
            if(param == null) return;

            APMFactorial factorial = param[0] as APMFactorial;
            var n = (int)param[1];

            var result = factorial.EndGetFactorial(asyncResult);
            Console.WriteLine("N : {0}, Calculate Result : {1}", n, result);
        }
        #endregion

        // BeginXXX() 호출부
        public static void MajorAction() {
            var factorial = new APMFactorial();
            factorial.BeginGetFactorial(N, FactorialDoneCallback, factorial);
        }
    }

    class APMFactorial {
        // 2 두번째, "병렬 처리하고 싶은 작업 메서드"의 레퍼런스를 담는 Delegate 생성
        #region Asynchronous Delegate 2
        private Func<int, BigInteger> delegateJob;

        public APMFactorial() {
            delegateJob = GetFactorial;
        }
        #endregion

        // 1 첫번째 "병렬 처리하고 싶은 작업 메서드" 작성하기
        #region Job Method 1
            // stack 에 저장되는 스레드내 공유되지 않은 데이터를 쓰는 예시지만,
            // 공유되는 메모리 (힙, 전역, 데이터) 이런것들을 사용하게 되면 문제가 생길 수 있어서 스레드 동기화를 해주는것이 좋다.
        public BigInteger GetFactorial(int p) {
            if(p <= 0) return -1;
            try {
                BigInteger n =1;
                for(var i = 1; i <= p; i++) {
                    n = n*i;
                    Thread.Sleep(100); // 슬립을 안 주면 너무 순식간에 처리되어서 동기 동작인지 비동기 동작인 구분하기 힘들다.)
                }
                return n;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                return -1;
            }
        }
        #endregion

        // 3 세번쨰 "비동기 호출을 시작하는 메서드" 작성
        #region 3 BeginXXX Method
            // state 에는 병렬 처리를 수행하는 주체인 APMFactorial이 들어간다.
            // https://github.com/dotnet/runtime/issues/16312#issuecomment-182107557 그런데.. 사용 못한다 ㅠ
        public IAsyncResult BeginGetFactorial(int p, AsyncCallback doneCallback, object state) {
            var param = new object[] {state, p};
            return delegateJob.BeginInvoke(p, doneCallback, param); 
        }
        #endregion

        // 4 네번쨰 "IAsyncResult를 인자로 받는 비동기 종료 메서드 작성"
        #region 4 EndXXX Method
        public BigInteger EndGetFactorial(IAsyncResult asyncResult) {
            return delegateJob.EndInvoke(asyncResult);
        }
        #endregion
    }
}