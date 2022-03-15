using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ShapesEditor
{
    public interface ISessionManager
    {
        (ObservableCollection<ShapeVM>, Brush) RestoreLastSession();
        void SaveSession(ObservableCollection<ShapeVM> shapes, Brush backgroundColor);
    }
}
