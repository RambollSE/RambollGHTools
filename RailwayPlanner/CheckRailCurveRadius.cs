using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RailwayPlanner
{
    public class CheckRailCurveRadius : GH_Component
    {
        

        /// <summary>
        /// Initializes a new instance of the CheckRailCurveRadius class.
        /// </summary>
        public CheckRailCurveRadius()
          : base("CheckRailCurveRadius", "RadiusCheck",
              "Checks Rail Curve for minimal radius",
              "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve To Check", GH_ParamAccess.item);
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
            if (!DA.GetData(0, ref crv)) return;

            List<Point3d> locations = new List<Point3d>();
            List<double> radii = new List<double>();


            if(crv.IsPolyline())
            {
                Polyline pLine = null; 
                if(crv.TryGetPolyline(out pLine))
                {
                    for (int i = 0; i < pLine.Count-2; i++)
                    {
                        Point3d p1 = pLine[i];
                        Point3d p2 = pLine[i + 1];
                        Point3d p3 = pLine[i + 2];
                        Circle circle = new Circle(p1, p2, p3);
                        double radius = circle.Radius;
                        if(radius>maxRadius)
                        {
                            radii.Add(radius);
                            locations.Add(p2);
                        }
                    }
                }
            }

            else
            {
                double[] parameters = crv.DivideByCount(divisions, true);
                foreach (double param in parameters)
                {
                    Vector3d curvatureVector =  crv.CurvatureAt(param);
                    double vectorLength = curvatureVector.Length;
                    try
                    {
                        double radius = 1 / vectorLength;
                        if (radius > maxRadius)
                        {
                            radii.Add(radius);
                            locations.Add(crv.PointAt(param));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }          
            }

            _positions = locations;
            _radii = radii;
        }

        private double maxRadius = 5000;
        private int divisions = 100;
        private List<Point3d> _positions = new List<Point3d>();
        private List<double> _radii = new List<double>();

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Color color1 = Color.SteelBlue;
            Color color2 = Color.LightSkyBlue;
            Color colorGrey = Color.DimGray;
            Color colorFrameColumn = Color.SteelBlue;
            Color colorRed = Color.Firebrick;



            // Display Building
            Point3d GC = _gravCenterBuilding;
            args.Display.Draw2dText(_textBuilding1, colorGrey, Point3d.Add(GC, new Point3d(2, 0, 0)), false, 20 * _textFac);
            args.Display.Draw2dText(_textBuilding2, colorGrey, Point3d.Add(GC, new Point3d(2, -2, 0)), false, 14 * _textFac);
            args.Display.DrawPoint(GC, colorGrey);
            args.Display.DrawCircle(new Circle(GC, 2 * _sFac), colorGrey);
            args.Display.DrawBrepWires(_brepBuilding, colorGrey);
            // Display Cores
            for (int i = 0; i < _gravCentCores.Count; i++)
            {
                GC = _gravCentCores[i];
                //args.Display.Draw2dText(_textCores1[i], colorCore, Point3d.Add(GC, new Point3d(2, 0, 0)), false, 14 * _textFac);
                args.Display.Draw2dText(_textCores2[i], color1, Point3d.Add(GC, new Point3d(2, -2, 0)), false, 14 * _textFac);
                args.Display.DrawPoint(GC, color1);
                args.Display.DrawCircle(new Circle(GC, _sFac), color1);
                args.Display.DrawBrepShaded(_brepCores[i], materialCore);
                args.Display.DrawBrepWires(_brepCores[i], color1, -1);
            }
            for (int i = 0; i < _evalPts.Count; i++)
            {
                Line line = new Line(_evalPts[i], _evalPts[i] + _sFac * _Sz[i] * Vector3d.ZAxis);
                //args.Display.DrawLine(line, colorStaticMoment);
            }

            // Display Bracing
            for (int i = 0; i < _bracingPoints.Count; i++)
            {
                for (int j = 0; j < _bracingPoints[i].Count; j++)
                {
                    // Draw Columns
                    args.Display.DrawLine(_bracingPoints[i][j][0], _bracingPoints[i][j][1], colorFrameColumn, 10);
                    args.Display.DrawLine(_bracingPoints[i][j][2], _bracingPoints[i][j][3], colorFrameColumn, 10);
                    // Draw Bracings
                    args.Display.DrawLine(_bracingPoints[i][j][0], _bracingPoints[i][j][3], colorRed, 10);
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
            get { return new Guid("4c77e8a0-7eeb-4b90-8a58-bbace5c0c1be"); }
        }
    }
}