using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPesaje.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object vistaActual;

        public MainViewModel()
        {
            VistaActual = new DashboardViewModel();

            WeakReferenceMessenger.Default.Register<MainViewModel, SalidaSeleccionadaMessage>(this, (r, m) =>
            {
                r.VistaActual = new AuditoriaViewModel(m.Value);
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, NavegarDashboardMessage>(this, (r, m) =>
            {
                var dashboardViewModel = new DashboardViewModel();
                r.VistaActual = new DashboardViewModel();
            });
        }
    }
}
