using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QDB.UserControls.Classes
{
    public class ChapterToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<SectionElement> sections = values[0] as IEnumerable<SectionElement>;
            string chapterHeader = values[1] as string;
            if (sections != null && chapterHeader != null)
            {
                var selectedCount = sections.Where(s => s.IsChecked).Count();
                if (selectedCount.Equals(sections.Count()))
                    return true;
                if (selectedCount > 0)
                    return null;
                
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
