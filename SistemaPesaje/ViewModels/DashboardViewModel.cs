using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;
using SistemaPesaje.Models;
using SistemaPesaje.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaPesaje.ViewModels
{
    /// <summary>
    /// ViewModel que se encarga de la lógica para mostrarle las salidas pendientes
    /// al usuario
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// ObservableProperty que guarda las salidas pendientes
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Salida> salidasPendientes = new();

        /// <summary>
        /// ObservableProperty que guarda la salida seleccionada
        /// por el usuario
        /// </summary>
        [ObservableProperty]
        private Salida? salidaSeleccionada;

        /// <summary>
        /// Inicializa el servicio de la base de datos
        /// y ejecuta el Command CargarSalidas al momento de que
        /// cargue la pantalla
        /// </summary>
        public DashboardViewModel()
        {
            _databaseService = new DatabaseService();
            _ = CargarSalidasAsync();
        }

        /// <summary>
        /// Command que llama al metodo GetSalidasPendientes del servicio de base de datos
        /// Esta envuelto en un bloque try-catch, si la conexión se ejecuta correctamente,
        /// guarda el resultado en un ObservableCollection que se mostrara al usuario en el UI
        /// Si la conexión falla, se loggea el error llamando al logservice y se muestra un
        /// MessageBox con un mensaje de error amigable para el usuario
        /// </summary>
        [RelayCommand]
        private async Task CargarSalidasAsync()
        {
            try
            {
                var resultado = await _databaseService.GetSalidasPendientesAsync();
                SalidasPendientes = new ObservableCollection<Salida>(resultado);
            }
            catch (Exception ex)
            {
                LogService.LogError("Error al cargar las salidas pendientes", ex);
                MessageBox.Show(
                    $"No se pudo conectar con la base de datos. Verifica tu conexión e intenta de nuevo.\n\nDetalle: {ex.Message}",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Comando que se ejecuta cuando el usuario hace doble clic en una fila del Dashboard.
        /// Verifica que el objeto Salida seleccionado no sea nulo y emite un mensaje 
        /// mediante WeakReferenceMessenger para notificar al MainViewModel que debe 
        /// navegar a la pantalla de Auditoría con los datos del camión seleccionado.
        /// </summary>
        /// <param name="salida">Objeto Salida correspondiente a la fila seleccionada en el DataGrid.</param>
        [RelayCommand]
        private void SeleccionarSalida(Salida salida)
        {
            if (salida is null) return;
            WeakReferenceMessenger.Default.Send(new SalidaSeleccionadaMessage(salida));
        }
    }
}
