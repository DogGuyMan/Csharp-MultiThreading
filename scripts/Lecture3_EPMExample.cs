using System.ComponentModel;
using System.Numerics;

// https://blog.naver.com/vactorman/80167390138

namespace Udemy.MultiThreading.Lecture3
{
    // 여기서 궁금한것은 비동기로 
    // 돌릴 액션
    // 틱 작업
    // 종료 작업
    // 까지는 이해 했다.
    // 1?? 그런데 결과 반환은 어찌 하는것이며,
    // 2?? 지금 예제는 Busy Waiting인데 이거 메인스레드가 정지되는것 아닌가?
    #region 1 EPM Example With Busy Wait
    public class EPMExample_1
    {
        public static void MajorAction()
        {
            var bw = WorkerBuilder(WorkerDoWork, WorkerProgressChanged, WorkerComplete);

            bw.RunWorkerAsync();
            Console.WriteLine("Press C to cancel work");

            do
            {
                if (Console.ReadKey(true).KeyChar == 'C')
                    bw.CancelAsync();
            } while (bw.IsBusy);

        }
        // public class WorkCompleteResultInfo ❌ 값타입을 넘길 수는 없나보다.
        public class WorkCompleteResultInfo
        {
            private string mText;
            private int mVal;
            public WorkCompleteResultInfo(string text, int val) { mText = text; val = mVal; }
            public string Text => mText;
            public int Value => mVal;
        }

        static BackgroundWorker WorkerBuilder(DoWorkEventHandler wh, ProgressChangedEventHandler ph, RunWorkerCompletedEventHandler rh)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            bw.DoWork += wh;
            bw.ProgressChanged += ph;
            bw.RunWorkerCompleted += rh;

            return bw;
        }
        /// <summary>
        /// 실제 비동기로 돌릴 작업
        /// </summary>
        /// <param name="sender">BackgroundWorker가 들어갈 것</param>
        /// <param name="e"></param> 매개변수와 리턴값을 담는다 <summary>
        static void WorkerDoWork(object sender, DoWorkEventArgs eventArgs)
        {
            Console.WriteLine($"DoWork thread pool thread id: {Thread.CurrentThread.ManagedThreadId}");
            var bwRef = sender as BackgroundWorker;
            if (bwRef == null) return;
            // 실제 비동기로 돌릴 작업
            for (int i = 0; i < 100; i++)
            {
                if (bwRef.CancellationPending) { eventArgs.Cancel = true; return; }
                if (i % 10 == 0) bwRef.ReportProgress(i);
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            eventArgs.Result = new WorkCompleteResultInfo("The answer is ", 42);
        }

        static void WorkerProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
        {
            var progressPercentage = eventArgs.ProgressPercentage;
            Console.WriteLine($"{progressPercentage}% Complete. Progress thread pool's thread id {Thread.CurrentThread.ManagedThreadId}");
        }

        static void WorkerComplete(object sender, RunWorkerCompletedEventArgs eventArgs)
        {
            Console.WriteLine($"Completed Thread pool's thread id : {Thread.CurrentThread.ManagedThreadId}");
            if (eventArgs.Error != null)
            {
                Console.WriteLine($"Exception {eventArgs.Error.Message} has occured.");
            }
            else if (eventArgs.Cancelled)
            {
                Console.WriteLine($"Operation has been canceled.");
            }
            else
            {
                WorkCompleteResultInfo res = eventArgs.Result as WorkCompleteResultInfo;
                Console.WriteLine($"{res.Text} {res.Value}");
            }
        }
    }
    #endregion

    // 결과 받아오기도 확인했다.
    // BusyWait이 아닌 대기도 확인하긴 했는데 아직도 언제 스레드 블락을 하는지 알 수 없다.
    // 약간 짐작 가는게 작업을 다룰때, Cancel 방식으로 처리한다.
    // 1. 클레스는 뭐지?
    // 2. 이벤트는 안다. EventArgs를 사용하는 | DoWork | ProgressChanged | Complete |
    // 3. AsyncOperation은 뭐지?
    // 4. SendOrPostCallback은 뭐지?
    // 5. 비동기 델리게이트 시작은 대충 bw.RunWorkerAsync(); 인듯하다.
    // 6. 어.. 동기 메서드는 뭐지 DoWork인가?

    #region 2 EPM Example With Return And Cancellation
    public class EPMExample_2
    {
        public static void MajorAction()
        {
            var tcs = new TaskCompletionSource<int>();
            var epmInstance = new EMPClass(tcs);
            var worker = epmInstance.Worker;

            worker.RunWorkerAsync();
            // int result = epmInstance.TCS.Task.Result;
            int result = tcs.Task.Result;

            Console.WriteLine("Result is: {0}", result);
        }

        public class EMPClass
        {
            public TaskCompletionSource<int> TCS { get; private set; }
            public BackgroundWorker Worker { get; private set; }
            public EMPClass(TaskCompletionSource<int> taskCompletionSource)
            {
                BackgroundWorker Worker = new BackgroundWorker();

                Worker.DoWork += WorkerDoWork;
                Worker.RunWorkerCompleted += WorkerComplete;

                TCS = taskCompletionSource;
            }

            private void WorkerDoWork(object sender, DoWorkEventArgs eventArgs)
            {
                var threadID = Thread.CurrentThread.ManagedThreadId;
                bool isThreadPool = Thread.CurrentThread.IsThreadPoolThread;
                Console.WriteLine($"Task {"Background worker"} is running on a thread id {threadID}. Is thread pool thread: {isThreadPool}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                eventArgs.Result = 42 * 5;
            }

            private void WorkerComplete(object sender, RunWorkerCompletedEventArgs eventArgs)
            {
                if (eventArgs.Error != null)
                {
                    TCS.SetException(eventArgs.Error);
                }
                else if (eventArgs.Cancelled)
                {
                    TCS.SetCanceled();
                }
                else
                {
                    TCS.SetResult((int)eventArgs.Result);
                }
            }

        }
    }
    #endregion

    // EventArgs를 상속하여 커스텀으로 만드는 예시.
    // 여기서는 스레드 대기에 대한 단서를 얻을 수 있을까?
    // 하지만, 여기도 IsBusy를 사용해 웨이팅하는 모습이다..
    #region 3 Calculate Factorial 
    public class EPMExample_3
    {
        const int PNUM = 10;
        public static void MajorAction() {
            var calculator = new CalculateFactorial 
            {
                WorkerReportsProgress = true,
                WorkerReportsCancellation = true
            };

            calculator.ProgressChanged += WorkerProgressChanged;
            calculator.CalculateCompleted += WorkerCalculateCompleted;

            var i = 0;
            while (i < 5) {
                if (calculator.IsBusy) {continue;}
                calculator.CalculateAsync(PNUM + i);
                i++;
            }
        }

        static void WorkerProgressChanged(object send, ProgressChangedEventArgs eventArgs) {
            Console.WriteLine("Calculating : {0}% Completed", eventArgs.ProgressPercentage);
        }
        static void WorkerCalculateCompleted(object send, CalculateCompletedEventArgs eventArgs) {
            Console.WriteLine(eventArgs.Result);
        }

        #region EventArgs
        public class ProgressChangedEventArgs : EventArgs {
            public int ProgressPercentage {get; private set;}
            public object UserState {get; private set;}
            public ProgressChangedEventArgs(int progressPercentage, object userState) {
                ProgressPercentage = progressPercentage;
                UserState = userState;
            }
        }
        #endregion

        public class CalculateCompletedEventArgs : AsyncCompletedEventArgs
        {
            private object mResult;
            public object Result {
                get {
                    base.RaiseExceptionIfNecessary();
                    return this.mResult;
                }
            }
            public object UserState {get => base.UserState;}

            public CalculateCompletedEventArgs(object result, Exception? error, bool cancelled, object? userState) : base(error, cancelled, userState)
            {
                this.mResult = result;
            }
        }

        #endregion

        #region Factorial Class
        public class CalculateFactorial
        {
            #region Getter Setter
                public bool IsBusy {get; private set;}
                public bool CancellationPending {get; private set;}
                public bool WorkerReportsProgress {get; set;}
                public bool WorkerReportsCancellation {get; set;}
            #endregion
                private AsyncOperation mAsyncOperation;

                // 1. 첫번째 작업 델리게이트 & 이벤트 델리게이트 초기화
                public CalculateFactorial() {
                    this.StartDelegate = WorkerThreadStart;                   // 1. 작업 델리게이트 초기화
                    this.mOperationCompleted = SendOperationCompletedEvent; // 2. 이벤트 발생 (작업 완료) 델리게이트 초기화
                    this.mProgressReporter = SendProgressChangedEvent;      // 3. 이벤트 발생 (중간 리포팅) 델리게이트 초기화
                }

            #region Calculate

                public void CancelAsync() {
                    if (!this.WorkerReportsCancellation){
                        throw new InvalidOperationException("BackgroundWorker_WorkerDoesntSupportCancellation");
                    }
                    this.CancellationPending = true;
                }
                
                // 2. 비동기 시작 메서드 구현
                    // 중복 호출을 막는 설정 (단일 스레딩 호출 예시)
                        // IsBusy
                    // AsyncOperation 객체 초기화
                    // 작업 델리게이트 BeginInvoke() 호출 수행
                public void CalculateAsync(object argument) {
                    this.IsBusy = true;
                    this.CancellationPending = false;
                    this.mAsyncOperation = AsyncOperationManager.CreateOperation(null); // ????
                    // ($0 : "StartDelegate"의 인자, $1 : 콜백으로 호출할 델리게이트, $2 : 콜백의 내용을 전달할 객체)
                    this.StartDelegate.BeginInvoke(argument, null, null);
                }

                public BigInteger Calculate(int p, bool isAsync = false) {
                    if(p <= 0)
                        throw new InvalidOperationException("Can not calculate by input data.");
                    BigInteger res = BigInteger.One;
                    for(int i = 2; i <= p; i++) {
                        if(this.CancellationPending) return res;
                        res *= p;
                        Thread.Sleep(500);
                        if(isAsync)
                            this.ReportProgress((int)((double)i / (double)p) * 100);
                    }
                    return res;
                }

            #endregion

            #region Event
                public event EventHandler<CalculateCompletedEventArgs> CalculateCompleted;
                private SendOrPostCallback mOperationCompleted;
                
                private void SendOperationCompletedEvent(object arg) // 6. 작업 이 완료됨을 알려주는 이벤트 AsyncOperation에 의해 호출된다.
                {
                    this.IsBusy = false;
                    this. CancellationPending = false;

                    var eventArgs = arg as CalculateCompletedEventArgs;
                    if (eventArgs == null) return;

                    var completed = this. CalculateCompleted;
                    if (completed != null) completed (this, eventArgs);
                }
                public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
                private SendOrPostCallback mProgressReporter;
                private void SendProgressChangedEvent(object arg) // 4. 작업 과정을 알려줄 이벤트 AsyncOperation에 의해 호출된다.
                {
                    var eventArgs = arg as ProgressChangedEventArgs;
                    if (eventArgs == null) return;
                    
                    var progressChanged = this.ProgressChanged;
                    if (progressChanged != null) 
                        progressChanged (this, eventArgs);
                }

                private void ReportProgress(int percentProgress) // 5. AsyncOperation의 Post를 호출할 별도의 메서드
                {
                    if(!this.WorkerReportsProgress) {
                        throw new InvalidOperationException("BackgroundWorker_WorkerDoesntReportProgress");
                    }

                    var arg = new ProgressChangedEventArgs(percentProgress, null);
                    if(this.mAsyncOperation != null) {
                        this.mAsyncOperation.Post(mProgressReporter, null);
                    }
                    else {
                        this.mProgressReporter(arg);
                    }
                }


                private readonly Action<object> StartDelegate;
                private void WorkerThreadStart(object arg) // 3. "public BigInteger Calculate(int p, bool isAsync = false)"를 실제로 호출하는 작업
                {
                    object result = null;
                    Exception error = null;
                    try {
                        result = this.Calculate((int)arg, true);
                    } catch (Exception ex) {error = ex;}

                    this.mAsyncOperation.PostOperationCompleted(
                        this.mOperationCompleted, 
                        new CalculateCompletedEventArgs(result, error, this.CancellationPending, null)
                    );
                }        
            #endregion
        }
        #endregion
    }
}