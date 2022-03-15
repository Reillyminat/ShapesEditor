using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

namespace ShapesEditor
{
    public class SessionManager : ISessionManager
    {
        private readonly string _path;
        public SessionManager(string path)
        {
            _path = path;
        }

        public (ObservableCollection<ShapeVM>, Brush) RestoreLastSession()
        {
            ObservableCollection<ShapeVM> shapes = new ObservableCollection<ShapeVM>();
            Brush backgroundColor = default;
            if (File.Exists(_path))
            {
                using (var file = File.OpenText(_path))
                {
                    var data = file.ReadToEnd();
                    var deserializedSessionState = JsonConvert.DeserializeAnonymousType(data, new { shapes, backgroundColor });
                    return (deserializedSessionState.shapes, deserializedSessionState.backgroundColor);
                }
            }
            else
            {
                return default;
            }
        }

        public void SaveSession(ObservableCollection<ShapeVM> shapes, Brush backgroundColor)
        {
            using (StreamWriter file = File.CreateText(@"../../../SessionData.json"))
            {
                file.Write(JsonConvert.SerializeObject(new { shapes, backgroundColor }));
            }
        }
    }
}
