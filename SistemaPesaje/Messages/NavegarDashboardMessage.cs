using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPesaje.Messages
{
    public class NavegarDashboardMessage : ValueChangedMessage<bool>
    {
        public NavegarDashboardMessage() : base(true) { }
    }
}
