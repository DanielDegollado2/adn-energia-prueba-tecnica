CREATE PROCEDURE usp_GetSalidasPendientes
AS
BEGIN
	SELECT Id, Folio_Despacho, Centro_Operativo, Placa_Tracto, Nombre_Conductor, Peso_Tara, Peso_Teorico_ERP
	FROM Salidas
	WHERE Fecha_Hora_Salida IS NULL
END;