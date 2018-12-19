using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RailwayPlanner
{
    public class CheckRailCurveInclination : GH_Component
    {


        /// <summary>
        /// Initializes a new instance of the CheckRailCurveRadius class.
        /// </summary>
        public CheckRailCurveInclination()
          : base("CheckRailCurveInclination", "InclinationCheck",
              "Checks Rail Curve for maximal inclination",
              "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve To Check", GH_ParamAccess.item);
            pManager.AddNumberParameter("Curve Scale", "Scale", "Height Scale of Curve, normal 10", GH_ParamAccess.item, 10);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Checked Curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            double sFac = 10;
            if (!DA.GetData(0, ref crv)) return;
            if (!DA.GetData(1, ref sFac)) return;

            double maxInclination = sFac * _maxInclination;

            List<Point3d> locations = new List<Point3d>();
            List<double> inclinations = new List<double>();


            if (crv.IsPolyline())
            {
                Polyline pLine = null;
                if (crv.TryGetPolyline(out pLine))
                {
                    for (int i = 0; i < pLine.Count - 1; i++)
                    {
                        Point3d p1 = pLine[i];
                        Point3d p2 = pLine[i + 1];
                        Vector3d vec1 = new Vector3d(p2 - p1);
                        double inclination = Math.Abs(vec1.Z / vec1.Y)/ sFac;
                        if (inclination > maxInclination)
                        {
                            inclinations.Add(inclination);
                            locations.Add(0.5*(p1+p2));
                        }
                    }
                }
            }

            else
            {
                double[] parameters = crv.DivideByCount(divisions, true);
                foreach (double param in parameters)
                {
                    Vector3d tangentVector = crv.TangentAt(param);
                    try
                    {
                        double inclination = Math.Abs(tangentVector.Z / tangentVector.Y)/sFac;
                        if (inclination > _maxInclination)
                        {
                            inclinations.Add(inclination);
                            locations.Add(crv.PointAt(param));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            DA.SetData(0, crv);
            _positions = locations;
            _inclinations = inclinations;
        }
        private double _maxInclination = 0.025;
        private int divisions = 50;
        private List<Point3d> _positions = new List<Point3d>();
        private List<double> _inclinations = new List<double>();

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Color color1 = Color.SteelBlue;
            Color color2 = Color.LightSkyBlue;
            Color colorGrey = Color.DimGray;
            Color colorFrameColumn = Color.SteelBlue;
            Color colorRed = Color.Firebrick;

            // Display Radius
            for (int i = 0; i < _positions.Count; i++)
            {
                string dispText = "Inclination to big r = " + ((int)(_inclinations[i]*1000)).ToString() + " ‰";
                Rhino.RhinoApp.WriteLine(args.Display.Viewport.Name);
                if (args.Display.Viewport.Name == "Right")
                {
                    args.Display.Draw2dText(dispText, color1, Point3d.Add(_positions[i], new Point3d(2, -2, 0)), false, 14);
                    args.Display.DrawPoint(_positions[i], color1);
                }                    
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("873b4311-921b-4b87-94e2-adc3120e438b"); }
        }
    }
}