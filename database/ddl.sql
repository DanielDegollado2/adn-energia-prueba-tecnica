CREATE TABLE Salidas (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Folio_Despacho VARCHAR(20) NOT NULL UNIQUE,
	Centro_Operativo VARCHAR(50) NOT NULL,
	Placa_Tracto VARCHAR(10) NOT NULL,
	Nombre_Conductor VARCHAR(50) NOT NULL,
	Peso_Tara DECIMAL(5,1) NOT NULL,
	Peso_Teorico_ERP DECIMAL(5,1) NOT NULL,
	Peso_Bascula_Salida DECIMAL(5,1) NULL,
	Peso_Neto_Real DECIMAL(5,1) NULL,
	Justificacion_Diferencia VARCHAR(MAX) NULL,
	Fecha_Hora_Salida DATETIME2 NULL
);