using System.IO;

namespace SistemaPesaje.Services
{
    /// <summary>
    /// Servicio para generar archivo donde se registran los errores técnicos de conexión
    /// </summary>
    public static class LogService
    {
        private static readonly string _logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        /// <summary>
        /// Define el formato en el que se mostrara el mensaje de error y lo agrega a el archivo log.txt
        /// </summary>
        /// <param name="mensaje">Mensaje de error</param>
        /// <param name="ex">Parametro para acceder a propiedades de la excepcion</param>
        public static void LogError(string mensaje, Exception ex)
        {
            try
            {
                var entrada = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {mensaje}{Environment.NewLine}" +
                              $"Excepción: {ex.GetType().Name}{Environment.NewLine}" +
                              $"Mensaje: {ex.Message}{Environment.NewLine}" +
                              $"StackTrace: {ex.StackTrace}{Environment.NewLine}" +
                              $"{"─".PadRight(80, '─')}{Environment.NewLine}";

                File.AppendAllText(_logPath, entrada);
            }
            catch
            {
               
            }
        }
    }
}
