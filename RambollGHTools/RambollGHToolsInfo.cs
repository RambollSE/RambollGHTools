using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace RambollGHTools
{
    public class RambollGHToolsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "RambollGHTools";
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
                return new Guid("bd099a0e-e3bf-4f41-8642-b3d386d1e7fe");
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
