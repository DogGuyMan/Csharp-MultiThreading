### 앱 성능에 대한 두가지 기준 (지연 시간 & 처리량)
성능은 시나리오와 용례에 따라 완전히 다르게 측정될 수 있습니다.

#### 1. 지연시간 (Latency)  : 하나의 Task(작업)을 처리하는 시간
Optimize Latency : 멀티 스레딩으로 지연시간의 최적화를 달성하는 방법으로 SubTask 기법이 있다. 
**하나의 Task(작업)을 SubTask로 나눠서 병렬 처리**하는 방법이다.
* EX). 분할정복, 이미지 처리, CPU Bound
* 각 SubTask들은 다시 하나의 결과물로 만들기 위해 처리해야하는 Task(작업)이 존재한다.      
   ```
   Task 나누기,
   Thread 생성 비용,
   Thread에 Task(작업)을 전달하고 시작하는 비용,
   스케쥴링하여 Thread가 실행되기까지 시간,
   집계를 마친 신호 기다리기,
   Main Thread가 재실행 되는데 까지 걸리는 시간,
   SubTask들을 하나로 통합하는 비용   
   ```
* 이론상 향상 수치
   * $T(하나의 Task(작업))$, $N(SubTask 수)$
   $T/N(하나의 Task(작업)을 SubTask로 나눈 최적화된 Latency)$
   * 이론상 N의 최대 크기는 CPU코어의 개수가 된다. 그리고 그 수를 넘어서게 된다면 오히려 성능 저하가 발생한다.
   * 그리고 모든 Thread가 인터럽트 없이 SubTask 실행해야 최적입니다. 
   다시 말해, 모든 Thread가 항상 runnable 상태여야 한다.

#### 2. 처리량 (Throughput)  : 단일 시간당 처리 완료하는 일의 양
Optimize Throughput : 멀티 스레딩으로 처리량 최적화를 달성하는 방법으로 
만약 여러 Task(작업)들이 서로 연관이 별로 없어 결과물 처리작업을 몇가지 생략 할 수 있다면
**그저 여러 Task(작업)들을 독립 처리하는** 식으로 처리량 최적화가 가능하며, **Thread Pool로 생성 비용을 최적화 하는것**이 가능하다.
* EX). HTTP Request, I/O Bound
* Thread 풀은 Thread의 생성 삭제에 발생하는 비용을 낮추기 위해 사용가능한 테크닉이고 다음 과정으로 Thread를 처리한다.
   ```
   Thread는 생성되면 Pool에 쌓이게 되고
   Task(작업)이 Queue를 통해 Thread 별로 분배가 된다
   Thread는 이용 가능할때 마다 Queue를 통해 작업을 받는다.
   ```
   > 쉬운 코딩 : 큐에 요청이 무한정 쌓이게 되는지 큐의 사이즈를 확인해봐야 합니다. 상황에 따라 메모리를 고갈 시키는 잠재적인 위험 요인이 될 수 있습니다. 요청을 버리더라도 전체 시스템을 유지하는 방식을 택해야 할떄도 있습니다.

---

### 참고

1. [Thread vs ThreadPool vs Task  닷넷 C#](https://blog.naver.com/hyungjoon_/221700870194)
2. [Process, Thread, ThreadPool, Task 개념 간단 정리](https://rito15.github.io/categories/c-threading/)
3. [쉬운 코딩 :스레드 풀(thread pool)은 왜 쓰는 걸까요? 어떻게 쓰는게 잘 쓰는 걸까요? 지금 이 영상으로 스레드 풀! 깔끔하게 정리하시죠!](https://www.youtube.com/watch?v=B4Of4UgLfWc&list=PLcXyemr8ZeoQOtSUjwaer0VMJSMfa-9G-&index=12)
4. [쉬운 코딩 : cpu bound, io bound 의미를 설명합니다! 이에 따른 스레드 개수를 정하는 팁도 알려드립니다!](https://www.youtube.com/watch?v=qnVKEwjG_gM&list=PLcXyemr8ZeoQOtSUjwaer0VMJSMfa-9G-&index=3)