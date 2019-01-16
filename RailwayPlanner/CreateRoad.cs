using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using RObjects;

namespace RailwayPlanner
{
    public class CreateRoad : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateRoad class.
        /// </summary>
        public CreateRoad()
          : base("CreateRoad", "CR",
              "Contains high level data about road segment",
              "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Segment Crv", "sCrv", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Road", "R", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve segmentCrv = null;
            double height = 0.0;
            if (!DA.GetData(0, ref segmentCrv)) return;
            if (!DA.GetData(1, ref height)) return;

            Road r = new Road();
            r.Curve = segmentCrv;
            r.Height = height;

            DA.SetData(0, r);

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
            get { return new Guid("16804ee7-9e5a-482b-9ae1-c42ff450057f"); }
        }
    }
}