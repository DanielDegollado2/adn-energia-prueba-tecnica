# Bitácora de Uso de IA

## Herramienta utilizada
Claude

---

## 1. Diseño de la capa de datos (SQL Server)

### Contexto
El reto requería diseñar el esquema de base de datos para el sistema de control de 
salidas y pesaje, migrando la lógica desde una Canvas App de Power Platform hacia 
una base de datos relacional en SQL Server, incluyendo un Stored Procedure 
transaccional para el flujo de autorización de salida.

### Cómo usé la IA
Utilicé la IA como herramienta de revisión y validación técnica: yo proponía el 
script (DDL y Stored Procedure) y la IA identificaba errores de sintaxis, problemas 
de diseño y oportunidades de mejora, explicando el porqué de cada corrección antes 
de que yo la aplicara.

### Decisiones técnicas tomadas con apoyo de IA

- **Diseño de la Primary Key**: Evalué usar `Folio_Despacho` (VARCHAR) como PK 
  directa. Con apoyo de la IA, analicé el trade-off de rendimiento (un índice 
  clustered sobre VARCHAR es menos eficiente que sobre un entero) y decidí usar 
  `Id INT IDENTITY(1,1)` como surrogate key, dejando `Folio_Despacho` como columna 
  `UNIQUE` para mantener la integridad de negocio sin sacrificar rendimiento.

- **INT vs UUID**: Consideré usar UUID para la PK. Descarté esta opción porque el 
  sistema opera sobre una única base de datos central, por lo que un `INT IDENTITY` 	
  es más eficiente en almacenamiento, comparación e indexado, sin necesidad de las garantías que aporta 
  un UUID en escenarios distribuidos.

- **Columna `Fecha_Hora_Salida`**: Inicialmente consideré `TIMESTAMP`, pero la IA 
  señaló que en SQL Server ese tipo no almacena fechas. Corregí a `DATETIME2`. Además, decidí que el valor 
  se asigna dentro del Stored Procedure con `GETDATE()` al momento de autorizar la 
  salida, en lugar de recibirlo como parámetro desde la app, para evitar depender 
  del reloj de la máquina cliente.


### Qué acepté tal cual / qué modifiqué
Acepté las explicaciones conceptuales (diferencias entre tipos de datos, 
trade-offs de PK) tal cual, ya que pude validarlas ejecutando el código y confirmando el comportamiento esperado en SSMS. 
Las decisiones de diseño (qué tipo de PK usar, qué precisión decimal mantener según la 
fuente de datos real) las tomé yo con base en la información de mi caso específico 
(pesos en toneladas, fuente de datos con un decimal).

## 2. Desarrollo de la aplicación de escritorio (.NET 10 / WPF / MVVM)

### Contexto
Desarrollo de una aplicación WPF en .NET 10 siguiendo el patrón MVVM, con dos 
pantallas principales: Dashboard (lista de camiones pendientes) y Auditoría 
(pesaje y autorización de salida).

### Cómo usé la IA
Utilicé la IA como guía arquitectónica y de resolución de errores: yo tomaba las 
decisiones de diseño y la IA explicaba las implicaciones técnicas de cada opción 
antes de implementarla.

### Decisiones técnicas tomadas con apoyo de IA

- **Navegación con ContentControl y Messenger**: En lugar de abrir múltiples 
  ventanas, se implementó navegación dentro de una sola `MainWindow` usando un 
  `ContentControl` enlazado a `VistaActual` en el `MainViewModel`. La comunicación 
  entre ViewModels se resolvió con `WeakReferenceMessenger` del toolkit, evitando 
  acoplamiento directo entre ellos.

- **DataTemplates para resolución de vistas**: Se definieron `DataTemplate` en 
  `MainWindow.xaml` que mapean automáticamente cada ViewModel a su UserControl 
  correspondiente.

- **`PuedeAutorizarSalida` como CanExecute**: El botón "Autorizar Salida" se 
  deshabilita automáticamente usando `[NotifyCanExecuteChangedFor]` en las 
  propiedades observables, garantizando que el usuario no pueda autorizar sin 
  ingresar un peso válido y, si aplica, una justificación.

### Qué acepté tal cual / qué modifiqué
Las explicaciones conceptuales (funcionamiento del Messenger, DataTemplates, 
CanExecute) las acepté como referencia para entender el patrón. Las 
implementaciones concretas las adapté según los errores reales que encontré 
durante el desarrollo, usando la IA para diagnosticar la causa raíz.

---

## 3. Pipeline CI/CD (GitHub Actions)

### Contexto
Configuración de un pipeline básico de integración y entrega continua para 
verificar que el proyecto compila correctamente en cada push al repositorio.

### Decisiones técnicas tomadas con apoyo de IA

- **GitHub Secrets para la cadena de conexión**: El `appsettings.json` se excluye 
  del repositorio mediante `.gitignore`. El pipeline genera el archivo 
  dinámicamente en cada ejecución usando el secret `CONNECTION_STRING`, evitando 
  exponer credenciales en el código fuente.
