using System.IO;

using Newtonsoft.Json;

namespace ScreenReaderService.Util
{
    public class FileManager
    {
        public void Save(string path, object data)
        {
            string json = JsonConvert.SerializeObject(data);

            using (StreamWriter strw = new StreamWriter(path))
                strw.Write(json);
        }

        public T Load<T>(string path) where T : class
        {
            if (!File.Exists(path))
                return null;

            using (StreamReader str = new StreamReader(path))
            {
                string json = str.ReadToEnd();

                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
