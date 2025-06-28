namespace WebAPI.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string category);
        Task<byte[]> GetFileAsync(string relativeFilePath);
    }

    public class FileService : IFileService
    {
        private readonly string _uploadsRoot;

        public FileService(IWebHostEnvironment env)
        {
            _uploadsRoot = Path.Combine(env.WebRootPath, "uploads");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string category)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo no válido.");

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("La categoría no puede ser vacía.");

            var categoryPath = Path.Combine(_uploadsRoot, category);
            if (!Directory.Exists(categoryPath))
                Directory.CreateDirectory(categoryPath);

            var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(categoryPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + Path.Combine(category, uniqueFileName).Replace("\\", "/");
        }

        public async Task<byte[]> GetFileAsync(string relativeFilePath)
        {
            var fullPath = Path.Combine(_uploadsRoot, relativeFilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Archivo no encontrado.", fullPath);

            return await File.ReadAllBytesAsync(fullPath);
        }
    }
}