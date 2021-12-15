using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Element> beams = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_StructuralFraming).ToList();

            List<ElementId> beamsId = beams.Select(b => b.Id).ToList();

            FamilySymbol familySymbol = new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_StructuralFraming).Cast<FamilySymbol>().Where(fs => fs.Name == "20,0 x 50,0").FirstOrDefault();

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Altera o tipo das vigas");

                Element.ChangeTypeId(doc, beamsId, familySymbol.Id);

                trans.Commit();
            }


            return Result.Succeeded;
        }
    }
}
