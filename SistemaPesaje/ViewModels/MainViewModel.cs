using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;

namespace SistemaPesaje.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// ObservableProperty que guarda la pantalla actual
        /// </summary>
        [ObservableProperty]
        private object vistaActual;

        /// <summary>
        /// Inicializa la vista actual con DashboardViewModel
        /// Registra MainViewModel para poder recibir mensajes de tipo
        /// SalidaSeleccionadaMessage y NavegarDashboardMessage,
        /// al recibirlos se encarga de mostrar la pantalla correspondiente
        /// </summary>
        public MainViewModel()
        {
            VistaActual = new DashboardViewModel();

            WeakReferenceMessenger.Default.Register<MainViewModel, SalidaSeleccionadaMessage>(this, (r, m) =>
            {
                r.VistaActual = new AuditoriaViewModel(m.Value);
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, NavegarDashboardMessage>(this, (r, m) =>
            {
                r.VistaActual = new DashboardViewModel();
            });
        }
    }
}
