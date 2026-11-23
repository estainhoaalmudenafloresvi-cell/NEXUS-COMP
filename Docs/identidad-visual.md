# Identidad visual de NEXUS

## 1. Dirección visual

La interfaz de NEXUS utiliza una estética de ciencia ficción con fondos oscuros, superficies violetas y acentos rosados y cian. La intención es mantener una lectura clara de los controles y destacar las acciones principales sin alejarse de los recursos gráficos incluidos en el juego.

Los colores de esta guía se tomaron de los valores declarados en `MainForm.cs`. No son una paleta inventada para una futura interfaz.

## 2. Paleta de la interfaz

| Uso | Color | Código |
|---|---|---|
| Fondo principal (`ink`) | Violeta casi negro | `#120B22` |
| Superficie secundaria (`violet`) | Violeta profundo | `#2E1C48` |
| Texto destacado suave (`lavender`) | Lavanda | `#CCB2F5` |
| Acento principal (`pink`) | Rosa espacial | `#FAB1DC` |
| Acento informativo (`cyan`) | Cian claro | `#85E4FF` |
| Texto principal (`white`) | Blanco cálido | `#FAF1FF` |

### Muestra de la paleta

<p align="center">
  <img src="https://placehold.co/120x48/120B22/FAF1FF?text=INK" alt="Ink" />
  <img src="https://placehold.co/120x48/2E1C48/FAF1FF?text=VIOLET" alt="Violet" />
  <img src="https://placehold.co/120x48/CCB2F5/120B22?text=LAVENDER" alt="Lavender" />
  <img src="https://placehold.co/120x48/FAB1DC/120B22?text=PINK" alt="Pink" />
  <img src="https://placehold.co/120x48/85E4FF/120B22?text=CYAN" alt="Cyan" />
</p>

## 3. Recursos gráficos del repositorio

Los archivos siguientes forman parte de `NEXUS/Assets`:

| Recurso | Ruta | Uso identificado |
|---|---|---|
| Textura EOS | `Assets/Planets/planet-1.png` | Fondo del primer planeta |
| Textura NÉBULA | `Assets/Planets/planet-2.png` | Fondo del segundo planeta |
| Textura SYLVARA | `Assets/Planets/planet-3.png` | Fondo del tercer planeta |
| Textura ASTRALIS | `Assets/Planets/planet-4.png` | Fondo del cuarto planeta |
| Pantalla splash | `Assets/UI/nexus_splash.png` | Recurso gráfico de interfaz incluido |
| Audio EOS | `Assets/Audio/eos_crystal_garden.wav` | Pista de la expedición EOS |
| Audio NÉBULA | `Assets/Audio/nebula_submerged.wav` | Pista de la expedición NÉBULA |
| Audio SYLVARA | `Assets/Audio/sylvara_sanctuary.wav` | Pista de la expedición SYLVARA |
| Audio ASTRALIS | `Assets/Audio/astralis_void.wav` | Pista de la expedición ASTRALIS |

Las cuatro texturas planetarias se cargan desde `GameCanvas.LoadPlanetTexture` según la misión seleccionada. El proyecto las copia al directorio de salida mediante la configuración de `NEXUS.csproj`.

## 4. Componentes de interfaz observados

- **Menú principal:** título NEXUS, resumen de las expediciones y botones de navegación.
- **Selector de planeta:** tarjetas con el nombre, descripción, estado y acción para viajar.
- **Área de juego:** escenario 2D, exploradora, objetos, entidades y portal.
- **HUD:** indicadores de vida, energía, recursos, objetivo y radar.
- **Armería:** opciones para adquirir botiquines y mejorar el equipo.
- **Pausa y archivo:** controles para continuar, guardar o acceder a opciones de persistencia.

## 5. Reglas de consistencia visual

1. Mantener el fondo oscuro como base de las ventanas.
2. Usar rosa para acciones principales y cian para información o selección.
3. Reservar el blanco cálido para textos de lectura.
4. Mantener botones con etiquetas breves y claras.
5. Conservar nombres y colores de forma consistente entre menú, selector y juego.
6. No añadir elementos visuales que oculten el objetivo o los indicadores del HUD.
7. Diferenciar las propuestas de diseño de las pantallas ya implementadas.

## 6. Relación con el código

La paleta se define al inicio de `MainForm.cs`. El menú se construye en `BuildHome` y se dibuja en `PaintHome`. El selector de expediciones se crea en `ShowPlanetSelector`. Las texturas del escenario se cargan desde `GameCanvas.LoadPlanetTexture`.

Esta guía documenta el estilo existente y sirve como referencia para futuras mejoras. No implica que se haya modificado el diseño interno del juego.
