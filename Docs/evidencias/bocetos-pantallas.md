# Bocetos de pantallas — NEXUS

## Propósito

Estos bocetos son esquemas sencillos de referencia para documentar la interfaz de Windows Forms. No son capturas reales ni reemplazan las pantallas implementadas. Se indica el estado de cada propuesta para evitar confundir un diseño futuro con una función existente.

## 1. Menú principal

**Estado:** implementado en MainForm.BuildHome.

\`\`\`text
+---------------------------------------------------------------+
|                         N E X U S                             |
|            EXPEDICIÓN EOS / ACCIÓN TÁCTICA EN 4 PLANETAS       |
|                                                               |
|             Cuatro mundos. Cuatro misiones únicas.            |
|                                                               |
|             [ INICIAR NUEVA EXPEDICIÓN ]                      |
|             [ CONTINUAR PARTIDA GUARDADA ]                    |
|             [ MAPA ESTELAR / ELEGIR PLANETA ]                 |
|             [ ARCHIVO Y BASE DE DATOS SQL ]                   |
|             [ SALIR ]                                         |
|                                                               |
+---------------------------------------------------------------+
\`\`\`

Elementos visibles: título, descripción de las cuatro expediciones y botones para iniciar, continuar, elegir planeta, consultar el archivo SQL o salir.

## 2. Exploración y mapa

**Estado:** implementado en el área de juego de MainForm.

\`\`\`text
+---------------------------------------------------------------+
| NEXUS                     PLANETA: EOS         OBJETIVO       |
|                                                               |
|  +---------------------------------------------------------+  |
|  |                                                         |  |
|  |     *       roca              enemigo                    |  |
|  |                                                         |  |
|  |               exploradora                                |  |
|  |                                       fragmento          |  |
|  |                                                         |  |
|  |                                portal                    |  |
|  +---------------------------------------------------------+  |
|                                                               |
| VIDA [##########]     ENERGÍA [########  ]     RADAR          |
| FRAGMENTOS: 1/3       CRÉDITOS: 45             NIVEL: 1       |
| WASD: mover | Ratón: apuntar | E: interactuar | Esc: pausa   |
+---------------------------------------------------------------+
\`\`\`

Elementos visibles: escenario, exploradora, obstáculos, entidades, objetos, portal, objetivo, indicadores y radar. Los elementos del dibujo son esquemáticos; la posición cambia durante la partida.

## 3. Misiones / mapa estelar

**Estado:** selector de planeta implementado en MainForm.ShowPlanetSelector.

\`\`\`text
+---------------------------------------------------------------+
|                  SELECCIONA DESTINO PLANETARIO                |
|                                                               |
|  [ EOS ]      Jardín de Cristal                 [ VIAJAR ]   |
|               Reunir 3 fragmentos                            |
|                                                               |
|  [ NÉBULA ]   Archivo Sumergido                 [ VIAJAR ]   |
|               Recuperar 4 núcleos                            |
|                                                               |
|  [ SYLVARA ]  Santuario de las Lunas            [ VIAJAR ]   |
|               Sincronizar 3 altares                          |
|                                                               |
|  [ ASTRALIS ] Corazón del Vacío                 [ VIAJAR ]   |
|               Desactivar pilares y vencer al jefe            |
|                                                               |
+---------------------------------------------------------------+
\`\`\`

Elementos visibles: nombre del planeta, descripción breve, estado de misión completada y botón para viajar.

## 4. Exploradora e inventario

**Estado:** propuesta de mejora. El código actual conserva datos de inventario y ofrece la Armería y el Archivo, pero no tiene una ventana independiente de inventario con este diseño.

\`\`\`text
+---------------------------------------------------------------+
|                EXPLORADORA / EQUIPO                           |
|                                                               |
|  Nombre: Nova             Nivel: 1          XP: 0              |
|  Vida: 100/100            Energía: 100/100                     |
|  Créditos: 45             Botiquines: 2                        |
|                                                               |
|  INVENTARIO                                                     |
|  +---------------------------------------------------------+  |
|  | Botiquín                                  Cantidad: 2   |  |
|  | Celda de energía                          Cantidad: 1   |  |
|  | Fragmentos prismáticos                    Cantidad: 0   |  |
|  +---------------------------------------------------------+  |
|                                                               |
|  [ VOLVER ]                 [ ABRIR ARMERÍA ]                 |
+---------------------------------------------------------------+
\`\`\`

Esta pantalla se propone para facilitar la consulta del estado y los objetos. Antes de implementarla se debe definir cómo se mostrarán las cantidades y cómo se sincronizarán con GameState e InventarioJson.

## 5. Relación con la implementación

- Menú principal: implementado.
- Exploración y HUD: implementados.
- Selector de planeta: implementado.
- Pantalla independiente de inventario: propuesta.
- Armería y pausa: implementadas, aunque no sustituyen una pantalla completa de inventario.

## 6. Evidencias disponibles y pendientes

El repositorio ya contiene una imagen de la interfaz en NEXUS_ Interfaz de juego en español.png y los diagramas en diagramas.md.

Para completar las evidencias de funcionamiento se recomienda añadir capturas reales del juego ejecutándose, de las cuatro misiones, de las tablas visibles en SQL Server Management Studio y de una prueba de guardado y recuperación. Esas capturas deben tomarse en el equipo donde se ejecuta el proyecto; estos bocetos no las sustituyen.
