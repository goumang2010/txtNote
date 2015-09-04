using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;

namespace txtnote
{
    public partial class ViewEdit : PhoneApplicationPage
    {
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private string fileName = "";
        public ViewEdit()
        {
            InitializeComponent();
        }

        private void AppBar_Back_Click(object sender, EventArgs e)
        {
            navigationback();
        }

        private void AppBar_Edit_Click(object sender, EventArgs e)
        {
            if (displayTextBlock.Visibility==System.Windows.Visibility.Visible)
            {
                bindEdit(displayTextBlock.Text);



        }
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (editTextBox.Visibility == System.Windows.Visibility.Visible)
            {

                //save
                var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
                using (var file = appStorage.OpenFile(fileName, System.IO.FileMode.Create))
                {
                    using (StreamWriter sr = new StreamWriter(file))
                    {
                        sr.WriteLine(editTextBox.Text);

                    }
                }
                displayTextBlock.Text = editTextBox.Text;
                editTextBox.Visibility = System.Windows.Visibility.Collapsed;
                displayTextBlock.Visibility = System.Windows.Visibility.Visible;

            }
        }

        private void AppBar_Delete_Click(object sender, EventArgs e)
        {
            confirmDialog.Visibility = System.Windows.Visibility.Visible;

        }
        private void navigationback()
        {
            settings["state"] = "";
            settings["value"] = "";
            settings["fileName"] = "";
            NavigationService.Navigate(new Uri("/txtnote;component/MainPage.xaml", UriKind.Relative));

        }

        private void PhoneApplicationPage_Loaded_2(object sender, RoutedEventArgs e)
        {

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
               
                    

                if (state == "edit")
                {
                    string value = "";
                    if (settings.Contains("fileName"))
                    {
                        if (settings.TryGetValue<string>("fileName", out value))
                        {
                            fileName = value;

                        }
                    }

                    if (settings.Contains("value"))
                    {

                        if (settings.TryGetValue<string>("value", out value))
                        {
                            bindEdit(value);
                           
                        }
                    }


                }
                else
                {
                    bindView();
                }
            }
            }
            else
            {
                bindView();
            }
             string freeName = fileName.Substring(20);
             freeName = freeName.Substring(0, freeName.Length - 4);
             top.Text = freeName;

        }
        private void bindView()
        {
            fileName = NavigationContext.QueryString["id"];
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = appStorage.OpenFile(fileName, System.IO.FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    displayTextBlock.Text = sr.ReadToEnd();

                }
            }

        }

        private void bindEdit(string content)
        {
            editTextBox.Text =content;
            displayTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            editTextBox.Visibility = System.Windows.Visibility.Visible;
            editTextBox.Focus();
            editTextBox.SelectionStart = editTextBox.Text.Length;
            settings["state"] = "edit";
            settings["value"] = editTextBox.Text;
            settings["fileName"] = fileName;

        }
          
       
        private void cancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            confirmDialog.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void deleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            appStorage.DeleteFile(fileName);
            navigationback();
        }

        private void editTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            settings["value"] = editTextBox.Text;
        }
    }
}