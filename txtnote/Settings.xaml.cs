using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace txtnote
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //防止跳转页面
            App.ViewModel.forShare = null;
            base.OnNavigatedFrom(e);
        }

        private void Bcakground_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBoxSetting.IsChecked)
            {
                takePhote();
                NavigationService.GoBack();
            }
            else
            {
                System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                if (isf.FileExists("back_temp.jpg")) isf.DeleteFile("back_temp.jpg");
            }
        }


        private void takePhote()
        {
            PhotoChooserTask PhotoTask = new PhotoChooserTask();
            PhotoTask.PixelWidth = 1024;
            PhotoTask.PixelHeight = 768;
            PhotoTask.ShowCamera = true;
            PhotoTask.Completed += new EventHandler<PhotoResult>(PhotoTask_Completed);
            PhotoTask.Show();
        }


        void PhotoTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {



                System.Windows.Media.Imaging.WriteableBitmap bmp = Microsoft.Phone.PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                if (isf.FileExists("back_temp.jpg")) isf.DeleteFile("back_temp.jpg");
                System.IO.IsolatedStorage.IsolatedStorageFileStream PhotoStream = isf.CreateFile("back_temp.jpg");
                System.Windows.Media.Imaging.Extensions.SaveJpeg(bmp, PhotoStream, 1024, 768, 0, 100); //这里设置保存后图片的大小、品质
                PhotoStream.Close();    //写入完毕，关闭文件流
                

                

                
                
                
            }

            
        }

        private void Call_Writer(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();         
            emailComposeTask.To = "goumang2010@live.com";
            emailComposeTask.Subject = "";
            emailComposeTask.Body = "";
            emailComposeTask.Show();
        }

        private void Evaluate(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }
    }
}