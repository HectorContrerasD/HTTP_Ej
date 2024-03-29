using ServidorPostIt.Models;
using ServidorPostIt.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServidorPostIt.ViewModels
{
    public class NotasViewModel : INotifyPropertyChanged
    {
        NotasService server = new();
        public ObservableCollection<Notas>ListaNotas { get; set; } = new ObservableCollection<Notas>();
        public string IP
        {
            get
            {
                return string.Join("", Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(x => x.ToString()));
            }
        }
        public NotasViewModel()
        {
            server.NotaRecibida += Server_NotaRecibida;
            server.Iniciar();
        }
        Random r = new();

        private void Server_NotaRecibida(object? sender, Notas e)
        {
           e.Angulo= r.Next(-5,6);
            ListaNotas.Add(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
