using SistemaPesaje.Models;
using SistemaPesaje.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DashboardViewModel;
            vm?.CargarSalidasCommand.Execute(null);
        }

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
