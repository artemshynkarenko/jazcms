using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.WebProject
{
    public class HidePropertyAttribute : System.Attribute
    {

        public bool IsHidden               
        {
            get
            {
                return isHidden;
            }
            set
            {

                isHidden = value;
            }
        }

        public HidePropertyAttribute(bool hideProperty) 
        {
            isHidden = hideProperty;
        }

        private bool isHidden;
    }
}
