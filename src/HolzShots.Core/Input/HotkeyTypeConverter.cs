using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace HolzShots.Input
{
    public class HotkeyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(int) || sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is int i)
                return Hotkey.FromHashCode(i);
            if (value is string s)
                return Hotkey.Parse(s);

            Debugger.Break(); // Something is wrong here.
            return base.ConvertFrom(context, culture, value);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(int) || destinationType == typeof(string);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Debug.Assert(value is Hotkey);
            if (destinationType == typeof(int))
                return (value as Hotkey).GetHashCode();
            if (destinationType == typeof(string))
                return (value as Hotkey).ToString();

            Debugger.Break(); // Something is wrong here.
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
