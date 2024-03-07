namespace CompanyHouse.Utilities
{
    public class FileUtil {
        public string[] GetAllFilePathsInDirectory(string directoryPath, string fileType)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"The specified directory does not exist: {directoryPath}");
                // return;
            }
            Console.WriteLine($"Reading files in: {directoryPath}");
            string[] filePaths = Directory.GetFiles(directoryPath, $"*.{fileType}");
            if (filePaths?.Length > 0)
            {
                Console.WriteLine($"The specified directory contains no files.");
               //  return;
            }
            return filePaths;
        }
    }
}