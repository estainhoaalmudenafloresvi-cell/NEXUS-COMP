using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace NEXUS;

/// <summary>Perfil de jugador (tabla dbo.Perfiles) con el total de partidas que tiene guardadas.</summary>
public sealed record PerfilInfo(int IdPerfil, string Nombre, DateTime FechaCreacion, int TotalPartidas);

/// <summary>Resumen de una partida guardada, para mostrarla en la lista de partidas.</summary>
public sealed record PartidaInfo(
    int IdPartida, int IdPerfil, string Perfil, int Mision, string Planeta, int Nivel, int Creditos, DateTime FechaGuardado);

/// <summary>
/// Persistencia en SQL Server con 5 tablas relacionadas:
/// Perfiles, Misiones, Partidas, ProgresoMisiones y MejorasPartida (ver SQL/CrearBaseNEXUS.sql).
/// </summary>
public sealed class SqlSaveRepository
{
    private readonly string connectionString;

    // Cada mejora de la Armería es una fila de dbo.MejorasPartida (TipoMejora, Nivel).
    private static readonly (string Tipo, Func<GameState, int> Leer, Action<GameState, int> Escribir)[] Mejoras =
    {
        ("Arma", s => s.ArmaNivel, (s, v) => s.ArmaNivel = v),
        ("Escudo", s => s.EscudoNivel, (s, v) => s.EscudoNivel = v),
        ("Propulsor", s => s.PropulsorNivel, (s, v) => s.PropulsorNivel = v),
        ("Dron", s => s.DronNivel, (s, v) => s.DronNivel = v)
    };

    public SqlSaveRepository()
    {
        connectionString = Environment.GetEnvironmentVariable("NEXUS_SQL_CONNECTION")
            ?? "Server=DESKTOP-9HMHJII\\SQLEXPRESS;Database=NEXUS;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;Connect Timeout=5;";
    }

    private SqlConnection Open()
    {
        var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    private static SqlParameter Int(string name, int value) =>
        new SqlParameter(name, SqlDbType.Int) { Value = value };

    private static string LimpiarNombre(string? nombre)
    {
        nombre = string.IsNullOrWhiteSpace(nombre) ? "Nova" : nombre.Trim();
        return nombre.Length > 40 ? nombre[..40] : nombre; // la columna es NVARCHAR(40)
    }

    public void ProbarConexion()
    {
        using var connection = Open();
    }

    // ───────────────────────── PERFILES ─────────────────────────

    public List<PerfilInfo> ListarPerfiles()
    {
        const string query = @"
            SELECT p.IdPerfil, p.Nombre, p.FechaCreacion, COUNT(g.IdPartida) AS TotalPartidas
            FROM dbo.Perfiles AS p
            LEFT JOIN dbo.Partidas AS g ON g.IdPerfil = p.IdPerfil
            GROUP BY p.IdPerfil, p.Nombre, p.FechaCreacion
            ORDER BY p.Nombre;";
        using var connection = Open();
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        var lista = new List<PerfilInfo>();
        while (reader.Read())
            lista.Add(new PerfilInfo(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3)));
        return lista;
    }

    public PerfilInfo CrearPerfil(string nombre)
    {
        nombre = LimpiarNombre(nombre);
        const string query = @"
            INSERT INTO dbo.Perfiles (Nombre)
            OUTPUT INSERTED.IdPerfil, INSERTED.FechaCreacion
            VALUES (@nombre);";
        using var connection = Open();
        using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@nombre", SqlDbType.NVarChar, 40).Value = nombre;
        try
        {
            using var reader = command.ExecuteReader();
            reader.Read();
            return new PerfilInfo(reader.GetInt32(0), nombre, reader.GetDateTime(1), 0);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new InvalidOperationException($"Ya existe un perfil llamado «{nombre}».");
        }
    }

    /// <summary>Devuelve el perfil con ese nombre; si no existe, lo crea.</summary>
    public PerfilInfo ObtenerOCrearPerfil(string nombre)
    {
        nombre = LimpiarNombre(nombre);
        const string query = "SELECT IdPerfil, Nombre, FechaCreacion FROM dbo.Perfiles WHERE Nombre = @nombre;";
        PerfilInfo? existente = null;
        using (var connection = Open())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 40).Value = nombre;
            using var reader = command.ExecuteReader();
            if (reader.Read())
                existente = new PerfilInfo(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2), 0);
        }
        return existente ?? CrearPerfil(nombre);
    }

    /// <summary>Elimina el perfil y, en cascada, todas sus partidas.</summary>
    public bool EliminarPerfil(int idPerfil)
    {
        using var connection = Open();
        using var command = new SqlCommand("DELETE FROM dbo.Perfiles WHERE IdPerfil = @id;", connection);
        command.Parameters.Add(Int("@id", idPerfil));
        return command.ExecuteNonQuery() > 0;
    }

    // ───────────────────────── PARTIDAS ─────────────────────────

    /// <summary>Guarda una partida nueva para el perfil (inserta en Partidas, ProgresoMisiones y MejorasPartida).</summary>
    public int GuardarPartida(int idPerfil, GameState state)
    {
        const string insertPartida = @"
            INSERT INTO dbo.Partidas
                (IdPerfil, IdMisionActual, Nivel, XP, Vida, MaxVida, Energia, MaxEnergia, Creditos, Botiquines,
                 Eliminaciones, MejorRacha, Fragmentos, Nucleos, InventarioJson)
            OUTPUT INSERTED.IdPartida
            VALUES
                (@perfil, @mision, @nivel, @xp, @vida, @maxVida, @energia, @maxEnergia, @creditos, @botiquines,
                 @eliminaciones, @racha, @fragmentos, @nucleos, @inventario);";
        const string insertProgreso =
            "INSERT INTO dbo.ProgresoMisiones (IdPartida, IdMision, Completada) VALUES (@partida, @mision, @ok);";
        const string insertMejora =
            "INSERT INTO dbo.MejorasPartida (IdPartida, TipoMejora, Nivel) VALUES (@partida, @tipo, @nivel);";

        using var connection = Open();
        using var tx = connection.BeginTransaction();
        try
        {
            int idPartida;
            using (var cmd = new SqlCommand(insertPartida, connection, tx))
            {
                cmd.Parameters.Add(Int("@perfil", idPerfil));
                cmd.Parameters.Add(new SqlParameter("@mision", SqlDbType.TinyInt) { Value = (byte)Math.Clamp(state.Mision + 1, 1, 4) });
                cmd.Parameters.Add(Int("@nivel", Math.Max(1, state.Nivel)));
                cmd.Parameters.Add(Int("@xp", Math.Max(0, state.XP)));
                cmd.Parameters.Add(Int("@vida", Math.Max(0, state.Vida)));
                cmd.Parameters.Add(Int("@maxVida", state.MaxVida));
                cmd.Parameters.Add(Int("@energia", Math.Max(0, state.Energia)));
                cmd.Parameters.Add(Int("@maxEnergia", state.MaxEnergia));
                cmd.Parameters.Add(Int("@creditos", Math.Max(0, state.Creditos)));
                cmd.Parameters.Add(Int("@botiquines", Math.Max(0, state.Botiquines)));
                cmd.Parameters.Add(Int("@eliminaciones", state.Eliminaciones));
                cmd.Parameters.Add(Int("@racha", state.MejorRacha));
                cmd.Parameters.Add(Int("@fragmentos", state.Fragmentos));
                cmd.Parameters.Add(Int("@nucleos", state.Nucleos));
                cmd.Parameters.Add("@inventario", SqlDbType.NVarChar, -1).Value =
                    JsonSerializer.Serialize(state.Inventario ?? new List<string>());
                idPartida = (int)cmd.ExecuteScalar()!;
            }

            for (int i = 0; i < 4; i++)
            {
                bool completada = state.MisionesCompletadas != null
                                  && i < state.MisionesCompletadas.Length
                                  && state.MisionesCompletadas[i];
                using var cmd = new SqlCommand(insertProgreso, connection, tx);
                cmd.Parameters.Add(Int("@partida", idPartida));
                cmd.Parameters.Add(new SqlParameter("@mision", SqlDbType.TinyInt) { Value = (byte)(i + 1) });
                cmd.Parameters.Add(new SqlParameter("@ok", SqlDbType.Bit) { Value = completada });
                cmd.ExecuteNonQuery();
            }

            foreach (var mejora in Mejoras)
            {
                using var cmd = new SqlCommand(insertMejora, connection, tx);
                cmd.Parameters.Add(Int("@partida", idPartida));
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 20).Value = mejora.Tipo;
                cmd.Parameters.Add(new SqlParameter("@nivel", SqlDbType.TinyInt)
                {
                    Value = (byte)Math.Clamp(mejora.Leer(state), 0, 10)
                });
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return idPartida;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>Lista las partidas de un perfil, de la más reciente a la más antigua.</summary>
    public List<PartidaInfo> ListarPartidas(int idPerfil)
    {
        const string query = @"
            SELECT g.IdPartida, g.IdPerfil, p.Nombre, g.IdMisionActual, m.Planeta, g.Nivel, g.Creditos, g.FechaGuardado
            FROM dbo.Partidas AS g
            JOIN dbo.Perfiles AS p ON p.IdPerfil = g.IdPerfil
            JOIN dbo.Misiones AS m ON m.IdMision = g.IdMisionActual
            WHERE g.IdPerfil = @perfil
            ORDER BY g.FechaGuardado DESC, g.IdPartida DESC;";
        using var connection = Open();
        using var command = new SqlCommand(query, connection);
        command.Parameters.Add(Int("@perfil", idPerfil));
        using var reader = command.ExecuteReader();
        var lista = new List<PartidaInfo>();
        while (reader.Read())
        {
            lista.Add(new PartidaInfo(
                reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetByte(3),
                reader.GetString(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetDateTime(7)));
        }
        return lista;
    }

    /// <summary>Reconstruye el GameState de una partida leyendo las 5 tablas. Devuelve null si ya no existe.</summary>
    public GameState? CargarPartida(int idPartida)
    {
        const string queryPartida = @"
            SELECT p.Nombre, g.IdMisionActual, g.Nivel, g.XP, g.Vida, g.MaxVida, g.Energia, g.MaxEnergia,
                   g.Creditos, g.Botiquines, g.Eliminaciones, g.MejorRacha, g.Fragmentos, g.Nucleos, g.InventarioJson
            FROM dbo.Partidas AS g
            JOIN dbo.Perfiles AS p ON p.IdPerfil = g.IdPerfil
            WHERE g.IdPartida = @id;";
        const string queryProgreso = "SELECT IdMision, Completada FROM dbo.ProgresoMisiones WHERE IdPartida = @id;";
        const string queryMejoras = "SELECT TipoMejora, Nivel FROM dbo.MejorasPartida WHERE IdPartida = @id;";

        using var connection = Open();
        var state = new GameState();

        using (var cmd = new SqlCommand(queryPartida, connection))
        {
            cmd.Parameters.Add(Int("@id", idPartida));
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            state.Nombre = reader.GetString(0);
            state.Mision = reader.GetByte(1) - 1;
            state.Nivel = reader.GetInt32(2);
            state.XP = reader.GetInt32(3);
            state.Vida = reader.GetInt32(4);
            state.MaxVida = reader.GetInt32(5);
            state.Energia = reader.GetInt32(6);
            state.MaxEnergia = reader.GetInt32(7);
            state.Creditos = reader.GetInt32(8);
            state.Botiquines = reader.GetInt32(9);
            state.Eliminaciones = reader.GetInt32(10);
            state.MejorRacha = reader.GetInt32(11);
            state.Fragmentos = reader.GetInt32(12);
            state.Nucleos = reader.GetInt32(13);
            state.Inventario = JsonSerializer.Deserialize<List<string>>(reader.GetString(14)) ?? new List<string>();
        }

        using (var cmd = new SqlCommand(queryProgreso, connection))
        {
            cmd.Parameters.Add(Int("@id", idPartida));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int mision = reader.GetByte(0);
                if (mision >= 1 && mision <= state.MisionesCompletadas.Length)
                    state.MisionesCompletadas[mision - 1] = reader.GetBoolean(1);
            }
        }

        using (var cmd = new SqlCommand(queryMejoras, connection))
        {
            cmd.Parameters.Add(Int("@id", idPartida));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string tipo = reader.GetString(0);
                int nivel = reader.GetByte(1);
                foreach (var mejora in Mejoras)
                {
                    if (mejora.Tipo == tipo) mejora.Escribir(state, nivel);
                }
            }
        }

        return state;
    }

    /// <summary>Todas las partidas de todos los perfiles (la usa la pantalla ELIMINAR PARTIDA del menú).</summary>
    public List<PartidaResumen> ListarPartidas()
    {
        const string query = @"
            SELECT g.IdPartida, p.Nombre, g.IdMisionActual, g.FechaGuardado
            FROM dbo.Partidas AS g
            JOIN dbo.Perfiles AS p ON p.IdPerfil = g.IdPerfil
            ORDER BY g.FechaGuardado DESC, g.IdPartida DESC;";
        using var connection = Open();
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        var lista = new List<PartidaResumen>();
        while (reader.Read())
        {
            lista.Add(new PartidaResumen
            {
                IdPartida = reader.GetInt32(0),
                Explorador = reader.GetString(1),
                Mision = reader.GetByte(2),
                FechaGuardado = reader.GetDateTime(3)
            });
        }
        return lista;
    }

    /// <summary>La partida guardada más reciente de cualquier perfil (la usa ARCHIVO Y BASE DE DATOS SQL).</summary>
    public GameState? CargarUltima()
    {
        const string query = "SELECT TOP (1) IdPartida FROM dbo.Partidas ORDER BY FechaGuardado DESC, IdPartida DESC;";
        int? id;
        using (var connection = Open())
        using (var command = new SqlCommand(query, connection))
        {
            object? resultado = command.ExecuteScalar();
            id = resultado is int valor ? valor : null;
        }
        return id.HasValue ? CargarPartida(id.Value) : null;
    }

    /// <summary>Elimina una partida; su progreso y sus mejoras se borran en cascada.</summary>
    public bool EliminarPartida(int idPartida)
    {
        using var connection = Open();
        using var command = new SqlCommand("DELETE FROM dbo.Partidas WHERE IdPartida = @id;", connection);
        command.Parameters.Add(Int("@id", idPartida));
        return command.ExecuteNonQuery() > 0;
    }
}

/// <summary>Resumen de una partida guardada en SQL Server (para la pantalla de eliminar partidas).</summary>
public sealed class PartidaResumen
{
    public int IdPartida { get; set; }
    public string Explorador { get; set; } = "";
    public int Mision { get; set; }
    public DateTime FechaGuardado { get; set; }
}