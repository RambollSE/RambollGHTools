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
                        Circle circle = new Circle(new Point3d(p1.X, p1.Y, 0), new Point3d(p2.X, p2.Y, 0), new Point3d(p3.X, p3.Y, 0));
                        double radius = circle.Radius;
                        if(radius<maxRadius)
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
                        if (radius < maxRadius)
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
            DA.SetData(0, crv);
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

            // Display Radius
            for (int i = 0; i < _positions.Count; i++)
            {
                string dispText = "Radius to small r = " + ((int)_radii[i]).ToString();
                Rhino.RhinoApp.WriteLine(args.Display.Viewport.Name);
                if (args.Display.Viewport.Name == "Top")
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
            get { return new Guid("4c77e8a0-7eeb-4b90-8a58-bbace5c0c1be"); }
        }
    }
}