using CommunityToolkit.Mvvm.Messaging.Messages;
using SistemaPesaje.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPesaje.Messages
{
    public class SalidaSeleccionadaMessage : ValueChangedMessage<Salida>
    {
        public SalidaSeleccionadaMessage(Salida salida) : base(salida) { }
    }
}
