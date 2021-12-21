using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Study
{
    [Transaction(TransactionMode.Manual)]
    public class TagOpenings : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Opening> openings = new FilteredElementCollector(doc, uidoc.ActiveView.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_StructuralFramingOpening).Cast<Opening>().ToList();

            TextNoteType textNoteType = new FilteredElementCollector(doc).WhereElementIsElementType().OfClass(typeof(TextNoteType)).Cast<TextNoteType>().Where(t => t.Name.Equals("2mm")).FirstOrDefault();

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Aplicar identificação das aberturas");

                foreach (Opening opening in openings)
                {
                    CurveArray curveArray = opening.BoundaryCurves;

                    List<Arc> arcs = new List<Arc>();

                    foreach (Arc curve in curveArray)
                        arcs.Add(curve);

                    Arc arc = arcs.First();

                    double centerElevation = UnitUtils.ConvertFromInternalUnits(arc.Center.Z, UnitTypeId.Centimeters);

                    double diameter = Math.Round(UnitUtils.ConvertFromInternalUnits(arc.Radius * 2.0, UnitTypeId.Millimeters), 0);

                    Element host = opening.Host;

                    double hostBottomElevation = UnitUtils.ConvertFromInternalUnits(host.LookupParameter("Elevação na parte inferior").AsDouble(), UnitTypeId.Centimeters);

                    double bottomDisplacement = Math.Round(centerElevation - hostBottomElevation - 0.05 * diameter, 0);

                    TextNoteOptions textNoteOptions = new TextNoteOptions();
                    textNoteOptions.TypeId = textNoteType.Id;

                    TextNote textNote;

                    if ((host as FamilyInstance).FacingOrientation.X.IsAlmostEquals(0))
                    {
                        textNoteOptions.Rotation = 0;
                        textNote = TextNote.Create(doc, uidoc.ActiveView.Id, arc.Center.Add(new XYZ(0, UnitUtils.ConvertToInternalUnits(10, UnitTypeId.Centimeters), 0)), $"Furo\nØ{diameter}\nH{bottomDisplacement}", textNoteOptions);
                    }
                    else
                    {
                        textNoteOptions.Rotation = 0.5 * Math.PI;
                        textNote = TextNote.Create(doc, uidoc.ActiveView.Id, arc.Center.Add(new XYZ(UnitUtils.ConvertToInternalUnits(-10, UnitTypeId.Centimeters), 0, 0)), $"Furo\nØ{diameter}\nH{bottomDisplacement}", textNoteOptions);
                    }

                    textNote.HorizontalAlignment = HorizontalTextAlignment.Center;
                    textNote.VerticalAlignment = VerticalTextAlignment.Bottom;
                }

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}
