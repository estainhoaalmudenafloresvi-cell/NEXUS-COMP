# Análisis técnico de NEXUS

## 1. Objetivo

El análisis compara las decisiones técnicas del proyecto y explica por qué se utiliza Windows Forms para la interfaz, GameEngine para las reglas y SQL Server junto con JSON para la persistencia.

## 2. Interfaz: consola o Windows Forms

### Alternativa A: mantener la consola

Ventajas:
- Conserva directamente la forma de interacción del proyecto H3.
- Es sencilla para probar entradas y reglas básicas.

Desventajas:
- Limita la presentación visual del escenario.
- No permite aprovechar de la misma forma el ratón, los paneles y los elementos gráficos de escritorio.

### Alternativa B: Windows Forms

Ventajas:
- Permite crear ventanas, botones, diálogos y controles.
- Puede recibir eventos de teclado y ratón.
- Permite dibujar el escenario 2D y el HUD.

Desventajas:
- Requiere adaptar la presentación y el ciclo de actualización del juego.
- El dibujo y los eventos deben coordinarse para mantener una respuesta fluida.

**Decisión:** la versión gráfica de NEXUS utiliza Windows Forms sobre .NET 8. La aplicación ya implementa el menú, el selector de planetas, el área de juego, la pausa y la Armería.

## 3. Organización de la lógica

### Alternativa A: concentrar toda la lógica en la ventana

Ventaja:
- Puede ser rápida para una demostración pequeña.

Desventaja:
- Mezcla eventos, reglas, estado y persistencia, lo que complica el mantenimiento.

### Alternativa B: separar responsabilidades

Ventajas:
- MainForm gestiona la interfaz.
- GameEngine controla las reglas y la actualización del mundo.
- GameState representa los datos de la partida.
- SqlSaveRepository concentra las consultas SQL.

Desventajas:
- Requiere definir con claridad qué clase es responsable de cada operación.

**Decisión:** el código actual utiliza esta separación. Es la opción más conveniente para ampliar el juego sin trasladar las reglas a los formularios.

## 4. Persistencia: JSON o SQL Server

### Alternativa A: archivo JSON

Ventajas:
- Es sencillo de serializar y recuperar con System.Text.Json.
- Permite guardar localmente sin depender de un servidor de base de datos.
- Ya está integrado en GameEngine y MainForm.

Desventajas:
- No facilita consultas relacionales de perfiles y múltiples partidas.
- La organización de la información depende de la estructura del archivo.

### Alternativa B: SQL Server

Ventajas:
- Organiza perfiles, partidas, misiones y mejoras en tablas.
- Permite consultar y recuperar partidas.
- Ofrece restricciones y transacciones para mantener la consistencia de los datos.

Desventajas:
- Requiere instalar o disponer de una instancia SQL Server.
- Es necesario configurar la cadena de conexión y ejecutar el script.

**Decisión:** se mantienen ambos mecanismos con funciones distintas. JSON sirve para el guardado local y SQL Server para el almacenamiento estructurado de perfiles y partidas.

## 5. Acceso a datos

La versión actual utiliza Microsoft.Data.SqlClient. SqlSaveRepository crea conexiones SqlConnection, ejecuta consultas mediante SqlCommand, define parámetros tipados y lee resultados con SqlDataReader.

Al guardar una partida, se utiliza una transacción para insertar el registro principal, el progreso de misiones y las mejoras. Esta decisión evita confirmar solo una parte de los datos si una de las operaciones falla.

## 6. Alternativas de actualización gráfica

El proyecto utiliza dibujo 2D con System.Drawing dentro de Windows Forms. Esta opción mantiene el alcance en una aplicación de escritorio sencilla y no requiere diseñar una escena 3D ni introducir un motor gráfico externo.

El temporizador de la ventana actualiza el motor y solicita el redibujado del área de juego. La interfaz también utiliza eventos de teclado y ratón para recibir las acciones.

## 7. Decisiones técnicas actuales

| Área | Decisión | Justificación |
|---|---|---|
| Lenguaje | C# | Mantiene continuidad con H3 |
| Plataforma | .NET 8 para Windows | Compatible con el proyecto Windows Forms |
| Interfaz | Windows Forms | Ventanas, controles y eventos |
| Dibujo | System.Drawing | Escenarios 2D y HUD |
| Lógica | GameEngine y GameState | Separa reglas y estado |
| Persistencia local | System.Text.Json | Guardado sin servidor |
| Persistencia relacional | SQL Server | Perfiles y partidas estructurados |
| Acceso SQL | Microsoft.Data.SqlClient | Conexión, comandos y lectura de datos |

## 8. Fuentes técnicas

- Microsoft Learn, Windows Forms para .NET: https://learn.microsoft.com/dotnet/desktop/winforms/
- Microsoft Learn, ADO.NET y SQL Server: https://learn.microsoft.com/sql/connect/ado-net/
- Microsoft Learn, System.Text.Json: https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview

Las fuentes se utilizan para comprobar las características generales de las tecnologías. La descripción de las funciones implementadas se obtuvo revisando los archivos del propio repositorio.

## 9. Verificación y uso de inteligencia artificial

La inteligencia artificial puede ayudar a ordenar la documentación y comparar alternativas, pero no sustituye la revisión del código ni las pruebas. Las afirmaciones sobre la versión actual se contrastaron con MainForm.cs, GameEngine.cs, SqlSaveRepository.cs, NEXUS.csproj y CrearBaseNEXUS.sql.

Para verificar el proyecto en el equipo se recomienda:
1. Abrir NEXUS.sln y compilar con .NET 8 en Windows.
2. Ejecutar las cuatro expediciones y comprobar sus objetivos.
3. Probar el guardado y la recuperación local.
4. Ejecutar el script SQL en una instancia de SQL Server.
5. Probar el guardado y la carga SQL y revisar los registros creados.
6. Confirmar que los errores de conexión se muestran sin cerrar inesperadamente el juego.

No se afirma que estas pruebas se hayan realizado en todos los equipos. La conexión SQL depende de la configuración local.

## 10. Conclusión

La combinación actual de C#, Windows Forms, GameEngine, System.Text.Json y SQL Server permite continuar el desarrollo a partir del proyecto H3. La separación entre interfaz, lógica y persistencia facilita mantener el código y añadir mejoras gradualmente.
