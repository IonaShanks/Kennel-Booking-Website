using System;
using System.Globalization;
using System.Web.Mvc;

namespace Kennels.Models
{
    //Added so that chrome takes date format as a real date
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