using System.Reflection;

namespace LegalAiAr.Worker.Parser.Tests.Fixtures;

/// <summary>
/// Loads fixture files from the Fixtures folder in the test output directory.
/// Fixtures are copied to output via CopyToOutputDirectory in the csproj.
/// </summary>
internal static class FixtureLoader
{
    private static string? _fixturesPath;

    private static string FixturesPath
    {
        get
        {
            if (_fixturesPath != null)
                return _fixturesPath;

            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? AppContext.BaseDirectory;
            _fixturesPath = Path.Combine(assemblyDir, "Fixtures");

            if (!Directory.Exists(_fixturesPath))
            {
                // Fallback: try relative to test project (e.g. when running from solution dir)
                var projectDir = Path.Combine(assemblyDir, "..", "..", "..");
                _fixturesPath = Path.GetFullPath(Path.Combine(projectDir, "Fixtures"));
            }

            return _fixturesPath;
        }
    }

    /// <summary>
    /// Loads a JSON fixture file and returns its content as string.
    /// </summary>
    public static string LoadJson(string fileName)
    {
        var path = Path.Combine(FixturesPath, fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Fixture not found: {path}", path);
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Loads a binary fixture file and returns its bytes.
    /// </summary>
    public static byte[] LoadBytes(string fileName)
    {
        var path = Path.Combine(FixturesPath, fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Fixture not found: {path}", path);
        return File.ReadAllBytes(path);
    }

    /// <summary>
    /// Loads a fixture file and returns a stream. Caller must dispose.
    /// </summary>
    public static Stream LoadStream(string fileName)
    {
        var path = Path.Combine(FixturesPath, fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Fixture not found: {path}", path);
        return File.OpenRead(path);
    }
}
