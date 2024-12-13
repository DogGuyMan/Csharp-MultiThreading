using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading;
using S
namespace Udemy.MultiThreading.Lecture3
{
    /*
    * https://csharpstudy.com/net/article/10-Socket-%EC%84%9C%EB%B2%84
    * https://blog.naver.com/ssod015/220282876968
    * https://blog.naver.com/PostView.naver?blogId=ssod015&logNo=220282802852&parentCategoryNo=&categoryNo=28&viewDate=&isShowPopularPosts=false&from=postView
    * https://stackoverflow.com/questions/4672010/multi-threading-with-net-httplistener
    */

    // 링크를 브라우저에 쳐보자! 
    // http://localhost:8000/search?word=talk
    public class HTTPProgram
    {
        const string INPUT_FILE = "./resources/war_and_peace.txt";
        const int NUMBER_OF_THREADS = 1;
        /* 1 첫번째로 호출 */
        public static void MajorAction()
        {
            try
            {
                var text = File.ReadAllText(INPUT_FILE);
                StartServer(text);
            }
            catch (IOException)
            {
            
            }
        }
        /* 2 두번째로 호출 */
        public static void StartServer(String text)
        {
            // HttpListener가 비동기 요청을 처리할 때, .NET의 스레드 풀에서 스레드를 자동으로 할당받아 사용합니다.
            /***
            APM 모델
                BeginXXX 메서드의 파라미터는 실제 수행할 작업 메서드가 필요한 파라미터로 먼저 채우고
                비동기 작업의 결과를 반환받을 AsyncCallback 타입의 델리게이트로 지정한다.

                EndXXX는 내부에 BeginXXX를 호출했던 실제 작업 수행 메서드의 델리게이트를 다시 가져와
                EndInvoke()를 호출하는 구조로 되어 있다.
            ***/

            /***
            public sealed unsafe partial class HttpListener {
                public static bool IsSupported => true;

                public HttpListenerTimeoutManager TimeoutManager

                public void Start()
                public void Stop()
                public void Abort()
                public bool UnsafeConnectionNtlmAuthentication

                public IAsyncResult BeginGetContext(AsyncCallback? callback, object? state)
                public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
                public HttpListenerContext GetContext()
            }
            ***/
            HttpListener listener = new HttpListener();
            WordCountHandler handler = new WordCountHandler(text, listener);
            // 클라이언트가 요청할 주소를 Prefix에 Add
            listener.Prefixes.Add("http://localhost:8000/search/");
            // Prefix에 추가된 URL로 요청을 기다리기 시작
            listener.Start();
            // GetContext는 호출하게 될떄 실제 요청이 들어올 때까지 스레드가 대기 상태가 된다.
            // 요청이 도착하게 된다면 context 객체를 반환한다.
            // 이 HttpListenerContext에는 Request / Response 객체가 들어있다. 
            // 특히 context.Request에는 대표적으로 URL, HttpMethod, InputStream, QueryString, ContentEncoding
            // var context = listener.BeginGetContext(handler.RequestCallback, listener);
            
            /***
            public sealed unsafe partial class HttpListenerContext
            {
                public HttpListenerRequest Request { get; }
                public HttpListenerResponse Response;
                public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string? subProtocol, TimeSpan keepAliveInterval);
            }
            ***/
            HttpListenerContext context = listener.GetContext(); // 여기서 바로 http://localhost:8000/search?word=talk 접속할 때까지 Request를 기다린다!!
            handler.ProcessRequest(context); // http://localhost:8000/search?word=talk 접속했으면 여기서 다시 수행된다.
            Console.WriteLine("Listening...");

        }
        
        public class WordCountHandler
        {
            private String text;
            private HttpListener httpListenerRef;
            
            public WordCountHandler(String text, HttpListener listenerRef)
            {
                this.text = text;
                this.httpListenerRef = listenerRef;
            }

            /* 3 세번째로 호출 */
            public void ProcessRequest(HttpListenerContext context)
            {
                string? action = context.Request.QueryString.GetKey(0);         // "word"
                string? word = context.Request.QueryString.GetValues(0)?[0];    // "talk"
                string responseString = "";
                byte[] response = null;
                if (action != null && !action.Equals("word"))
                {
                    responseString = "<html><body><h1>400 Bad Request</h1></body></html>";
                    response = Encoding.UTF8.GetBytes(responseString);
                    context.Response.StatusCode = 400; // HTTP 상태 코드를 400으로 설정
                    context.Response.ContentLength64 = 0;
                    // HttpListenerResponse 객체의 ContentType 속성을 "text/html"로 설정하여 
                    // 응답이 HTML 문서임을 나타냅니다. 
                    // 이 설정으로 브라우저는 응답을 페이지 내용으로 해석하고, 
                    // 사용자에게 HTML로 렌더링된 내용을 표시하게 됩니다.
                    context.Response.ContentType = "text/html"; 
                }
                else
                {
                    responseString = $"<html><body><h1>200 OK</h1><div>{CountWord(word).ToString()}</div></body></html>";
                    response = Encoding.UTF8.GetBytes(responseString);
                    context.Response.StatusCode = 200;
                    context.Response.ContentLength64 = response.Length;
                    context.Response.ContentType = "text/html";
                }

                using (Stream output = context.Response.OutputStream)
                {
                    if (response == null) { throw new Exception("버퍼가 없음"); }
                    // Write를 하게 되면 웹 브라우저가 갱신된다 
                    // image/WriteResult.png
                    output.Write(response, 0, response.Length);
                }
            }

            public void RequestCallback(IAsyncResult result)
            {
                var context = this.httpListenerRef.EndGetContext(result);
                this.httpListenerRef.BeginGetContext(RequestCallback, result);
                this.ProcessRequest(context);
            }

            /* 4 네번쨰로 호출 */
            private long CountWord(string word)
            {
                long count = 0;
                int index = 0;
                while (index >= 0)
                {
                    index = text.IndexOf(word, index);

                    if (index >= 0)
                    {
                        count++;
                        index++;
                    }
                }
                return count;
            }
        }
    }
}