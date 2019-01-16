using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using RObjects;

namespace RailwayPlanner
{
    public class RailClassifier : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RailClassifier class.
        /// </summary>
        public RailClassifier()
          : base("RailClassifier", "RClassifier",
              "Assigns segments to the right rail type",
              "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Road", "R", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Segment Curve", "sCrv", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Road Types", "rTypes", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Road roadSegment = new Road();
            if (!DA.GetData(0, ref roadSegment)) return;

            List<string> rTypes = new List<string>();

            if (roadSegment.Height <= 5)
                rTypes.Add("Bank Low");

            if (roadSegment.Height >= 5 && roadSegment.Height <= 10)
                rTypes.Add("Bank High");

            if (roadSegment.Height > 2 && roadSegment.Height <= 10)
                rTypes.Add("Bridge Low");

            if (roadSegment.Height > 10)
                rTypes.Add("Bridge High");

            if (roadSegment.Height < 0 && roadSegment.Height >= -15)
                rTypes.Add("Cut");

            if (roadSegment.Height < -15)
                rTypes.Add("Tunnel");

            DA.SetData(0, roadSegment.Curve);
            DA.SetDataList(1, rTypes);
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
            get { return new Guid("3c67d9b3-f0da-4f54-bd5d-b56fa28865e7"); }
        }
    }
}