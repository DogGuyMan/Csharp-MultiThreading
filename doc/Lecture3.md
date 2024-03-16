병럴처리 테크닉
1. Optimize Latency : 하나의 작업을 SubTask로 나눠서 병렬 처리하기 (각 서브테스크들은 다시 하나의 결과물로 만들기 위해 처리해야하는 작업이 존재한다.)
   * 분할정복 이미지 처리
2. Optimize Throughput : 만약 여러 작업들이 서로 연관이 별로 없다면 그저 여러 작업들을 병렬 처리하는식으로 진행한다면 MAX 처리속도 만큼을 보장 받을 수 있고
   * ThreadPool을 이용하면 최적의 결과를 얻어낼 수 있다.
   * HTTP Request

[Thread vs ThreadPool vs Task  닷넷 C#](https://blog.naver.com/hyungjoon_/221700870194)
[Process, Thread, ThreadPool, Task 개념 간단 정리](https://rito15.github.io/categories/c-threading/)