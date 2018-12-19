using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace RailwayPlanner
{
    public class RailwayPlannerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "RailwayPlanner";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("d50cfb82-a42d-47fc-a3e0-45f2f3f868a3");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Microsoft";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
