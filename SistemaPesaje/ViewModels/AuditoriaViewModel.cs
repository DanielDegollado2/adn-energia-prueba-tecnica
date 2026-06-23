using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;
using SistemaPesaje.Models;
using SistemaPesaje.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaPesaje.ViewModels
{
    public partial class AuditoriaViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        private readonly Salida _salidaSeleccionada;

        public string PlacaTracto => _salidaSeleccionada.PlacaTracto;
        public string NombreConductor => _salidaSeleccionada.NombreConductor;
        public decimal PesoTeoricoERP => _salidaSeleccionada.PesoTeoricoERP;

        private decimal PesoTara => _salidaSeleccionada.PesoTara;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PesoNetoReal))]
        [NotifyPropertyChangedFor(nameof(RequiereJustificacion))]
        [NotifyCanExecuteChangedFor(nameof(ActualizarSalidaCommand))]
        private string pesoBasculaSalidaTexto = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ActualizarSalidaCommand))]
        private string? justificacionDiferencia;

        [ObservableProperty]
        private bool isLoading;

        public decimal PesoBasculaSalidaValor =>
            decimal.TryParse(PesoBasculaSalidaTexto, out var result) ? result : 0m;

        public decimal PesoNetoReal => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) ? PesoBasculaSalidaValor - PesoTara : 0;
        public bool RequiereJustificacion => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) &&
            decimal.TryParse(PesoBasculaSalidaTexto, out _) &&
            Math.Abs(PesoNetoReal - PesoTeoricoERP) > PesoTeoricoERP * 0.03m;

        private bool PuedeAutorizarSalida => !string.IsNullOrWhiteSpace(PesoBasculaSalidaTexto) &&
            decimal.TryParse(PesoBasculaSalidaTexto, out _) && !RequiereJustificacion || !string.IsNullOrWhiteSpace(JustificacionDiferencia);

        public AuditoriaViewModel(Salida salidaSeleccionada)
        {
            _databaseService = new DatabaseService();
            _salidaSeleccionada = salidaSeleccionada;
        }

        [RelayCommand]
        private void Regresar()
        {
            WeakReferenceMessenger.Default.Send(new NavegarDashboardMessage());
        }

        [RelayCommand(CanExecute = nameof(PuedeAutorizarSalida))]
        private async Task ActualizarSalidaAsync()
        {
            IsLoading = true;

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
                MessageBox.Show(
                    $"No se pudo conectar con la base de datos. Verifica tu conexión e intenta de nuevo.\n\nDetalle: {ex.Message}",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
