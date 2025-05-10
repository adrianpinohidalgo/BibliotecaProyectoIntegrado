using BibliotecaProyectoIntegrado.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class LibroActualizadoMessage : ValueChangedMessage<Libro>
{
    public LibroActualizadoMessage(Libro value) : base(value) { }
}
