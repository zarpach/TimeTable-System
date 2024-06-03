using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace iuca.Application.Enums
{
    public static class EnumExtentions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName;
            displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }
           /* else 
            {
                ResourceManager rm = new ResourceManager(enumValue.GetType());
                displayName = rm.GetString(displayName);
            }*/
            return displayName;
        }

        public static SelectList ToSelectList<TEnum>(this TEnum obj)
    where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return new SelectList(Enum.GetValues(typeof(TEnum))
            .OfType<Enum>()
            .Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = (Convert.ToInt32(x))
                .ToString()
            }), "Value", "Text");
        }

        public static SelectList ToSelectList<TEnum>(this TEnum obj, object selectedValue)
    where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return new SelectList(Enum.GetValues(typeof(TEnum))
            .OfType<Enum>()
            .Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = (Convert.ToInt32(x))
                .ToString()
            }), "Value", "Text", selectedValue);
        }
    }
}
