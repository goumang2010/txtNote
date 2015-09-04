using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace txtnote
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static txtnote.Resources.StringLibrary res = new txtnote.Resources.StringLibrary();
        public txtnote.Resources.StringLibrary Resource1 { get { return res; } }
    }
}
