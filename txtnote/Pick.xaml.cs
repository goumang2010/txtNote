using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace txtnote
{
    public partial class Pick : PhoneApplicationPage
    {
        public Pick()
        {
            InitializeComponent();
            
            longListSelector.ItemsSource = new Items();



        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel link = (StackPanel)sender;
            //选定该项目
            App.ViewModel.CleanCheckBox();
            App.ViewModel.forShare = App.ViewModel.displayItem(link.Tag.ToString());
           // App.ViewModel.SaveChangesToDB();
            NavigationService.GoBack();

        }

  //      private void cleanShare(object sender, System.ComponentModel.CancelEventArgs e)
 //       {
  //          App.ViewModel.forShare = null;
  //      }



       /* private void pick_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBox.Show("nima");
        }
        * */
    }
}