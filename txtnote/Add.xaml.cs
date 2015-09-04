using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Text;
using System.IO;

namespace txtnote
{
    public partial class Add : PhoneApplicationPage
    {
        //private string location = "";
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public Add()
        {

            InitializeComponent();
            /**  GeoCoordinateWatcher mywatcher = new GeoCoordinateWatcher();
            var myposition = mywatcher.Position;

            double latitude = 47.674;
            double longitude = -122.12;

            if (!myposition.Location.IsUnknown)
            {
                latitude = myposition.Location.Latitude;
                longitude = myposition.Location.Longitude;
            }

          myTerraService.TerraServiceSoapClient client = new myTerraService.TerraServiceSoapClient();           
            client.ConvertLonLatPtToNearestPlaceAsync(new myTerraService.LonLatPt() { Lat = latitude, Lon = longitude });
            client.ConvertLonLatPtToNearestPlaceCompleted += new EventHandler<myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs>(client_ConvertLonLatPtToNearestPlaceCompleted);
            
        }


       void client_ConvertLonLatPtToNearestPlaceCompleted(object sender, myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs e)
        {

            location = e.Result;
       
        */
        }
        private void AppBar_Cancel_Click(object sender, EventArgs e)
        {
            navigationback();
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            nameDialog.Visibility = System.Windows.Visibility.Visible;
            fileNameInput.Focus();
            ApplicationBar.IsVisible = false;

        }
        private void navigationback()
        {
            NavigationService.Navigate(new Uri("/txtnote;component/MainPage.xaml", UriKind.Relative));
            settings["state"] = "";
            settings["value"] = "";

        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            
            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "add")
                    {
                        string value = "";
                        if (settings.Contains("value"))
                        {
                            if (settings.TryGetValue<string>("value", out value))
                            {
                                editTextBox.Text = value;
                            }
                        }

                    }


                }
                settings["state"] = "add";
                settings["value"] = editTextBox.Text;
                editTextBox.Focus();
                editTextBox.SelectionStart = editTextBox.Text.Length;
            }
        }

        private void editTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            settings["value"] = editTextBox.Text;
        }

        private void okButton_Click_1(object sender, RoutedEventArgs e)
        {
          /*  if (location.Trim().Length == 0)
            {
                location = "unknown";
            }
            */
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.Year);
            sb.Append("_");
            sb.Append(string.Format("{0:00}", DateTime.Now.Month));
            sb.Append("_");
            sb.Append(string.Format("{0:00}", DateTime.Now.Day));
            sb.Append("_");
            sb.Append(string.Format("{0:00}", DateTime.Now.Hour));
            sb.Append("_");
            sb.Append(string.Format("{0:00}", DateTime.Now.Minute));
            sb.Append("_");
            sb.Append(string.Format("{0:00}", DateTime.Now.Second));
            sb.Append("_");
            /**
            location = location.Replace(" ", "-");
            location = location.Replace(",", "_");
             **/
            sb.Append(fileNameInput.Text);
            sb.Append(".txt");
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fileStream = appStorage.OpenFile(sb.ToString(), System.IO.FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.WriteLine(editTextBox.Text + "\r\n" /*+ location*/ + DateTime.Now.ToLongTimeString());
                }
            }
            navigationback();

        }

        private void deleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            fileNameInput.Text = "";
            fileNameInput.Focus();
        }
    }
}