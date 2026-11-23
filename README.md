# NEXUS — Expedición Eos

Videojuego de acción táctica 2D desarrollado en C# con Windows Forms y .NET 8. La versión gráfica contiene cuatro expediciones, combate, exploración, progresión, inventario y guardado de partidas. Se conserva también el proyecto original de consola H3.

## Proyectos

- NEXUS/: versión gráfica Windows Forms.
- ConsoleApp1/: proyecto original de consola.
- Docs/: documentación de diseño, arquitectura, modelo de datos, análisis técnico y evidencias.

## Expediciones

1. EOS — Jardín de Cristal: reunir los Fragmentos Prismáticos y activar el portal.
2. NÉBULA — Archivo Sumergido: recuperar los Núcleos de Datos antes de que termine la tormenta.
3. SYLVARA — Santuario de las Lunas: sincronizar los altares y vencer al Acechador Lunar.
4. ASTRALIS — Corazón del Vacío: desactivar los pilares de escudo y derrotar al Vigía de Eos.

## Controles

| Entrada | Acción |
|---|---|
| WASD / Flechas | Movimiento |
| Ratón | Apuntado |
| Clic izquierdo / Espacio | Disparar |
| Shift / Clic derecho | Impulso espectral |
| Q | Onda prismática |
| E | Activar portal |
| H | Usar botiquín |
| U / B | Abrir Armería Estelar |
| M | Activar o silenciar sonido |
| F5 | Guardar partida |
| Esc | Pausa |

## Requisitos y ejecución

- Windows 10/11.
- Visual Studio 2022 con la carga de trabajo Desarrollo de escritorio de .NET, o .NET 8 SDK.
- SQL Server / SQL Server Express si se desea utilizar la persistencia SQL.

Abre NEXUS.sln en Visual Studio 2022, selecciona el proyecto NEXUS como inicio y ejecuta con F5. También puedes usar COMPILAR_Y_EJECUTAR.bat.

## Persistencia SQL Server

1. Abre NEXUS/SQL/CrearBaseNEXUS.sql en SQL Server Management Studio y ejecútalo.
2. Comprueba que se hayan creado las tablas Perfiles, Misiones, Partidas, ProgresoMisiones y MejorasPartida.
3. Si tu instancia SQL Server es distinta, configura la variable de entorno NEXUS_SQL_CONNECTION con tu cadena de conexión. El código incluye una cadena predeterminada que puede requerir adaptación al nombre de instancia de tu equipo.

La persistencia guarda perfiles y partidas, y registra el progreso de las cuatro misiones y las mejoras de la Armería. El guardado local JSON también se conserva.

## Estructura principal

NEXUS-COMP/
├── ConsoleApp1/
├── NEXUS/
│   ├── Assets/ (Audio, Planets, UI)
│   ├── SQL/
│   ├── GameEngine.cs
│   ├── MainForm.cs
│   ├── Program.cs
│   ├── SoundFX.cs
│   ├── SqlSaveRepository.cs
│   └── NEXUS.csproj
├── Docs/
├── NEXUS.sln
├── NEXUS V2.slnx
└── README.md

Consulta Docs/ para la documentación del proyecto y CORRECCIONES.md para el registro de correcciones conocidas.
