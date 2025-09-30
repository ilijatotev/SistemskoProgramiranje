using System;
using System.Net;
using System.Threading;

public class SimpleWebServer
{
    private readonly HttpListener listener;
    private readonly RequestHandler requestHandler;
    private readonly Logger logger;
    private readonly string url;

    public SimpleWebServer(string url, RequestHandler requestHandler, Logger logger)
    {
        this.url = url;
        this.requestHandler = requestHandler;
        this.logger = logger;
        this.listener = new HttpListener();
        this.listener.Prefixes.Add(this.url);
    }

    public void Start()
    {
        listener.Start();
        logger.Log($"Server pokrenut na adresi {url}");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ThreadPool.QueueUserWorkItem(requestHandler.HandleRequest, context);
        }
    }
}