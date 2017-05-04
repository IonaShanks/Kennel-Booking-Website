using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kennels.Models
{
    public class DateFormat : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            CultureInfo culture = new CultureInfo("en-GB"); // dd/MM/yyyy
            if (value != null)
            {
                var date = value.ConvertTo(typeof(DateTime?), culture);
                return date;
            }

            return null;
        }
    }
}