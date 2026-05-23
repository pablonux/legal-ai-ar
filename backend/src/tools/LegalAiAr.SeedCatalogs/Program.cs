// Seed Catalogs: loads CSJN reference data (courts, judges) from csjn-catalogs.json into Azure SQL.
// Idempotent: skips rows that already exist (matched by name).
//
// Usage: dotnet run [--dry-run]
// Env: AzureSql__ConnectionString

using System.Text.Json;
using Microsoft.Data.SqlClient;

LoadEnvFile();

var dryRun = args.Contains("--dry-run");
Console.WriteLine($"CSJN Seed Catalogs{(dryRun ? " [DRY-RUN]" : "")}");
Console.WriteLine();

var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(sqlConn))
{
    Console.WriteLine("ERROR: AzureSql__ConnectionString is not configured.");
    return 1;
}

var catalogPath = Path.Combine(AppContext.BaseDirectory, "csjn-catalogs.json");
if (!File.Exists(catalogPath))
{
    catalogPath = Path.Combine(Directory.GetCurrentDirectory(), "csjn-catalogs.json");
}
if (!File.Exists(catalogPath))
{
    Console.WriteLine($"ERROR: csjn-catalogs.json not found.");
    return 1;
}

var json = await File.ReadAllTextAsync(catalogPath);
using var doc = JsonDocument.Parse(json);
var root = doc.RootElement;

await using var conn = new SqlConnection(sqlConn);
await conn.OpenAsync();

var courtsSeeded = 0;
var courtsSkipped = 0;

if (root.TryGetProperty("courts", out var courts))
{
    Console.WriteLine("Seeding courts...");
    foreach (var court in courts.EnumerateArray())
    {
        var name = court.GetProperty("name").GetString()!;
        var jurisdictionArea = court.GetProperty("jurisdictionArea").GetString() ?? "";
        var territory = court.GetProperty("territory").GetString() ?? "";
        var instance = court.GetProperty("instance").GetString() ?? "";

        var exists = await ExistsAsync(conn,
            "SELECT COUNT(*) FROM Courts WHERE Name = @name",
            ("@name", name));

        if (exists)
        {
            courtsSkipped++;
            continue;
        }

        if (!dryRun)
        {
            await ExecuteAsync(conn,
                "INSERT INTO Courts (Name, JurisdictionArea, Territory, Instance) VALUES (@name, @ja, @t, @i)",
                ("@name", name), ("@ja", jurisdictionArea), ("@t", territory), ("@i", instance));
        }
        courtsSeeded++;
        Console.WriteLine($"  + {name}");
    }
    Console.WriteLine($"  Courts: {courtsSeeded} inserted, {courtsSkipped} already existed\n");
}

var judgesSeeded = 0;
var judgesSkipped = 0;

if (root.TryGetProperty("judges", out var judges))
{
    var csjnCourtId = await GetScalarAsync<int?>(conn,
        "SELECT Id FROM Courts WHERE Name = 'Corte Suprema de Justicia de la Nación'");

    Console.WriteLine("Seeding judges...");
    foreach (var judge in judges.EnumerateArray())
    {
        var lastName = judge.GetProperty("lastName").GetString()!;
        var firstName = judge.GetProperty("firstName").GetString() ?? "";

        var exists = await ExistsAsync(conn,
            "SELECT COUNT(*) FROM Judges WHERE LastName = @ln AND FirstName = @fn",
            ("@ln", lastName), ("@fn", firstName));

        if (exists)
        {
            judgesSkipped++;
            continue;
        }

        if (!dryRun)
        {
            await ExecuteAsync(conn,
                "INSERT INTO Judges (FirstName, LastName, CurrentCourtId) VALUES (@fn, @ln, @cid)",
                ("@fn", firstName), ("@ln", lastName),
                ("@cid", csjnCourtId.HasValue ? (object)csjnCourtId.Value : DBNull.Value));
        }
        judgesSeeded++;
        Console.WriteLine($"  + {lastName}, {firstName}");
    }
    Console.WriteLine($"  Judges: {judgesSeeded} inserted, {judgesSkipped} already existed\n");
}

Console.WriteLine("Done.");
return 0;

static async Task<bool> ExistsAsync(SqlConnection conn, string sql, params (string name, object value)[] parameters)
{
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    foreach (var (name, value) in parameters)
        cmd.Parameters.AddWithValue(name, value);
    var count = (int)(await cmd.ExecuteScalarAsync())!;
    return count > 0;
}

static async Task ExecuteAsync(SqlConnection conn, string sql, params (string name, object value)[] parameters)
{
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    foreach (var (name, value) in parameters)
        cmd.Parameters.AddWithValue(name, value);
    await cmd.ExecuteNonQueryAsync();
}

static async Task<T?> GetScalarAsync<T>(SqlConnection conn, string sql, params (string name, object value)[] parameters)
{
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    foreach (var (name, value) in parameters)
        cmd.Parameters.AddWithValue(name, value);
    var result = await cmd.ExecuteScalarAsync();
    if (result is null or DBNull) return default;
    return (T)result;
}

static void LoadEnvFile()
{
    var dir = Directory.GetCurrentDirectory();
    while (dir is not null)
    {
        var envPath = Path.Combine(dir, ".env");
        if (File.Exists(envPath))
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                    continue;
                var eqIdx = trimmed.IndexOf('=');
                if (eqIdx <= 0)
                    continue;
                var key = trimmed[..eqIdx].Trim();
                var val = trimmed[(eqIdx + 1)..].Trim().Trim('"');
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                    Environment.SetEnvironmentVariable(key, val);
            }
            return;
        }
        dir = Directory.GetParent(dir)?.FullName;
    }
}
