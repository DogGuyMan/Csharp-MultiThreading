using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading;
namespace Udemy.MultiThreading.Lecture3
{
    /*
    * https://csharpstudy.com/net/article/10-Socket-%EC%84%9C%EB%B2%84
    * https://blog.naver.com/ssod015/220282876968
    * https://blog.naver.com/PostView.naver?blogId=ssod015&logNo=220282802852&parentCategoryNo=&categoryNo=28&viewDate=&isShowPopularPosts=false&from=postView
    * https://stackoverflow.com/questions/4672010/multi-threading-with-net-httplistener
    */

    // http://localhost:8000/search?word=talk
    public class HTTPProgram
    {
        const string INPUT_FILE = "./resources/war_and_peace.txt";
        const int NUMBER_OF_THREADS = 1;

        public static void Main(string[] args)
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
        public static void StartServer(String text)
        {
            //HttpListener가 비동기 요청을 처리할 때, .NET의 스레드 풀에서 스레드를 자동으로 할당받아 사용합니다.
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
            var context = listener.GetContext();
            handler.ProcessRequest(context);
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

            public void ProcessRequest(HttpListenerContext context)
            {
                string? action = context.Request.QueryString.GetKey(0);
                string? word = context.Request.QueryString.GetValues(0)?[0];
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
                    output.Write(response, 0, response.Length);
                }
            }

            public void RequestCallback(IAsyncResult result)
            {
                var context = this.httpListenerRef.EndGetContext(result);
                this.httpListenerRef.BeginGetContext(RequestCallback, result);
                this.ProcessRequest(context);
            }
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