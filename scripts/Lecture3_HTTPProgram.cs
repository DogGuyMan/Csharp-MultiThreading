using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Udemy.MultiThreading.Lecture3
{

    // 링크를 브라우저에 쳐보자! 
    // http://localhost:8000/search?word=talk
    public class HTTPProgram
    {
        const string INPUT_FILE = "./resources/war_and_peace.txt";
        const int NUMBER_OF_THREADS = 8;

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

        public static void StartServer(String text)
        {
            using (HttpServer httpServer = new HttpServer(IPAddress.Any, 8000)) {

                httpServer.SetMinWorker(NUMBER_OF_THREADS);
                // WordCountHandler handler = new WordCountHandler(text);
                // httpServer.ProcessRequest += handler.ProcessRequest;
                httpServer.Start().Wait();
                Console.WriteLine("Listening...");
            }
        }

        public class HttpServer : IDisposable {
            private int minWorker = -1; 
            private int minIOC = -1;
            private readonly HttpListener listener;

            public HttpServer(IPAddress address, int port) {

                listener = new HttpListener();
                listener.Prefixes.Add(String.Format(@"http://+:{0}/", port));
            }

            public void SetMinWorker(int minThreads) {
                if(minWorker == -1 ||  minIOC == -1) 
                    ThreadPool.GetMinThreads(out minWorker, out minIOC);
                minWorker = minThreads;
                ThreadPool.SetMinThreads(minThreads, minIOC);
            }

            public void Dispose()
            {
                listener.Stop();
            }

            public async Task Start() {
                listener.Start();

                while(true) {
                    var context = await Task.Factory.FromAsync<HttpListenerContext>(listener.BeginGetContext(RequestCallback, listener), listener.EndGetContext);
                    Task.Factory.StartNew(AsyncPrecessRequest, context);
                }
            }

            private void RequestCallback(IAsyncResult ar) {
                if(listener == null) return;
                if(!listener.IsListening) return;
                if(ar.AsyncState is HttpListener) {
                    HttpListener httpListener = (HttpListener)ar.AsyncState;
                    var context = httpListener.GetContext();
                    ProcessRequest(context);
                }
            }

            public void AsyncPrecessRequest(object? state) {
                if(state is HttpListenerContext) {
                    ProcessRequest((HttpListenerContext)state);
                }
            }
            
            public event Action<HttpListenerContext> ProcessRequest;
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

            public WordCountHandler(String text) : this(text, null) {

            }

            public void ProcessRequest(HttpListenerContext context)
            {
                string? action = context.Request.QueryString.GetKey(0);
                string? word = context.Request.QueryString.GetValues(0)?[0];
                byte[] response = null;
                if (action != null && !action.Equals("word"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentLength64 = 0;
                }
                else
                {
                    response = Encoding.UTF8.GetBytes(CountWord(word).ToString());
                    context.Response.StatusCode = 200;
                    context.Response.ContentLength64 = response.Length;
                }

                using (Stream output = context.Response.OutputStream)
                {
                    if (response == null) { throw new Exception("버퍼가 없음"); }
                    // Write를 하게 되면 웹 브라우저가 갱신된다 
                    // image/WriteResult.png
                    output.Write(response, 0, response.Length);
                }
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