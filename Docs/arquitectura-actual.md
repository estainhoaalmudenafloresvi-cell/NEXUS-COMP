# Arquitectura actual de NEXUS

## 1. Objetivo

Este documento describe las dos etapas presentes en el repositorio: el proyecto original de consola H3 y la versión gráfica actual. La finalidad es explicar cómo se pasó de la estructura inicial a una aplicación Windows Forms y qué responsabilidades tiene cada parte.

## 2. Versiones y tecnologías

### Proyecto original H3

El proyecto ConsoleApp1 conserva el videojuego de consola desarrollado en C#. Su manual técnico describe las clases del cadete, personajes, mapa y sistema de juego, así como el guardado JSON.

### Versión gráfica actual

El proyecto NEXUS utiliza:
- C# y .NET 8 para Windows.
- Windows Forms para la ventana principal y los diálogos.
- Dibujo 2D mediante System.Drawing.
- Microsoft.Data.SqlClient para conectar con SQL Server.
- System.Text.Json para el guardado local y para serializar el inventario dentro del registro SQL.
- Archivos PNG y WAV dentro de NEXUS/Assets.

La solución principal es NEXUS.sln y el proyecto gráfico se encuentra en NEXUS/NEXUS.csproj.

## 3. Organización de la versión gráfica

### 3.1. Program

Program contiene el punto de entrada Main. Inicializa la configuración de Windows Forms y abre MainForm.

### 3.2. MainForm

MainForm es la ventana principal y coordina la interacción con la jugadora. Crea los controles de la interfaz mediante código, administra los temporizadores y conecta los eventos de teclado y ratón con el motor del juego.

Responsabilidades principales:
- Construir el menú principal y el selector de planeta.
- Solicitar el nombre de la exploradora.
- Iniciar o cargar una partida local.
- Crear el área de juego y dibujar el escenario, el HUD y el radar.
- Interpretar el teclado y el ratón.
- Mostrar pausa y Armería.
- Guardar localmente y solicitar operaciones SQL.
- Mostrar mensajes de resultado y errores.

La ventana utiliza un temporizador de juego de 16 milisegundos para actualizar el estado y refrescar el dibujo. También usa un temporizador separado para la animación del menú.

### 3.3. GameEngine

GameEngine contiene las reglas y el estado dinámico del videojuego. No construye los botones ni administra directamente la ventana.

Entre sus responsabilidades están:
- Crear una nueva partida y seleccionar la misión.
- Preparar el escenario de cada planeta.
- Actualizar movimiento, enemigos, objetos y objetivos.
- Gestionar disparos y habilidades del juego.
- Procesar la recolección y las interacciones.
- Controlar la finalización o el fracaso de una misión.
- Aplicar recompensas y mejoras.
- Guardar y cargar el estado JSON.

Métodos públicos relevantes identificados en el código:
- Nueva(string name)
- SelectMission(int missionIndex)
- StartMission()
- SetAim(float dx, float dy)
- Update(...)
- Shoot()
- Dash()
- Pulse()
- Interact()
- UseMedkit()
- Retry()
- UpgradeWeapon()
- UpgradeShield()
- UpgradeThruster()
- UpgradeDrone()
- BuyMedkit()
- Save(string path)
- Load(string path)
- LoadFromState(GameState state)

### 3.4. GameState y clases auxiliares

GameState almacena los datos persistentes de la exploradora: nombre, vida, energía, XP, nivel, misión, recursos, mejoras, misiones completadas e inventario.

GameEngine utiliza además estructuras auxiliares para representar elementos del mundo:
- Enemy y EnemyKind: entidades y tipos de enemigo.
- Shot: proyectiles del juego.
- Pickup: objetos recogibles.
- LunarAltar: altares de la tercera expedición.
- Particle, FloatingText y GhostTrail: efectos visuales.

Estas clases separan los datos de los objetos del escenario de la lógica general del motor.

### 3.5. SqlSaveRepository

SqlSaveRepository concentra las operaciones de persistencia en SQL Server. Utiliza SqlConnection, SqlCommand, parámetros tipados y SqlDataReader.

Sus operaciones principales son:
- ProbarConexion()
- ListarPerfiles()
- CrearPerfil(string nombre)
- ObtenerOCrearPerfil(string nombre)
- EliminarPerfil(int idPerfil)
- Guardar(GameState state)
- GuardarPartida(int idPerfil, GameState state)
- ListarPartidas(int idPerfil)
- CargarPartida(int idPartida)
- ListarPartidas()
- CargarUltima()
- EliminarPartida(int idPartida)

Al guardar una partida, el repositorio utiliza una transacción para registrar la partida, el progreso de las cuatro misiones y los niveles de las mejoras. Si ocurre un error, revierte la transacción.

### 3.6. SoundFX

SoundFX administra los sonidos asociados al juego y a los planetas. La clase mantiene las operaciones de audio separadas de la interfaz y del motor.

## 4. Persistencia actual

### Guardado local JSON

MainForm guarda el archivo expedicion.json dentro de la carpeta local de datos de la aplicación del usuario. El juego permite guardar con F5 y desde la pausa. También realiza un autoguardado periódico. GameEngine utiliza System.Text.Json para serializar GameState.

### SQL Server

El script NEXUS/SQL/CrearBaseNEXUS.sql crea la base NEXUS y cinco tablas:
- Perfiles
- Misiones
- Partidas
- ProgresoMisiones
- MejorasPartida

La conexión se obtiene de la variable de entorno NEXUS_SQL_CONNECTION o de una cadena predeterminada en SqlSaveRepository. Esa cadena predeterminada puede requerir ajuste según la instancia SQL Server de cada equipo.

El guardado SQL se inicia desde la pausa. La opción Archivo y Base de Datos SQL permite intentar cargar la partida más reciente. El inventario se serializa en InventarioJson dentro de Partidas.

## 5. Flujo de ejecución

1. Program inicializa Windows Forms y crea MainForm.
2. MainForm construye el menú principal.
3. La jugadora inicia una expedición o selecciona un planeta.
4. GameEngine prepara el escenario y el estado inicial.
5. El temporizador de MainForm llama periódicamente a GameEngine.Update.
6. Los eventos de teclado y ratón transmiten las acciones de la jugadora al motor.
7. MainForm dibuja el estado actualizado en el área de juego.
8. El juego guarda localmente de forma periódica y bajo petición.
9. Si la jugadora lo solicita, SqlSaveRepository guarda o recupera datos de SQL Server.

## 6. Relación con H3

El proyecto de consola se conserva en ConsoleApp1. La versión gráfica mantiene la idea de exploración, personaje, recursos, encuentros y progreso, pero su implementación actual utiliza GameState y GameEngine como estructura principal.

No se debe afirmar que las clases antiguas de H3 se usan directamente en la versión gráfica si no están referenciadas desde el proyecto NEXUS. La relación entre ambas versiones es principalmente evolutiva y conceptual, además de conservar el código original en el repositorio.

## 7. Limitaciones observadas

- La conexión SQL depende de que exista una instancia disponible y de que la cadena de conexión sea correcta.
- El guardado SQL se realiza desde una acción de la interfaz; el autoguardado periódico corresponde al archivo JSON local.
- La interfaz no contiene una pantalla independiente para administrar el inventario, aunque el estado y la persistencia sí conservan la lista de objetos.
- El archivo Manual_Tecnico_NEXUS_H3.md documenta la versión original de consola y debe leerse como referencia histórica, no como descripción de la interfaz gráfica actual.
- La presencia del script SQL no demuestra por sí sola que la base esté instalada en todos los equipos; debe ejecutarse y probarse en cada instancia.

## 8. Archivos principales

| Archivo | Responsabilidad |
|---|---|
| NEXUS/Program.cs | Inicio de Windows Forms |
| NEXUS/MainForm.cs | Interfaz, eventos, temporizadores y dibujo |
| NEXUS/GameEngine.cs | Reglas y estado del juego |
| NEXUS/SqlSaveRepository.cs | Acceso a SQL Server |
| NEXUS/SoundFX.cs | Audio |
| NEXUS/SQL/CrearBaseNEXUS.sql | Creación del esquema SQL |
| ConsoleApp1/Program.cs | Versión original de consola |
