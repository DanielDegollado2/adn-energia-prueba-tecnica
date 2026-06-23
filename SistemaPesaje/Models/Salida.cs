using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPesaje.Models
{
    public class Salida
    {
        public int Id { get; set; }
        public string FolioDespacho { get; set; }
        public string CentroOperativo { get; set; }
        public string PlacaTracto { get; set; }
        public string NombreConductor { get; set; }
        public decimal PesoTara { get; set; }
        public decimal PesoTeoricoERP { get; set; }
        public decimal? PesoBasculaSalida { get; set; }
        public decimal? PesoNetoReal { get; set; }
        public string? JustificacionDiferencia { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
    }
}
