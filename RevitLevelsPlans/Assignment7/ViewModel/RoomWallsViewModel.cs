
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using RevitLevelsPlans.Assignment7.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RevitLevelsPlans.Assignment7.ViewModel
{
    public class RoomWallsViewModel
    {
        public string RoomName { get; }
        public string RoomNumber { get; }
        public string RoomLevel { get; }

        public ObservableCollection<WallInRoomModel> Walls { get; } = new ObservableCollection<WallInRoomModel>();

        public RoomWallsViewModel(Document doc, Room room)
        {
            RoomName = room.Name;
            RoomNumber = room.Number;
            var lvl = doc.GetElement(room.LevelId) as Level;
            RoomLevel = lvl?.Name ?? "(none)";

            // Room boundary segments (Finish face)
            var opt = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            };

            var loops = room.GetBoundarySegments(opt);
            if (loops == null || loops.Count == 0)
                return;

            var wallIds = new HashSet<ElementId>();

            foreach (IList<BoundarySegment> loop in loops)
            {
                foreach (BoundarySegment seg in loop)
                {
                    // 1) Try local model element
                    ElementId localId = seg.ElementId;
                    if (localId != ElementId.InvalidElementId)
                    {
                        Element neighbor = doc.GetElement(localId);
                        if (neighbor is Wall wLocal)
                        {
                            wallIds.Add(wLocal.Id);
                            continue; // done with this segment
                        }

                        // Some boundaries are ModelCurve (Room Separation Line), Floors, Roofs etc.
                        // Skip non-walls.
                    }

                    // 2) Try linked element (if boundary comes from a link)
                    ElementId linkInstId = seg.LinkElementId;
                    if (linkInstId != ElementId.InvalidElementId)
                    {
                        var linkInst = doc.GetElement(linkInstId) as RevitLinkInstance;
                        var linkDoc = linkInst?.GetLinkDocument();
                        if (linkDoc != null)
                        {
                            // Boundaries in link return the linked element id through LinkElementId
                            Element linkedElem = linkDoc.GetElement(seg.ElementId); // In link context, ElementId refers to linked doc element id
                            if (linkedElem is Wall wLinked)
                            {
                                // We cannot add cross-doc ids to local doc directly; store the link instance id to keep uniqueness.
                                // Here, we add the link instance id (host side) as representative. Alternatively, track pair (linkInstId, linkedElem.Id).
                                wallIds.Add(linkInstId); // or use a separate structure to display linked wall name
                                // If you want the linked wall's display info, handle below when adding rows.
                            }
                        }
                    }
                }
            }

            // Build UI rows (local walls).
            foreach (var wid in wallIds.OrderBy(id => id.IntegerValue))
            {
                var w = doc.GetElement(wid) as Wall;
                if (w == null)
                {
                    // This might be a linked wall case if we stored linkInstId above; skip or handle specially.
                    // Optional: resolve linked name via RevitLinkInstance name.
                    var linkInst = doc.GetElement(wid) as RevitLinkInstance;
                    if (linkInst != null)
                    {
                        Walls.Add(new WallInRoomModel
                        {
                            Id = wid.IntegerValue,
                            Name = $"Linked: {linkInst.Name}",
                            Type = "(linked)",
                            Level = "(linked)",
                            Length = "(linked)",
                            Thickness = "(linked)"
                        });
                    }
                    continue;
                }

                var wt = doc.GetElement(w.GetTypeId()) as WallType;
                var wl = doc.GetElement(w.LevelId) as Level;

                // Length (formatted if available)
                var lenP = w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                var lenStr = lenP?.AsValueString();
                if (string.IsNullOrEmpty(lenStr) && lenP != null)
                    lenStr = lenP.AsDouble().ToString("0.##");

                // Thickness (WallType width)
                var widthP = wt?.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
                var thickStr = widthP?.AsValueString();
                if (string.IsNullOrEmpty(thickStr) && widthP != null)
                    thickStr = widthP.AsDouble().ToString("0.###");

                Walls.Add(new WallInRoomModel
                {
                    Id = w.Id.IntegerValue,
                    Name = w.Name,
                    Type = wt?.Name ?? "(none)",
                    Level = wl?.Name ?? "(none)",
                    Length = lenStr ?? "(none)",
                    Thickness = thickStr ?? "(none)"
                });
            }

            // Optional: sort for better UX
            var ordered = Walls.OrderBy(x => x.Level).ThenBy(x => x.Id).ToList();
            Walls.Clear();
            foreach (var x in ordered) Walls.Add(x);
        }
    }
}