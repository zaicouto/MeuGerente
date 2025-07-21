namespace Shared.Helpers;

public static class PathHelpers
{
    /// <summary>
    /// Go up directories until you find the .sln file or .git folder, marking the root of the solution.
    /// </summary>
    /// <returns>Absolute path to the root of the solution.</returns>
    public static string GetSolutionRootPath()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (
            dir != null
            && !Directory.Exists(Path.Combine(dir.FullName, ".git"))
            && !File.Exists(Path.Combine(dir.FullName, "MeuGerente.sln"))
        )
        {
            dir = dir.Parent;
        }

        if (dir == null)
        {
            throw new Exception(
                "Root of solution not found. Make sure you're running inside the solution folder."
            );
        }

        Console.WriteLine("Root directory is " + dir.FullName);
        return dir.FullName;
    }
}
