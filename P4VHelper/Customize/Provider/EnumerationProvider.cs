// jdyun 24/04/29(월) - 오프
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace P4VHelper.Customize.Provider
{
    //@출처: https://stackoverflow.com/questions/4306743/wpf-data-binding-how-to-data-bind-an-enum-to-combo-box-using-xaml
    public class EnumerationProvider
    {
        public static Array GetValues(Type _enumeration)
        {
            Array wArray = Enum.GetValues(_enumeration);
            ArrayList wFinalArray = new ArrayList();
            foreach (Enum wValue in wArray)
            {
                FieldInfo fi = _enumeration.GetField(wValue.ToString());
                if (null != fi)
                {
                    BrowsableAttribute[] wBrowsableAttributes = fi.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                    if (wBrowsableAttributes.Length > 0)
                    {
                        //  If the Browsable attribute is false
                        if (wBrowsableAttributes[0].Browsable == false)
                        {
                            // Do not add the _enumeration to the list.
                            continue;
                        }
                    }

                    DescriptionAttribute[] wDescriptions = fi.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                    if (wDescriptions.Length > 0)
                    {
                        wFinalArray.Add(wDescriptions[0].Description);
                    }
                    else
                        wFinalArray.Add(wValue);
                }
            }

            return wFinalArray.ToArray();
        }
    }
}
