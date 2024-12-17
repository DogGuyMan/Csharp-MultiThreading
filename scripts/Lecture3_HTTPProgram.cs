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
        const int NUMBER_OF_THREADS = 2;

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
            HttpServer httpServer = new HttpServer(NUMBER_OF_THREADS);
                WordCountHandler handler = new WordCountHandler(text);
                httpServer.ProcessRequest += handler.ProcessRequest;
                httpServer.Start(8000);
                Console.WriteLine("Listening...");
            
        }


        class HttpServer : IDisposable
        {
            private readonly int _maxThreads;
            private readonly HttpListener _listener;
            private readonly Thread _listenerThread;
            private readonly ManualResetEvent _stop, _idle;
            private readonly Semaphore _busy;

            public HttpServer(int maxThreads)
            {
                _maxThreads = maxThreads;
                _stop = new ManualResetEvent(false);
                _idle = new ManualResetEvent(false);
                _busy = new Semaphore(maxThreads, maxThreads);
                _listener = new HttpListener();
                _listenerThread = new Thread(HandleRequests);
            }

            public void Start(int port)
            {
                _listener.Prefixes.Add(String.Format(@"http://+:{0}/", port));
                _listener.Start();
                _listenerThread.Start();
            }

            public void Dispose()
            { Stop(); }

            public void Stop()
            {
                _stop.Set();
                _listenerThread.Join();
                _idle.Reset();

                //aquire and release the semaphore to see if anyone is running, wait for idle if they are.
                _busy.WaitOne();
                if (_maxThreads != 1 + _busy.Release())
                    _idle.WaitOne();

                _listener.Stop();
            }

            private void HandleRequests()
            {
                while (_listener.IsListening)
                {
                    var context = _listener.BeginGetContext(ListenerCallback, null);

                    if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }))
                        return;
                }
            }

            private void ListenerCallback(IAsyncResult ar)
            {
                _busy.WaitOne();
                try
                {
                    HttpListenerContext context;
                    try
                    { context = _listener.EndGetContext(ar); }
                    catch (HttpListenerException)
                    { return; }

                    if (_stop.WaitOne(0, false))
                        return;

                    ProcessRequest(context);
                }
                finally
                {
                    if (_maxThreads == 1 + _busy.Release())
                        _idle.Set();
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

            public WordCountHandler(String text) : this(text, null)
            {

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