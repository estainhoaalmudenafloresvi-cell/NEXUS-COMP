# Arquitectura propuesta y evolución de NEXUS

## 1. Objetivo

La arquitectura de NEXUS debe mantener separadas la interfaz gráfica, las reglas del juego y la persistencia. La versión gráfica ya contiene una primera implementación de esta separación. Este documento describe esa estructura y las mejoras que pueden realizarse sin comenzar el proyecto desde cero.

## 2. Tecnologías seleccionadas

- C# y .NET 8.
- Windows Forms para la interfaz de escritorio.
- System.Drawing para representar el escenario 2D.
- SQL Server para almacenar perfiles y partidas.
- Microsoft.Data.SqlClient para el acceso a datos.
- System.Text.Json para el guardado local y la serialización del inventario.

## 3. Capas del sistema

### 3.1. Presentación — MainForm

Es la capa que recibe las acciones de la jugadora y muestra los resultados. Incluye el menú, el selector de planetas, el área de juego, el HUD, la pausa, la Armería y el Archivo.

La interfaz no debería contener las reglas de los objetivos ni las operaciones SQL directamente. Debe delegar esas tareas en GameEngine y SqlSaveRepository.

### 3.2. Lógica del juego — GameEngine

Controla el estado de la partida, las cuatro expediciones, los objetivos, los enemigos, los recursos, las habilidades y las mejoras.

GameState mantiene los datos de la exploradora que deben guardarse y recuperarse. Las clases auxiliares representan enemigos, objetos recogibles y elementos del escenario.

### 3.3. Acceso a datos — SqlSaveRepository

Centraliza las consultas y modificaciones de SQL Server. Recibe o devuelve objetos GameState y evita que MainForm tenga que construir consultas SQL.

El repositorio utiliza parámetros para las consultas y una transacción al guardar una partida junto con el progreso de misiones y las mejoras.

### 3.4. Almacenamiento

Se mantienen dos mecanismos con propósitos distintos:
- **JSON local:** permite guardar automáticamente una copia en el equipo.
- **SQL Server:** permite guardar perfiles y partidas en tablas y recuperar la última partida registrada.

## 4. Diagrama de arquitectura

\`\`\`mermaid
flowchart TD
    U[Jugadora] --> UI[MainForm - Windows Forms]
    UI --> GE[GameEngine]
    GE --> GS[GameState]
    UI --> SR[SqlSaveRepository]
    GE --> JS[System.Text.Json - guardado local]
    SR --> DB[(SQL Server)]
    DB --> SR
    SR --> UI
    GE --> UI
\`\`\`

## 5. Flujo de guardado y recuperación

### Guardado local

1. MainForm solicita el guardado.
2. GameEngine serializa GameState con System.Text.Json.
3. El archivo se escribe en la carpeta local de datos de NEXUS.
4. La interfaz informa si la operación tuvo éxito.

### Guardado SQL

1. MainForm solicita guardar el estado actual.
2. SqlSaveRepository obtiene o crea el perfil.
3. El repositorio abre una conexión y una transacción.
4. Inserta la partida, el progreso de las cuatro misiones y las mejoras.
5. Confirma la transacción; si ocurre un error, realiza rollback.
6. MainForm muestra el resultado.

### Recuperación SQL

1. La interfaz solicita la última partida o una partida identificada.
2. SqlSaveRepository consulta las tablas y reconstruye GameState.
3. MainForm entrega el estado recuperado a GameEngine.
4. El juego inicia con los datos cargados.

## 6. Relación entre componentes

| Componente | Responsabilidad |
|---|---|
| Program | Iniciar la aplicación |
| MainForm | Presentación y eventos |
| GameEngine | Reglas, objetivos y actualización del mundo |
| GameState | Datos de la partida |
| SqlSaveRepository | Operaciones de persistencia SQL |
| SoundFX | Reproducción y control de sonido |

## 7. Mejoras técnicas propuestas

Las siguientes mejoras son posibles etapas posteriores y no deben confundirse con funciones ya terminadas:

1. Añadir una pantalla independiente para consultar el inventario.
2. Mejorar el selector de partidas SQL para elegir entre varias partidas guardadas.
3. Centralizar la configuración de la cadena de conexión fuera del código fuente.
4. Añadir pruebas automatizadas para el guardado, la carga y la progresión de misiones.
5. Mostrar mensajes de error de conexión con instrucciones claras para configurar SQL Server.

## 8. Estado de implementación

La separación principal ya está presente en el código: MainForm administra la interfaz, GameEngine administra las reglas y SqlSaveRepository administra SQL Server. El guardado local JSON también está implementado.

La arquitectura propuesta no exige reemplazar estas clases. Se recomienda continuar mejorándolas de manera incremental y conservar la compatibilidad con el estado de partida existente.
