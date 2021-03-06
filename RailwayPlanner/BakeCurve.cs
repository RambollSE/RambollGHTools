﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace RailwayPlanner
{
    public class BakeCurve : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BakeCurve()
          : base("Adjust Curve", "CrvAdjust",
              "Bakes or replaces curve ",
              "Ramboll Tools", "Railway Planner")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "R", "True if reset", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Layername", "L", "Name of layer", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "Object Color", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Locked", "L", "Lock Layer", GH_ParamAccess.item, false);


            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddGenericParameter("BakedCurveGuid", "G", "Guid of Baked Curve", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        /// 
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve sectionCurve = null;
            bool reset = false;
            System.Drawing.Color color = System.Drawing.Color.Aquamarine;
            string colorString = ""; 
            bool locked = false; 
            string layerName = "Profile Curve";
            if (!DA.GetData(0, ref sectionCurve)) return;
            if (!DA.GetData(1, ref reset)) return;
            if (!DA.GetData(2, ref layerName)) return;
            if (!DA.GetData(3, ref color)) return;
            if (!DA.GetData(4, ref locked)) return;

            //color = System.Drawing.Color.

            if (reset)
            {
                hasRunned = false; 
            }

            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            
            ObjectAttributes attr = doc.CreateDefaultAttributes();
            int layerIndex = 0;
            if (doc.Layers.FindName(layerName) == null)
            {
                layerIndex = doc.Layers.Add(layerName, color);
            }
            else
            {
                layerIndex = doc.Layers.FindName(layerName).Index;
                //Layer layer = doc.Layers.FindIndex(layerIndex);
                /*
                RhinoObject[] rhobjs = doc.Objects.FindByLayer(layer);
                foreach (RhinoObject obj in rhobjs)
                {
                    doc.Objects.Delete(obj);
                }*/
            }
            attr.LayerIndex = layerIndex;
            Layer layer = doc.Layers.FindIndex(layerIndex);
            layer.IsLocked = false;
            if (hasRunned == false)
            {
                guid = doc.Objects.AddCurve(sectionCurve,attr);
                hasRunned = true;

            }
            ObjRef objRef = new ObjRef(guid);
            doc.Objects.Replace(objRef, sectionCurve);
            Curve returnedCurve = doc.Objects.FindGeometry(guid) as Curve;
            layer.IsLocked = locked;

            // Finally assign the spiral to the output parameter.
            DA.SetData(0, guid);
        }

        private bool hasRunned = false;
        private Guid guid = new Guid(); 

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("19876a33-7bcc-4930-95a2-685893271bdc"); }
        }
    }
}
