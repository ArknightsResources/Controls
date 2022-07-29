using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ArknightsResources.Controls.Uwp
{
    internal class Converters
    {
        public class BoolNullableToVisibility : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (value is bool?)
                {
                    bool? valueLocal = (bool?)value;
                    if (valueLocal == true)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                switch (value)
                {
                    case Visibility visibility:
                        switch (visibility)
                        {
                            case Visibility.Visible:
                                return true;
                            case Visibility.Collapsed:
                            default:
                                return false;
                        }
                    default:
                        return DependencyProperty.UnsetValue;
                }
            }
        }
    }
}
