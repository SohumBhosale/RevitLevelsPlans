
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace WallInspector
{
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem?.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_Walls;
        }

        public bool AllowReference(Reference reference, XYZ position) => true;
    }
}
