using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BXM.HttpServer
{
    public class SimpleHttpServer : HttpServer {
        public SimpleHttpServer(int port)
            : base(port) {
        }
        public override void handleGETRequest(HttpProcessor p) {
            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);

            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form>");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
            string data = inputData.ReadToEnd();

            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }
    }

    //public class TestMain {
    //    public static int Main(String[] args) {
    //        HttpServer httpServer;
    //        if (args.GetLength(0) > 0) {
    //            httpServer = new SimpleHttpServer(Convert.ToInt16(args[0]));
    //        } else {
    //            httpServer = new SimpleHttpServer(8080);
    //        }
    //        Thread thread = new Thread(new ThreadStart(httpServer.listen));
    //        thread.Start();
    //        return 0;
    //    }
    //}

}



