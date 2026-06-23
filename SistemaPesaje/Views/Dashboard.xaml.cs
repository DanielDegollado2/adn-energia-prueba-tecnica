using SistemaPesaje.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaPesaje.Views
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Se ejecuta cuando el UserControl termina de cargarse.
        /// Dispara el comando CargarSalidas para obtener los camiones 
        /// pendientes de pesaje desde la base de datos.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DashboardViewModel;
            vm?.CargarSalidasCommand.Execute(null);
        }

        /// <summary>
        /// Se ejecuta cuando el usuario hace doble clic en una fila del DataGrid.
        /// Verifica que haya una fila seleccionada y dispara el comando SeleccionarSalida
        /// para navegar a la pantalla de Auditoría con los datos del camión seleccionado.
        /// </summary>
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as DashboardViewModel;
            if (vm?.SalidaSeleccionada != null)
            {
                vm.SeleccionarSalidaCommand.Execute(vm.SalidaSeleccionada);
            }
        }
    }
}
