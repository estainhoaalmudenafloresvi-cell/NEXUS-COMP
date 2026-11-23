# Diseño del juego — NEXUS: Expedición Eos

## 1. Visión del juego

NEXUS es un videojuego de acción y exploración en 2D para PC con Windows. La jugadora controla a una exploradora espacial que debe completar cuatro expediciones en planetas distintos. Cada planeta presenta un objetivo, peligros y elementos propios.

El proyecto parte del videojuego de consola desarrollado en H3. La versión actual conserva la temática de exploración, los recursos del personaje, los enemigos y la progresión, y los presenta mediante una interfaz gráfica desarrollada en C# con Windows Forms.

**Género:** acción, exploración y ciencia ficción.  
**Plataforma:** PC con Windows.  
**Lenguaje:** C#.  
**Interfaz actual:** Windows Forms, con dibujo 2D mediante gráficos de .NET.  
**Modo:** un jugador.

## 2. Objetivo y experiencia

El objetivo principal es completar las cuatro expediciones, reunir los recursos necesarios, superar los desafíos de cada planeta y activar el portal de salida. La experiencia combina desplazamiento, exploración del escenario, encuentros, recolección de objetos y mejora del equipo.

La interfaz utiliza una paleta oscura con tonos violetas, rosados y cian. Los indicadores del HUD permiten consultar el estado de la exploradora y el objetivo activo.

## 3. Ciclo principal de juego

1. **Preparación:** la jugadora inicia una expedición, introduce el nombre de la exploradora o selecciona un planeta.
2. **Exploración:** recorre el escenario, evita obstáculos y localiza los objetivos.
3. **Encuentros:** responde a las amenazas y utiliza las habilidades disponibles.
4. **Recolección:** obtiene fragmentos, núcleos, créditos, energía o elementos de recuperación.
5. **Progreso:** completa el objetivo del planeta y activa el portal.
6. **Mejora:** utiliza los créditos en la Armería para mejorar el equipo.
7. **Persistencia:** guarda el avance localmente o en SQL Server para continuar después.

## 4. Las cuatro expediciones

### 4.1. EOS — Jardín de Cristal

La exploradora debe reunir tres Fragmentos Prismáticos dispersos por el escenario. Después de completar la recolección, debe acercarse al portal de extracción y activarlo. El planeta incluye centinelas de cristal y objetos de recuperación.

### 4.2. NÉBULA — Archivo Sumergido

La exploradora debe recuperar cuatro Núcleos de Datos antes de que finalice la tormenta iónica. En el escenario aparecen estabilizadores que ayudan a mantener la expedición. El temporizador y los objetivos hacen que esta misión tenga presión de tiempo.

### 4.3. SYLVARA — Santuario de las Lunas

La exploradora debe sincronizar tres Altares Lunares permaneciendo dentro de sus círculos de energía. Cuando los tres altares están sincronizados, debe superar al Acechador Lunar para habilitar el portal.

### 4.4. ASTRALIS — Corazón del Vacío

La expedición final requiere desactivar dos Pilares de Escudo para dejar vulnerable al jefe del planeta. Después de superar el encuentro final, la exploradora puede activar el portal y completar la campaña.

Las cuatro expediciones y sus objetivos están definidos en GameEngine. Los nombres, los objetivos y el orden se deben mantener sincronizados con la selección de planetas y con los registros de la tabla Misiones.

## 5. Mecánicas y reglas

### 5.1. Exploradora y estado de partida

El estado principal se concentra en GameState. Incluye nombre, vida, energía, experiencia, nivel, misión actual, créditos, botiquines, eliminaciones, mejor racha, fragmentos, núcleos, mejoras y misiones completadas.

### 5.2. Movimiento y escenario

El escenario se representa en un mundo 2D de 2600 por 1700 unidades. La jugadora se desplaza con WASD o flechas. El ratón permite orientar el apuntado. El motor actualiza el movimiento, los encuentros y los elementos del escenario mediante un temporizador de Windows Forms.

### 5.3. Habilidades e interacción

- Shift o clic derecho: impulso.
- Q: onda prismática.
- E: interacción con elementos del escenario y activación del portal.
- H: uso de botiquín.
- U o B: acceso a la Armería.
- Esc: pausa.
- F5: guardado local.

### 5.4. Recursos e inventario

El estado de partida contempla vida, energía, botiquines, créditos, fragmentos, núcleos y una lista de objetos. Los objetos recogidos se gestionan desde la lógica del juego. En la persistencia SQL, la lista del inventario se serializa en el campo InventarioJson de Partidas.

La versión actual no tiene una ventana independiente de inventario. Por eso, el boceto de inventario incluido en evidencias se identifica como una mejora de interfaz propuesta y no como una pantalla ya implementada.

### 5.5. Progresión y mejoras

Al completar una misión, el motor marca el planeta como completado, concede 85 créditos y un botiquín, restaura vida y energía y avanza a la siguiente expedición. La Armería permite mejorar el cañón, el blindaje, los propulsores y el dron, además de comprar botiquines.

## 6. Navegación y pantallas

La interfaz actual incluye:
- Menú principal.
- Diálogo de identidad de la exploradora.
- Selector de planeta.
- Pantalla de exploración con HUD y radar.
- Pantalla de pausa.
- Armería Estelar.
- Archivo de exploradora y opciones de consulta/carga SQL.

Los bocetos de cuatro pantallas requeridas por el trabajo están en [evidencias/bocetos-pantallas.md](evidencias/bocetos-pantallas.md). En ellos se diferencia lo que ya existe de la pantalla de inventario propuesta.

## 7. Criterios de experiencia de usuario

- **Legibilidad:** usar texto claro, tamaños adecuados y contraste entre fondo e indicadores.
- **Jerarquía:** destacar el objetivo del planeta, la vida, la energía y las acciones principales.
- **Consistencia:** mantener los mismos colores, nombres y estilos de botones.
- **Navegación:** permitir regresar al menú, pausar y acceder a las opciones principales sin perder el estado.
- **Retroalimentación:** informar cuando se recoge un objeto, se completa un objetivo, se compra una mejora o se guarda una partida.
- **Accesibilidad básica:** ofrecer controles de teclado y ratón y mostrar las teclas principales en la documentación.

## 8. Alcance y estado

La versión gráfica actual contiene el menú, selector de planetas, exploración, HUD, pausa, Armería, archivo, cuatro expediciones, motor de juego y persistencia local y SQL.

Quedan como posibles mejoras una pantalla independiente para consultar y administrar el inventario, una pantalla de selección de partidas SQL más completa y pruebas de integración automatizadas contra una instancia SQL Server. Estas mejoras no deben describirse como funcionalidades ya terminadas.
