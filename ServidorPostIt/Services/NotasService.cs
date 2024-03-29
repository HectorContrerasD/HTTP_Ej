using ServidorPostIt.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Automation;

namespace ServidorPostIt.Services
{
    public class NotasService
    {
        HttpListener server = new();
        public event EventHandler<Notas>? NotaRecibida;
        public NotasService()
        {
            server.Prefixes.Add("http://*:12345/notas/");
        }
        public void Iniciar()
        {
            if (!server.IsListening)
            {
                server.Start();
                new Thread(Escuchar)
                {
                    IsBackground = true
                }.Start();
            }
        }
        public void Detener()
        {
            server.Stop();
        }
        void Escuchar()
        {
            while (true)
            {
                var context = server.GetContext();
                var pagina = File.ReadAllText("assets/index.html");
                var buffpagina = Encoding.UTF8.GetBytes(pagina);
                if (context.Request.Url != null)
                {
                    if (context.Request.Url.LocalPath=="/notas/")
                    {
                        context.Response.ContentLength64 = buffpagina.Length;
                        context.Response.OutputStream.Write(buffpagina, 0, buffpagina.Length);
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath=="/notas/crear")
                    {
                        byte[] buffer = new byte[context.Request.ContentLength64];
                        context.Request.InputStream.Read(buffer,0, buffer.Length);
                        string datos= Encoding.UTF8.GetString(buffer);
                        
                       
                        var diccionario = HttpUtility.ParseQueryString(datos);
                        

                        Notas nota = new()
                        {
                            Titulo = diccionario["titulo"]??"",
                            Contenido = diccionario["contenido"]??"",
                            X= double.Parse(diccionario["x"] ?? "0"),
                            Y = double.Parse(diccionario["y"]??"0"),
                            Remitente = Dns.GetHostEntry(context.Request.RemoteEndPoint.Address).HostName,
                            
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            NotaRecibida?.Invoke(this,nota);
                        }

                        );
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();

                    }
                }
            }
            
        }
    }
}
