
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

namespace RevitLevelsPlans.Assignment14.ViewModel
{
    public class DoorSizeViewModel
    {
        private readonly Document _doc;
        private readonly List<FamilyInstance> _doors;

        // User inputs (in document display units, e.g., mm or inches)
        public double? NewWidth { get; set; }
        public double? NewHeight { get; set; }

        // Control whether to write Instance or Type parameter (if available)
        public bool ApplyToInstance { get; set; } = true;
        public bool ApplyToType { get; set; } = false; // caution: affects all instances of a type

        // Summary for UI
        public string Summary { get; }

        public DoorSizeViewModel(Document doc, IEnumerable<FamilyInstance> doors)
        {
            _doc = doc;
            _doors = doors?.ToList() ?? new List<FamilyInstance>();
            Summary = $"Doors selected: {_doors.Count}";
        }

        /// <summary>
        /// Change size parameters on all selected doors, respecting project units.
        /// </summary>
        public void ApplyChanges()
        {
            if (NewWidth == null && NewHeight == null) return;

            // Get project units (length display units) → convert to internal feet
            Units units = _doc.GetUnits();
            var foLen = units.GetFormatOptions(SpecTypeId.Length);
            var unitId = foLen.GetUnitTypeId(); // ForgeTypeId of current length display unit
            // Convert provided values to internal units (feet)
            double? wInternal = (NewWidth != null) ? UnitUtils.ConvertToInternalUnits(NewWidth.Value, unitId) : (double?)null;
            double? hInternal = (NewHeight != null) ? UnitUtils.ConvertToInternalUnits(NewHeight.Value, unitId) : (double?)null;
            // Revit stores LENGTH in FEET internally; UnitUtils is the correct, precise way to convert. [7](https://help.autodesk.com/cloudhelp/2024/PTB/Revit-API/files/Revit_API_Developers_Guide/Introduction/Application_and_Document/Revit_API_Revit_API_Developers_Guide_Introduction_Application_and_Document_Units_html.html)[5](https://revapidocs.com/2025/b5e8d065-d274-62f8-7b5d-89722f7c44f3.htm)

            using (var t = new Transaction(_doc, "Set Door Width/Height"))
            {
                t.Start();

                foreach (var fi in _doors)
                {
                    // Instance first (if requested)
                    if (ApplyToInstance)
                        SetSizeOnInstance(fi, wInternal, hInternal);

                    // Type next (if requested)
                    if (ApplyToType)
                    {
                        var type = _doc.GetElement(fi.GetTypeId()) as ElementType;
                        if (type != null)
                            SetSizeOnType(type, wInternal, hInternal);
                    }
                }

                t.Commit();
            }
        }

        private void SetSizeOnInstance(FamilyInstance fi, double? wInternal, double? hInternal)
        {
            // Try typical instance parameter names (Width/Height); otherwise shared/custom.
            var pW = fi.LookupParameter("Width");
            var pH = fi.LookupParameter("Height");

            // If not found or read-only, skip that one
            if (wInternal != null && pW != null && !pW.IsReadOnly && pW.StorageType == StorageType.Double)
                pW.Set(wInternal.Value);

            if (hInternal != null && pH != null && !pH.IsReadOnly && pH.StorageType == StorageType.Double)
                pH.Set(hInternal.Value);
        }

        private void SetSizeOnType(ElementType type, double? wInternal, double? hInternal)
        {
            var pW = type.LookupParameter("Width");
            var pH = type.LookupParameter("Height");

            if (wInternal != null && pW != null && !pW.IsReadOnly && pW.StorageType == StorageType.Double)
                pW.Set(wInternal.Value);

            if (hInternal != null && pH != null && !pH.IsReadOnly && pH.StorageType == StorageType.Double)
                pH.Set(hInternal.Value);
        }
    }
}
