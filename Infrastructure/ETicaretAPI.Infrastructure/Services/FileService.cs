using ETicaretAPI.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }       

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, path);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            List<(string fileName, string path)> datas = new();
            List<bool> results = new();

            foreach (IFormFile file in files)
            {
                string newFileName = await FileRenameAsync(file.FileName, path);

                bool result = await CopyFileAsync($"{uploadPath}\\{newFileName}",file);
                datas.Add((newFileName, $"{uploadPath}\\{newFileName}"));
                results.Add(result);
            }
            
            if(results.TrueForAll(r => r.Equals(true)))
            {
                return datas;
            }
            return null;
        }
        
        public Task<string> FileRenameAsync(string fileName, string path)
        {
            fileName = fileName.Replace("ç", "c")
                               .Replace("ğ", "g")
                               .Replace("ı", "i")
                               .Replace("ö", "o")
                               .Replace("ş", "s")
                               .Replace("ü", "u")
                               .Replace("İ", "I");

            fileName = fileName.Replace(" ", "-");

            string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, path);
            string filePath = Path.Combine(uploadPath, fileName);
            int count = 1;

            while (File.Exists(filePath))
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                nameWithoutExtension = nameWithoutExtension.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                count++;

                nameWithoutExtension = nameWithoutExtension + count.ToString();

                fileName = nameWithoutExtension + extension;
                filePath = Path.Combine(uploadPath, fileName);
            }

            // Yeni dosya ismini döndür
            return Task.FromResult(fileName);
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        
    }
}
