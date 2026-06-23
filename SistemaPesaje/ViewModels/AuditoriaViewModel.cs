using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;
using SistemaPesaje.Models;
using SistemaPesaje.Services;
using System.Windows;

namespace SistemaPesaje.ViewModels
{
    /// <summary>
    /// View model que se encarga de la lógica para realizar la auditoria de pesaje
    /// </summary>
    public partial class AuditoriaViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        private readonly Salida _salidaSeleccionada;
       
        public string PlacaTracto => _salidaSeleccionada.PlacaTracto;
        public string NombreConductor => _salidaSeleccionada.NombreConductor;
        public decimal PesoTeoricoERP => _salidaSeleccionada.PesoTeoricoERP;

        private decimal PesoTara => _salidaSeleccionada.PesoTara;

        /// <summary>
        /// ObservableProperty que recibe el peso de la carga mas el peso del camion
        /// Cuando se modifica esta propiedad, mediante NotifyPropertyChangedFor se ejecutan las funciones para calcular
        /// el Peso Neto Real y si la diferencia es mayor al 3% para indicar si se necesita justificar.
        /// Cada vez que se modifique esta propiedad  [NotifyCanExecuteChangedFor(ActualizarSalidaCommand)]: reevalúa si el botón "Autorizar Salida" 
        ///  debe habilitarse o deshabilitarse, según si el peso es válido y la justificación fue ingresada (si aplica).
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PesoNetoReal))]
        [NotifyPropertyChangedFor(nameof(RequiereJustificacion))]
        [NotifyCanExecuteChangedFor(nameof(ActualizarSalidaCommand))]
        private string pesoBasculaSalidaTexto = string.Empty;

        /// <summary>
        /// Justificación requerida cuando la diferencia entre el peso neto real y el peso teórico ERP supera el 3%.
        /// El campo se vuelve visible y obligatorio en la UI cuando RequiereJustificacion es true.
        /// 
        /// Al modificarse esta propiedad, [NotifyCanExecuteChangedFor(ActualizarSalidaCommand)] reevalúa 
        /// si el botón "Autorizar Salida" debe habilitarse, garantizando que el operador no pueda 
        /// autorizar la salida sin haber escrito una justificación cuando esta es requerida.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ActualizarSalidaCommand))]
        private string? justificacionDiferencia;

        /// <summary>
        /// Se encarga de convertir el texto introducido a su valor decimal.
        /// Verifica que sea un numero valido para retornar el valor, si no lo es
        /// retorna 0
        /// </summary>
        public decimal PesoBasculaSalidaValor =>
            decimal.TryParse(PesoBasculaSalidaTexto, out var result) ? result : 0m;

        /// <summary>
        /// Calcula el peso real de la carga restando el peso al momento de salir al peso del camion vacio.
        /// Verifica que el usuario haya escrito algun valor para realizar la resta
        /// </summary>
        public decimal PesoNetoReal => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) ? PesoBasculaSalidaValor - PesoTara : 0;

        /// <summary>
        /// Verifica si se requiere que el usuario provea una justificacion por la diferencia de pesos.
        /// Realiza el calculo para verificar si la diferencia entre el peso neto real y el peso teorico ERP es mayor al 3% del peso teorico ERP
        /// </summary>
        public bool RequiereJustificacion => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) &&
            decimal.TryParse(PesoBasculaSalidaTexto, out _) &&
            Math.Abs(PesoNetoReal - PesoTeoricoERP) > PesoTeoricoERP * 0.03m;

        /// <summary>
        /// Determina si el botón "Autorizar Salida" debe estar habilitado.
        /// Se requiere que:
        /// 1. El campo de peso báscula no esté vacío.
        /// 2. El valor ingresado sea un número decimal válido.
        /// 3. Si la diferencia con el peso teórico supera el 3%, la justificación debe estar escrita.
        /// </summary>
        private bool PuedeAutorizarSalida => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) &&
            decimal.TryParse(PesoBasculaSalidaTexto, out _) && (!RequiereJustificacion || !string.IsNullOrWhiteSpace(JustificacionDiferencia));

        /// <summary>
        /// Constructor que inicializa el servicio de la base de datos
        /// y la salida que fue seleccionada en el dashboard
        /// </summary>
        /// <param name="salidaSeleccionada"></param>
        public AuditoriaViewModel(Salida salidaSeleccionada)
        {
            _databaseService = new DatabaseService();
            _salidaSeleccionada = salidaSeleccionada;
        }

        /// <summary>
        /// Command para regresar a la pantalla dashboard
        /// Emite un mensaje para avisar a MainViewModel que se realice esa modificacion de pantalla
        /// </summary>
        [RelayCommand]
        private void Regresar()
        {
            WeakReferenceMessenger.Default.Send(new NavegarDashboardMessage());
        }

        /// <summary>
        /// Ejecuta el metodo AutorizarSalida de el servicio de base de datos
        /// Se envuelve en un bloque try-catch, si la conexión a la base de datos
        /// se ejecuta correctamente, se generara un MessageBox mostrando un mensaje de exito
        /// junto con información de la salida aprobada.
        /// Se emite un mensaje para volver al dashboard una vez se cierre el mensaje de exito.
        /// Si la conexión falla, el catch atrapa esa excepcion, la loggea llamando al logservice y emite
        /// un MessageBox con un mensaje de error amigable para el usuario
        /// Se coloca un CanExecute que toma como referencia la funcion PuedeAutorizarSalida que decide si
        /// el boton al que se hizo Binding este command puede tener interacción del usuario
        /// </summary>
        [RelayCommand(CanExecute = nameof(PuedeAutorizarSalida))]
        private async Task ActualizarSalidaAsync()
        {

            try
            {
                await _databaseService.AutorizarSalidaAsync(_salidaSeleccionada.Id, PesoBasculaSalidaValor, PesoNetoReal, JustificacionDiferencia);

                MessageBox.Show(
                    $"Salida autorizada exitosamente.\n\n" +
                    $"Folio: {_salidaSeleccionada.FolioDespacho}\n" +
                    $"Placa: {_salidaSeleccionada.PlacaTracto}\n" +
                    $"Conductor: {_salidaSeleccionada.NombreConductor}\n" +
                    $"Peso Neto Real: {PesoNetoReal} ton",
                    "Salida autorizada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                WeakReferenceMessenger.Default.Send(new NavegarDashboardMessage());
            }
            catch(Exception ex) 
            {
                LogService.LogError("Error al autorizar salida del folio: ", ex);
                MessageBox.Show(
                    $"No se pudo conectar con la base de datos. Verifica tu conexión e intenta de nuevo.\n\nDetalle: {ex.Message}",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
