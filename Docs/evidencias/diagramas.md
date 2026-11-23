# Diagramas de NEXUS — versión gráfica

## 1. Alcance

Estos diagramas representan el funcionamiento de la versión gráfica que se encuentra en el repositorio. Se basan en Program.cs, MainForm.cs, GameEngine.cs, SqlSaveRepository.cs y el script SQL.

## 2. Flujo principal

\`\`\`mermaid
flowchart TD
    A[Iniciar aplicación] --> B[Menú principal]
    B --> C[Nueva expedición]
    B --> D[Continuar JSON local]
    B --> E[Elegir planeta]
    B --> F[Archivo y base SQL]
    C --> G[Identidad de exploradora]
    G --> H[GameEngine prepara misión]
    D --> H
    E --> H
    H --> I[Exploración y objetivos]
    I --> J{¿Objetivo cumplido?}
    J -->|No| I
    J -->|Sí| K[Recompensa y siguiente misión]
    K --> I
    I --> L[Pausa]
    L --> M[Guardar JSON]
    L --> N[Guardar SQL]
    L --> O[Armería]
    M --> I
    N --> I
    O --> I
\`\`\`

## 3. Navegación de la interfaz

\`\`\`mermaid
flowchart TD
    A[Menú principal] --> B[Nueva expedición]
    A --> C[Continuar partida]
    A --> D[Mapa estelar]
    A --> E[Archivo y base SQL]
    B --> F[Nombre de exploradora]
    F --> G[Exploración]
    C --> G
    D --> G
    G --> H[Pausa]
    G --> I[Armería]
    H --> I
    H --> J[Guardar local]
    H --> K[Guardar SQL]
    I --> G
    J --> G
    K --> G
    E --> L[Cargar última partida SQL]
    L --> G
\`\`\`

## 4. Arquitectura implementada

\`\`\`mermaid
flowchart TD
    P[Program] --> UI[MainForm - Windows Forms]
    UI --> GE[GameEngine]
    GE --> GS[GameState]
    UI --> SR[SqlSaveRepository]
    GE --> JSON[System.Text.Json]
    SR --> SQL[(SQL Server)]
    UI --> SFX[SoundFX]
    GE --> SFX
\`\`\`

## 5. Modelo de datos SQL actual

\`\`\`mermaid
erDiagram
    PERFILES ||--o{ PARTIDAS : guarda
    MISIONES ||--o{ PARTIDAS : identifica
    PARTIDAS ||--o{ PROGRESO_MISIONES : registra
    MISIONES ||--o{ PROGRESO_MISIONES : aparece_en
    PARTIDAS ||--o{ MEJORAS_PARTIDA : contiene
\`\`\`

El inventario se serializa en el campo InventarioJson de Partidas; no es una tabla independiente.

## 6. Flujo de una operación de guardado SQL

\`\`\`mermaid
flowchart TD
    A[MainForm solicita guardar] --> B[SqlSaveRepository]
    B --> C[Obtener o crear perfil]
    C --> D[Abrir conexión y transacción]
    D --> E[Insertar Partidas]
    E --> F[Registrar ProgresoMisiones]
    F --> G[Registrar MejorasPartida]
    G --> H{¿Todas las operaciones correctas?}
    H -->|Sí| I[Commit]
    H -->|No| J[Rollback]
    I --> K[Informar resultado]
    J --> K
\`\`\`

## 7. Relación con el proyecto H3

ConsoleApp1 conserva el proyecto de consola original. NEXUS contiene la versión gráfica. Ambos forman parte de la historia del proyecto, pero la aplicación gráfica tiene su propio motor GameEngine y su estado GameState.
