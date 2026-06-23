using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SistemaPesaje.Messages;
using SistemaPesaje.Models;
using SistemaPesaje.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SistemaPesaje.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Salida> salidasPendientes = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private Salida? salidaSeleccionada;

        public DashboardViewModel()
        {
            _databaseService = new DatabaseService();
            _ = CargarSalidasAsync();
        }

        [RelayCommand]
        private async Task CargarSalidasAsync()
        {
            IsLoading = true;
            try
            {
                var resultado = await _databaseService.GetSalidasPendientesAsync();
                SalidasPendientes = new ObservableCollection<Salida>(resultado);
            }
            catch (Exception ex)
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

        [RelayCommand]
        private void SeleccionarSalida(Salida salida)
        {
            if (salida is null) return;
            WeakReferenceMessenger.Default.Send(new SalidaSeleccionadaMessage(salida));
        }
    }
}
