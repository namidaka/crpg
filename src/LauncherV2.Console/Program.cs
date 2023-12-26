using LauncherV3.LauncherHelper;
internal static class Program
{
    private static bool hash;
    private static string hashFolderPath = string.Empty;
    private static string hashOutputPath = string.Empty;
    static void Main(string[] args)
    {
        ProcessArgs(args);
        Console.WriteLine($"Hashing now {hashFolderPath} , it should take less than 60s");
        var hashXml = CrpgHashMethods.GenerateCrpgFolderHashMap(hashFolderPath).GetAwaiter().GetResult();
        hashXml.Save(hashOutputPath);
        Console.WriteLine($"hashing done, hash can be found at {hashOutputPath}");
    }

    static void ProcessArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "/hash")
            {
                hash = true;
                hashFolderPath = args[i + 1];
            }

            if (args[i] == "/output")
            {
                hashOutputPath = args[i + 1];
            }
        }
    }
}
