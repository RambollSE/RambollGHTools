using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RailwayPlanner
{
    public class HeightIdentifier : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HeightIdentifier class.
        /// </summary>
        public HeightIdentifier()
            : base("HeightIdentifier", "HI",
        ".",
        "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("zValue", "zVal", "double", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Low", "L", "int", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mid", "M", "int", GH_ParamAccess.list);
            pManager.AddNumberParameter("High", "H", "int", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> zVal = new List<double>();
            if (!DA.GetDataList(0, zVal)) return;
            List<int> low = new List<int> ();
            List<int> mid = new List<int>();
            List<int> high = new List<int>();
            List<int> tempLow = new List<int>();

            for (int i = 0; i < zVal.Count; i++)
            {

                zVal[i] = zVal[i] * -1;
                if(zVal[i] >= 0)
                {
                    if (zVal[i] >= 5 && zVal[i] <= 15)
                    {
                        mid.Add(i);
                    }
                    if (zVal[i] > 15)
                    {
                        high.Add(i);
                    }
                    if (zVal[i] < 5)
                        {
                        low.Add(i);
                    }
                }
            }
            DA.SetDataList(0, low);
            DA.SetDataList(1, mid);
            DA.SetDataList(2, high);

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
            get { return new Guid("e46da29a-bb92-4549-b7bf-6e90cdf08780"); }
        }
    }
}