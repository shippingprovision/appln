using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business
{
    public static class BOFactory
    {
        public static TBO GetBO<TBO>() where TBO:new()
        { 
            return new TBO();
        }        
    }
}
