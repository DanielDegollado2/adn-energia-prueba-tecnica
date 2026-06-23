using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using SistemaPesaje.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SistemaPesaje.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json.");
        }

        public async Task<List<Salida>> GetSalidasPendientesAsync()
        {
            var salidas = new List<Salida>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetSalidasPendientes", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                salidas.Add(new Salida
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FolioDespacho = reader.GetString(reader.GetOrdinal("Folio_Despacho")),
                    CentroOperativo = reader.GetString(reader.GetOrdinal("Centro_Operativo")),
                    PlacaTracto = reader.GetString(reader.GetOrdinal("Placa_Tracto")),
                    NombreConductor = reader.GetString(reader.GetOrdinal("Nombre_Conductor")),
                    PesoTara = reader.GetDecimal(reader.GetOrdinal("Peso_Tara")),
                    PesoTeoricoERP = reader.GetDecimal(reader.GetOrdinal("Peso_Teorico_ERP"))
                });
            }

            return salidas;
        }

        public async Task AutorizarSalidaAsync(int id, decimal pesoBasculaSalida, decimal pesoNetoReal, string? justificacionDiferencia)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateSalidas", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Peso_Bascula_Salida", pesoBasculaSalida);
            command.Parameters.AddWithValue("@Peso_Neto_Real", pesoNetoReal);
            command.Parameters.AddWithValue("@Justificacion_Diferencia", (object?)justificacionDiferencia ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
