using System.IO;

using Autofac;

using ScreenReaderService.Util;

namespace ScreenReaderService.Data.Services
{
    public class ObjectFileMappingService<T> where T : class, new()
    {
        private FileManager FileManager = DependencyHolder.Dependencies.Resolve<FileManager>();

        private string Folder { get; set; }
        private string Filename { get; set; }

        private string FilePath => Path.Combine(Folder, Filename);

        public ObjectFileMappingService(string folder, string filename)
        {
            Folder = folder;
            Filename = filename;

            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }

        private T data = null;
        protected T Data
        {
            get
            {
                if (data == null)
                    data = FileManager.Load<T>(FilePath);

                if (data == null)
                    data = new T();

                return data;
            }
            set
            {
                data = value;
                Save();
            }
        }

        public void Save()
        {
            FileManager.Save(FilePath, data);
        }
    }
}
