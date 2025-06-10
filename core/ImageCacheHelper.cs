using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace gizindir.helpers
{
    public static class ImageCacheHelper
    {
        private static readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GizindirApp", "ImageCache");

        static ImageCacheHelper()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            // Cache klasörünü oluştur
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        public static async Task<Image> LoadImageAsync(string imageUrl, Image placeholderImage = null)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return placeholderImage;

            try
            {
                // Önce cache'den kontrol et
                if (_imageCache.ContainsKey(imageUrl))
                {
                    return _imageCache[imageUrl];
                }

                // Disk cache'den kontrol et
                string fileName = GetCacheFileName(imageUrl);
                string filePath = Path.Combine(_cacheDirectory, fileName);

                if (File.Exists(filePath))
                {
                    var cachedImage = Image.FromFile(filePath);
                    _imageCache[imageUrl] = cachedImage;
                    return cachedImage;
                }

                // İnternetten indir
                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

                using (var stream = new MemoryStream(imageBytes))
                {
                    var image = Image.FromStream(stream);

                    // Memory cache'e ekle
                    _imageCache[imageUrl] = image;

                    // Disk cache'e kaydet (async olarak)
                    _ = Task.Run(() => SaveToDiskCache(imageBytes, filePath));

                    return image;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Resim yükleme hatası: {ex.Message}");
                return placeholderImage;
            }
        }

        private static string GetCacheFileName(string url)
        {
            // URL'den güvenli dosya adı oluştur
            int hash = url.GetHashCode();
            return $"cached_image_{Math.Abs(hash)}.jpg";

        }

        private static void SaveToDiskCache(byte[] imageBytes, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, imageBytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Disk cache kaydetme hatası: {ex.Message}");
            }
        }

        public static void ClearCache()
        {
            // Memory cache'i temizle
            foreach (var image in _imageCache.Values)
            {
                image?.Dispose();
            }
            _imageCache.Clear();

            // Disk cache'i temizle
            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    var files = Directory.GetFiles(_cacheDirectory);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cache temizleme hatası: {ex.Message}");
            }
        }

        public static void Dispose()
        {
            ClearCache();
            _httpClient?.Dispose();
        }
    }
}