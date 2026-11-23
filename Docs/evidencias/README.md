# Evidencias de funcionamiento de NEXUS

Esta carpeta reúne diagramas, bocetos y capturas que permiten revisar el proyecto. Los bocetos y diagramas explican el diseño; las pruebas de funcionamiento deben documentarse con capturas obtenidas al ejecutar el programa real.

## Material que ya está en el repositorio

- `NEXUS_ Interfaz de juego en español.png`: imagen de interfaz ya incorporada.
- `diagramas.md`: diagramas de arquitectura y flujo.
- `bocetos-pantallas.md`: esquemas de menú, exploración, selector de misiones e inventario propuesto.

## Capturas reales que faltan completar

Guarda las imágenes tomadas desde tu propio equipo en esta carpeta. No uses los bocetos como sustituto de las capturas de ejecución.

| Nombre sugerido | Qué debe mostrar |
|---|---|
| `01-menu-principal.png` | NEXUS abierto en el menú principal |
| `02-selector-planetas.png` | Selector con las cuatro expediciones |
| `03-expedicion-eos.png` | Partida real en EOS con el HUD visible |
| `04-expedicion-nebula.png` | Partida real en NÉBULA |
| `05-expedicion-sylvara.png` | Partida real en SYLVARA |
| `06-expedicion-astralis.png` | Partida real en ASTRALIS |
| `07-tablas-sql-server.png` | Las cinco tablas de NEXUS visibles en SSMS |
| `08-guardar-partida-sql.png` | Resultado de guardar una partida en SQL Server |
| `09-recuperar-partida-sql.png` | Resultado de cargar la partida guardada |

## Cómo tomar evidencias claras

1. Ejecuta la solución `NEXUS.sln` desde Visual Studio en Windows.
2. Maximiza o acomoda la ventana para que se lean el juego y sus indicadores.
3. Toma una captura de cada pantalla o prueba solicitada.
4. Para SQL Server, muestra el nombre de la base de datos y las tablas creadas.
5. Para guardado y recuperación, muestra el resultado real de las operaciones y, si es posible, los registros de la base de datos.
6. Copia las capturas a esta carpeta y conserva los nombres sugeridos.

## Importante

No se afirma que las pruebas de SQL Server estén completadas solo porque el script exista. La conexión depende de la instancia configurada en el equipo. Tampoco se debe presentar una pantalla propuesta como si ya estuviera implementada.
