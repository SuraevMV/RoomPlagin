using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlagin
{
    [Transaction(TransactionMode.Manual)]
    public class AddRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts = new Transaction(doc, "Добавление помещений");
            ts.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in listLevel)
            {
                rooms = doc.Create.NewRooms2(level);
            }      
            
            FilteredElementCollector filterRooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomsId = filterRooms.ToElementIds() as IList<ElementId>;

            foreach (ElementId roomId in roomsId)
            {
                Element e = doc.GetElement(roomId);
                Room r = e as Room;
                string levelName = r.Level.Name.Substring(6);
                r.Name = $"{levelName}_{r.Number}";

                doc.Create.NewRoomTag(new LinkElementId(roomId), new UV(0, 0), null);
            }

            ts.Commit();

            return Result.Succeeded;
        }
    }
}
