# NEXUS — Expedición Eos

NEXUS es un videojuego de acción y exploración en 2D desarrollado en C# para Windows. La versión actual utiliza Windows Forms y .NET 8. El jugador controla a una exploradora que recorre cuatro planetas, cumple objetivos, se enfrenta a entidades, recoge recursos y mejora su equipo.

El repositorio conserva el proyecto original de consola H3 y la versión gráfica actual. La versión gráfica reutiliza la idea de exploración y progresión, pero incorpora un motor de juego en tiempo real, escenarios dibujados, controles de teclado y ratón, efectos de sonido y persistencia de partidas.

## Proyectos

- **NEXUS/**: versión gráfica actual, desarrollada con Windows Forms.
- **ConsoleApp1/**: código original de consola utilizado como punto de partida.
- **Docs/**: diseño, arquitectura, modelo de datos, análisis técnico, manual y evidencias.
- **NEXUS.sln**: solución principal que se debe abrir en Visual Studio.

## Las cuatro expediciones

1. **EOS — Jardín de Cristal:** reunir tres Fragmentos Prismáticos y activar el portal de extracción.
2. **NÉBULA — Archivo Sumergido:** recuperar cuatro Núcleos de Datos antes de que termine la tormenta iónica; se pueden recoger estabilizadores.
3. **SYLVARA — Santuario de las Lunas:** sincronizar tres Altares Lunares y derrotar al Acechador Lunar.
4. **ASTRALIS — Corazón del Vacío:** desactivar dos Pilares de Escudo, derrotar al jefe final y activar el portal.

La secuencia de objetivos está definida en el motor GameEngine. La cuarta expedición cierra la campaña principal.

## Funcionalidades de la versión gráfica

- Menú principal con inicio de expedición, continuación local y selección de planeta.
- Exploración en escenarios 2D con obstáculos, cámara, radar y objetivos.
- Movimiento, apuntado, disparos dentro del juego, impulso e interacción con el entorno.
- Enemigos, objetos recogibles, altares, portales y encuentros con jefes.
- Indicadores de vida, energía, experiencia, créditos y progreso.
- Armería con mejoras de cañón, blindaje, propulsores y dron; compra de botiquines.
- Efectos visuales y sonidos asociados a los planetas.
- Guardado local JSON y guardado/carga mediante SQL Server.

## Controles

| Entrada | Acción |
|---|---|
| W, A, S, D o flechas | Moverse |
| Ratón | Apuntar |
| Clic izquierdo o Espacio | Acción de disparo |
| Shift o clic derecho | Impulso |
| Q | Onda prismática |
| E | Interactuar o activar el portal |
| H | Usar botiquín |
| U o B | Abrir la Armería |
| M | Silenciar o activar el sonido |
| F5 | Guardar partida local |
| Esc | Abrir la pausa |

## Requisitos

- Windows 10 u 11.
- .NET 8 SDK.
- Visual Studio 2022 con la carga de trabajo **Desarrollo de escritorio de .NET**, o un entorno compatible con Windows Forms.
- SQL Server o SQL Server Express para utilizar la persistencia SQL.
- SQL Server Management Studio (SSMS) para ejecutar el script de creación de la base de datos.

## Ejecutar el juego

1. Clona o descarga este repositorio.
2. Abre **NEXUS.sln** en Visual Studio 2022.
3. Comprueba que el proyecto **NEXUS** esté seleccionado como proyecto de inicio.
4. Ejecuta con F5.

También se incluye el archivo COMPILAR_Y_EJECUTAR.bat para iniciar la compilación desde Windows con el SDK instalado.

## Persistencia de partidas

### Guardado local JSON

La aplicación guarda una copia local en la carpeta de datos de la aplicación del usuario, dentro de NEXUS/expedicion.json. El guardado se realiza desde la pausa con F5 y también mediante autoguardado periódico.

### SQL Server

1. Abre NEXUS/SQL/CrearBaseNEXUS.sql en SQL Server Management Studio.
2. Ejecuta el script para crear la base de datos NEXUS y sus tablas.
3. Si tu instancia no coincide con la configuración predeterminada del código, define la variable de entorno NEXUS_SQL_CONNECTION con la cadena de conexión correcta.
4. Inicia el juego. Desde la pausa puedes guardar en SQL Server; desde Archivo y Base de Datos SQL puedes intentar recuperar la partida más reciente.

El script crea las tablas **Perfiles**, **Misiones**, **Partidas**, **ProgresoMisiones** y **MejorasPartida**. El inventario se conserva como JSON dentro del registro de partida; no existe una tabla independiente de inventario en el esquema actual.

**Importante:** guardar localmente en JSON y guardar en SQL Server son operaciones diferentes. Para comprobar la persistencia SQL, ejecuta primero el script y utiliza las opciones SQL del juego.

## Documentación

- [Diseño del juego](Docs/diseno-del-juego.md)
- [Arquitectura actual](Docs/arquitectura-actual.md)
- [Arquitectura propuesta](Docs/arquitectura-propuesta.md)
- [Modelo de datos](Docs/modelo-de-datos-propuesto.md)
- [Análisis técnico](Docs/analisis-tecnico.md)
- [Manual técnico de la versión H3](Docs/Manual_Tecnico_NEXUS_H3.md)
- [Diagramas](Docs/evidencias/diagramas.md)
- [Bocetos de pantallas](Docs/evidencias/bocetos-pantallas.md)
- [Registro de correcciones](CORRECCIONES.md)

## Validación automática

El repositorio incluye un workflow de GitHub Actions en .github/workflows/validar-nexus.yml. Este workflow restaura y compila la solución principal en un entorno Windows con .NET 8.

La compilación automática no sustituye las pruebas manuales del juego ni la verificación de la conexión a una instancia SQL Server local.

## Estado del proyecto

La versión gráfica, las cuatro expediciones, el guardado local y la integración de persistencia SQL están presentes en el código. La conexión SQL depende de que cada equipo configure una instancia válida y ejecute el script.

Los bocetos de la carpeta de evidencias son documentación de apoyo. La pantalla independiente de inventario descrita como mejora de interfaz es una propuesta y no debe confundirse con una ventana ya implementada.
