CREATE PROCEDURE usp_UpdateSalidas
  @Id INT,
  @Peso_Bascula_Salida DECIMAL(5,1),
  @Peso_Neto_Real DECIMAL(5,1),
  @Justificacion_Diferencia VARCHAR(MAX) = NULL
AS
BEGIN
  BEGIN TRY
    IF NOT EXISTS (
        SELECT Id FROM Salidas WHERE Id = @Id
    )
    BEGIN
        THROW 50001, 'Esa Id no existe', 1;
    END;

    IF EXISTS (
        SELECT Id FROM Salidas WHERE Id = @Id AND Fecha_Hora_Salida IS NOT NULL
    )
    BEGIN
        THROW 50002, 'Esa salida ya fue autorizada', 1;
    END;

    BEGIN TRANSACTION;

    UPDATE Salidas
    SET Peso_Bascula_Salida = @Peso_Bascula_Salida,
        Peso_Neto_Real = @Peso_Neto_Real,
        Justificacion_Diferencia = @Justificacion_Diferencia,
        Fecha_Hora_Salida = GETDATE()
    WHERE Id = @Id;

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrMsg NVARCHAR(4000), @ErrSeverity INT;
    SELECT @ErrMsg = ERROR_MESSAGE(), @ErrSeverity = ERROR_SEVERITY();
    RAISERROR(@ErrMsg, @ErrSeverity, 1);
  END CATCH
END;