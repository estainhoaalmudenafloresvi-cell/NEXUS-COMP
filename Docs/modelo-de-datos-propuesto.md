# Modelo de datos de NEXUS

## 1. Objetivo

Este documento describe el modelo utilizado por el script SQL actual de NEXUS. La base de datos permite registrar perfiles, partidas, misiones, progreso y mejoras para recuperar el avance de la exploradora.

## 2. Tablas existentes

El archivo NEXUS/SQL/CrearBaseNEXUS.sql crea cinco tablas.

### 2.1. Perfiles

Almacena el nombre de cada exploradora y la fecha en que se creó el perfil. Un perfil puede tener varias partidas guardadas.

### 2.2. Misiones

Contiene el catálogo de las cuatro expediciones. El script registra:
1. EOS — Jardín de Cristal.
2. NÉBULA — Archivo Sumergido.
3. SYLVARA — Santuario de las Lunas.
4. ASTRALIS — Corazón del Vacío.

### 2.3. Partidas

Guarda el estado principal de una partida:
- Perfil al que pertenece.
- Misión actual.
- Nivel y experiencia.
- Vida y energía actuales y máximas.
- Créditos y botiquines.
- Eliminaciones y mejor racha.
- Fragmentos y núcleos.
- Inventario serializado como JSON.
- Fecha y hora del guardado.

El inventario no tiene una tabla independiente en el esquema actual. Sus elementos se guardan en el campo InventarioJson de Partidas.

### 2.4. ProgresoMisiones

Registra si cada misión está completada dentro de una partida. De esta forma, el juego puede conservar el avance de las cuatro expediciones.

### 2.5. MejorasPartida

Almacena el nivel de las mejoras compradas para una partida:
- Arma.
- Escudo.
- Propulsor.
- Dron.

## 3. Relaciones entre los datos

- Un perfil puede tener varias partidas.
- Cada partida corresponde a un perfil y tiene una misión actual.
- Una partida registra el estado de varias misiones en ProgresoMisiones.
- El catálogo Misiones se utiliza tanto para identificar la misión actual como para registrar el progreso.
- Una partida puede tener varios registros de mejoras.

## 4. Diagrama del modelo

\`\`\`mermaid
erDiagram
    PERFILES ||--o{ PARTIDAS : guarda
    MISIONES ||--o{ PARTIDAS : identifica
    PARTIDAS ||--o{ PROGRESO_MISIONES : registra
    MISIONES ||--o{ PROGRESO_MISIONES : aparece_en
    PARTIDAS ||--o{ MEJORAS_PARTIDA : contiene
\`\`\`

El inventario se representa dentro de Partidas mediante un campo JSON, por lo que no aparece como entidad independiente en este diagrama.

## 5. Cómo se guarda una partida en SQL Server

El repositorio SqlSaveRepository realiza las siguientes operaciones:
1. Busca el perfil por nombre o lo crea si todavía no existe.
2. Abre una conexión con SQL Server.
3. Inicia una transacción.
4. Inserta el estado principal en Partidas.
5. Registra el estado de las cuatro misiones en ProgresoMisiones.
6. Registra las mejoras de arma, escudo, propulsor y dron en MejorasPartida.
7. Confirma la transacción.

Si una operación falla, la transacción se revierte para evitar dejar una partida guardada de forma incompleta.

## 6. Cómo se recupera una partida

El repositorio consulta la partida seleccionada, recupera el perfil, reconstruye el estado principal y después carga el progreso de misiones y los niveles de mejoras. El inventario se reconstruye leyendo el JSON almacenado en Partidas.

La interfaz actual también ofrece la opción de cargar la última partida registrada en SQL Server.

## 7. Relación con el guardado local

El juego conserva además un archivo JSON local en la carpeta de datos de la aplicación. Este mecanismo es independiente de SQL Server: el guardado local no necesita que SQL Server esté conectado.

## 8. Integridad y validaciones

El script utiliza claves, restricciones y valores predeterminados para proteger la información. Entre las validaciones se encuentran límites para vida, energía, nivel, créditos, botiquines y mejoras, además de la comprobación de que InventarioJson contenga JSON válido.

El script registra las cuatro misiones si todavía no existen y crea un índice para consultar las partidas de un perfil por fecha.

## 9. Consideraciones y alcance

Este documento refleja las cinco tablas del script actual. No se debe añadir una tabla de inventario o de objetos como si ya existiera. Si en una siguiente versión se decide separar los objetos en filas individuales, habrá que modificar el modelo, el script SQL y SqlSaveRepository de manera coordinada.
