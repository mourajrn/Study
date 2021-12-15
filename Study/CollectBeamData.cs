﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    [Transaction(TransactionMode.Manual)]
    public class CollectBeamData : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Element> beams = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_StructuralFraming).ToList();

            using (StreamWriter streamWriter = new StreamWriter("C:\\Users\\Pichau\\source\\repos\\Study\\Study\\bin\\Debug\\Output.txt", false))
            {
                foreach (Element beam in beams)
                {
                    string floor = beam.LookupParameter("Pavimento").AsString();
                    
                    double length = UnitUtils.ConvertFromInternalUnits(beam.LookupParameter("Comprimento").AsDouble(), UnitTypeId.Meters);

                    streamWriter.WriteLine($"{beam.Id};{floor};{length}");
                }

            }

            return Result.Succeeded;
        }
    }
}
