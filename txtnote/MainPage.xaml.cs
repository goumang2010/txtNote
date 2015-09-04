using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
// Directive for the ViewModel.
using LocalDatabaseSample.Model;
using System.Windows.Navigation;





namespace txtnote
{
    
    public partial class MainPage : PhoneApplicationPage
    {
    // private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private string fileName = "";
       
        //private ToDoItem App.ViewModel.forShare = null;
        bool scrollWhenDescChanged = false;
       //public event EventHandler<ActivatedEventArgs> Activated;
        // 构造函数
        public MainPage()
        {
            BackKeyPress += MainPage_BackKeyPress;
            //Activated += MainPage_Activated;
            InitializeComponent();
            this.DataContext = App.ViewModel;
           appBar1();

          // ListPickerItem richang = new ListPickerItem { Content = App.ViewModel.CategoriesList.ElementAt(1).Name };
        }
        


       
        
        #region systemEvent
        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {






            int count = App.ViewModel.displayItemCount("example.txt");
            if (count == 0)
            {

                ToDoItem newToDoItem = new ToDoItem
                {
                    //不同difference
                    ItemName = "example.txt",
                    Category = (ToDoCategory)IEcategoriesListPicker.SelectedItem,
                    CreatTime = DateTime.Now,
                    EditTime = DateTime.Now,
                    TxtFile = txtnote.Resources.StringLibrary.shili

                };
                App.ViewModel.AddToDoItem(newToDoItem);
            }


   
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            switch (MainPano.SelectedIndex)
            {
               
                case 2:
                    imageView(1);
                    break;
                case 0:
                    //由设置界面返回时，刷新列表
                    App.ViewModel.Search(serchBox.Text, (bool)allTextSearch.IsChecked);
                    //有设置界面返回时，刷新背景图片
                    showBackground();
                    nvigationEdit();
                    break;
                default:
                    //启动时也要刷新背景图片，此时索引值为-1
                    showBackground();
                    break;

            }


         base.OnNavigatedTo(e);
        }
        void MainPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainPano.SelectedItem == AddPano)
            {
                if (AddHeader.Text != txtnote.Resources.StringLibrary.addtxt)
                {
                    if (App.ViewModel.forShare.TxtFile != editTextBox.Text || App.ViewModel.forShare.ItemName != fileNameInput.Text)
                    {
                        okBack();

                    }
                    else
                    {
                        initialAddPano();
                        MainPano.DefaultItem = ViewPano;
                        CleanCheckBox();
                    }

                }
                else
                {
                    okBack();
                    

                }
                e.Cancel = true;


            }
            else
            {
                if (MessageBox.Show(txtnote.Resources.StringLibrary.tuichu, "Exit", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;


                }

            }

        }
        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            switch (((Panorama)sender).SelectedIndex)
            {
                case 0:
                    appBar1();
                    break;
                case 1:
                    appBar2();
                   // ApplicationBar.IsVisible = false;
                    break;
                case 2:
                    tip.Focus();
                    ApplicationBar.IsVisible = false;
                    imageView(0);

                   
                    break;
            }

        }
        
        
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Save changes to the database.
            App.ViewModel.SaveChangesToDB();
        }


        #endregion

        #region Search


        private void serchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (serchBox.Text == txtnote.Resources.StringLibrary.searchBox)
            {
                serchBox.Text = "";
            }
        }
        private void serchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (serchBox.Text == "")
            {
                serchBox.Text = txtnote.Resources.StringLibrary.searchBox;
            }
        }
        private void serchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            serchHelp();
        }


        private void allTextSearch_Click(object sender, RoutedEventArgs e)
        {
            serchHelp();
        }

        private void serchHelp()
        {

        App.ViewModel.Search(serchBox.Text,(bool)allTextSearch.IsChecked);


        }
        private void Allselect_Click(object sender, RoutedEventArgs e)
        {
            if (Allselect.IsChecked == true)
            {
                var updateIe = App.ViewModel.AllToDoItems.AsQueryable();
                foreach (ToDoItem updateItem in updateIe)
                {
                    updateItem.IsComplete = true;
                }
            }
            else
            {
                var updateIe = App.ViewModel.AllToDoItems.AsQueryable();
                foreach (ToDoItem updateItem in updateIe)
                {
                    updateItem.IsComplete = false;
                }

            }
        }
        private void categoriesListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesListPicker != null)
            {
                ListPickerItem selectedItem = (ListPickerItem)categoriesListPicker.SelectedItem;
                string content = selectedItem.Tag.ToString();

                    App.ViewModel.changeAllItem(content);
                
            }
        }







        #endregion

        #region ListBox



        private void noteLocation_click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton clickedLink = (HyperlinkButton)sender;
            App.ViewModel.forShare = clickedLink.DataContext as ToDoItem;
            nvigationEdit();
            //   string uri = String.Format("/txtnote;component/ViewEdit.xaml?id={0}&state=0", clickedLink.Tag);
            //   NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
        #endregion

        #region ContextMenu


        private void nvigationEdit()
        {
            if (App.ViewModel.forShare != null)
            {
                MainPano.DefaultItem = AddPano;
                fileName = App.ViewModel.forShare.ItemName;
                fileNameInput.Text = App.ViewModel.forShare.ItemName;
                IEcategoriesListPicker.SelectedIndex = App.ViewModel.forShare._categoryId - 1;
                editTextBox.Text = App.ViewModel.forShare.TxtFile;
                AddHeader.Text = txtnote.Resources.StringLibrary.edittxt;
            }
            // NavigationService.Navigate(new Uri("/txtnote;component/MainPage.xaml", UriKind.Relative));
        }



        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedLink = (MenuItem)sender;
            if (clickedLink != null)
            {
                ToDoItem toDoForDelete = clickedLink.DataContext as ToDoItem;
                fileName = toDoForDelete.ItemName;
           //删除图片
            System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (isf.FileExists(toDoForDelete.ItemName + ".jpg")) isf.DeleteFile(toDoForDelete.ItemName + ".jpg");
                //删除磁贴
            deleteTile();
              //  删除数据
            App.ViewModel.DeleteToDoItem(toDoForDelete);
            }
            this.Focus();
        }

        private void deleteTile()
        {
            string uri = String.Format("/txtnote;component/MainPage.xaml?ID={0}", fileName);
            // Try to find a Tile that has this page's URI.
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(uri));
            if (tile != null)
            {
                tile.Delete();
            }
            
        }


        private void Skydrive_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedLink = (MenuItem)sender;
            ToDoItem toDoForUpload = clickedLink.DataContext as ToDoItem;
            CleanCheckBox();
            toDoForUpload.IsComplete = true;
            string uri = String.Format("/txtnote;component/share.xaml");
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        private void PinToStart_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedLink = (MenuItem)sender;
            ToDoItem toDoForTile = clickedLink.DataContext as ToDoItem;
            fileName = toDoForTile.ItemName;
            CleanCheckBox();
            toDoForTile.IsComplete = true;
            string uri = String.Format("/txtnote;component/MainPage.xaml?ID={0}", fileName);
            // Try to find a Tile that has this page's URI.
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(uri));
           
            if (tile == null)
            {

                // No Tile was found, so add one for this page.
                StandardTileData tileData = new StandardTileData { Title = fileName, BackContent = toDoForTile.TxtFile, BackgroundImage = new Uri("/Background.png", UriKind.Relative) };
                ShellTile.Create(new Uri(uri, UriKind.Relative), tileData);
                clickedLink.IsEnabled = false;

            }

        }

        #endregion

        #region AppBar

        void appBar1()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/appbar.add.rest.png", UriKind.Relative);
            button1.Text = txtnote.Resources.StringLibrary.addtxt;
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/share.png", UriKind.Relative);
            button2.Text = txtnote.Resources.StringLibrary.fenxiang;
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/transport.play.png", UriKind.Relative);
            button3.Text = txtnote.Resources.StringLibrary.gailan;
            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/appbar.delete.rest.png", UriKind.Relative);
            button4.Text = txtnote.Resources.StringLibrary.shanchu;
            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            menuItem1.Text = txtnote.Resources.StringLibrary.shezhi;
            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            menuItem2.Text = txtnote.Resources.StringLibrary.pingjia;
            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();
            menuItem3.Text = txtnote.Resources.StringLibrary.bangzhu;

            ApplicationBar.MenuItems.Add(menuItem1);
            ApplicationBar.MenuItems.Add(menuItem2);
            ApplicationBar.MenuItems.Add(menuItem3);
            ApplicationBar.Buttons.Add(button1);
            ApplicationBar.Buttons.Add(button2);
            ApplicationBar.Buttons.Add(button3);
            ApplicationBar.Buttons.Add(button4);
            button1.Click += new EventHandler(AppBar_Add_Click);
            button2.Click += new EventHandler(AppBar_Cloud_Click);
            button3.Click += new EventHandler(View_Click);
            button4.Click += new EventHandler(selectionDelete_Click);
            menuItem1.Click += new EventHandler(Setting_Click);
            menuItem2.Click += new EventHandler(Evaluate_Click);
            menuItem3.Click += new EventHandler(Help_Click);
        }

        private void Help_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/txtnote;component/Help.xaml", UriKind.Relative));
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/txtnote;component/Settings.xaml", UriKind.Relative));
        }

        private void Evaluate_Click(object sender, EventArgs e)
        {
               MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

               marketplaceReviewTask.Show();

        }
        void appBar2()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/save.png", UriKind.Relative);
            button1.Text = txtnote.Resources.StringLibrary.baocun;
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/feature.camera.png", UriKind.Relative);
            button2.Text = txtnote.Resources.StringLibrary.addImage;

            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/appbar.cancel.rest.png", UriKind.Relative);
            button3.Text = txtnote.Resources.StringLibrary.qingchu;


            ApplicationBar.Buttons.Add(button1);
            ApplicationBar.Buttons.Add(button2);
            ApplicationBar.Buttons.Add(button3);

            button1.Click += new EventHandler(Save_Click);
            button2.Click += new EventHandler(Image_Click);
            button3.Click += new EventHandler(Clear_Click);

        }

        private void Clear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(txtnote.Resources.StringLibrary.qingchu + "?", txtnote.Resources.StringLibrary.qingchu, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
            initialAddPano();
        }
        }


        void appBar3()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/save.png", UriKind.Relative);
            button1.Text = txtnote.Resources.StringLibrary.baocun;

            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/appbar.cancel.rest.png", UriKind.Relative);
            button3.Text =  txtnote.Resources.StringLibrary.qingchu;

            ApplicationBar.Buttons.Add(button1);
            ApplicationBar.Buttons.Add(button3);

            button1.Click += new EventHandler(Picture_Save);
            button3.Click += new EventHandler(Image_Clear);

        }

        private void Image_Clear(object sender, EventArgs e)
        {
            App.ViewModel.forShare = null;
            imagePicker.Content = txtnote.Resources.StringLibrary.imagename;
            imagePicker.Opacity = 0.5;
            imageTip.Visibility = System.Windows.Visibility.Visible;
            tip.Background = null;
            ApplicationBar.IsVisible = false;
        }
        private void takePhote()
        {
            PhotoChooserTask PhotoTask = new PhotoChooserTask();
            PhotoTask.PixelWidth = 432;
            PhotoTask.PixelHeight = 645;
            PhotoTask.ShowCamera = true;
            PhotoTask.Completed += new EventHandler<PhotoResult>(PhotoTask_Completed);
            PhotoTask.Show();
        }

        private void Save_Click(object sender, EventArgs e)
        {

            saveOrNot();

        }


        private void AppBar_Add_Click(object sender, EventArgs e)
        {
            initialAddPano();
            MainPano.DefaultItem = AddPano;

        }
        private void initialAddPano()
        {
            App.ViewModel.forShare = null;
            AddHeader.Text = txtnote.Resources.StringLibrary.addtxt;
           
            
            fileNameInput.Text = txtnote.Resources.StringLibrary.filename;
            ViewPano.Focus();
            editTextBox.Text = "";
        }

        private void AppBar_Cloud_Click(object sender, EventArgs e)
        {
           // App.ViewModel.UpdateItem();
            string uri = String.Format("/txtnote;component/share.xaml");
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

      /**  private void Search_Click(object sender, EventArgs e)
        {
            allTextSearch.IsChecked = true;
            serchBox.Focus();
        }
       * */

        private void selectionDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(txtnote.Resources.StringLibrary.shanchu+"？", txtnote.Resources.StringLibrary.shanchu, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.ViewModel.SaveChangesToDB();
                App.ViewModel.UpdateItem();
                var toDelete = App.ViewModel.AllToUpdateItems.AsQueryable();
                foreach (ToDoItem ord in toDelete)
                {
                    App.ViewModel.DeleteToDoItem(ord);
                    fileName = ord.ItemName;
                    deleteTile();


                }
            }
        }


        #endregion

        #region Add


        private void Image_Click(object sender, EventArgs e)
        {
            string abc = fileNameInput.Text;
            if (App.ViewModel.displayItem(fileNameInput.Text) == null)
            {
                if (fileNameInput.Text == txtnote.Resources.StringLibrary.filename || fileNameInput.Text == "")
                {
                    MessageBox.Show(txtnote.Resources.StringLibrary.filename);

                    // MainPano.DefaultItem = AddPano;
                    fileNameInput.Focus();
                }
                else
                {
                    Save();
                    App.ViewModel.forShare = App.ViewModel.displayItem(abc);
                    MainPano.DefaultItem = tip;
                }
            }
            else
            {
                App.ViewModel.forShare = App.ViewModel.displayItem(abc);
                MainPano.DefaultItem = tip;
            }
            
        }
        private void fileNameInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (fileNameInput.Text == txtnote.Resources.StringLibrary.filename)
            {
                fileNameInput.Text = "";
            }
        }
        private void fileNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fileNameInput.Text == "")
            {
                fileNameInput.Text = txtnote.Resources.StringLibrary.filename;
            }

        }
        private void fileNameInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                editTextBox.Focus();
          textSroll.ScrollToVerticalOffset(editTextBox.ActualHeight);
            }
        }

        private void editTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (scrollWhenDescChanged && editTextBox.SelectionStart == editTextBox.Text.Length)
            {
           textSroll.ScrollToVerticalOffset(editTextBox.ActualHeight);
            }
            scrollWhenDescChanged = true;
        }

    

        #endregion

        #region helpMethod

        private void showBackground()
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            //点击超链接后图片存在

            if (isf.FileExists("back_temp.jpg"))
            {

                System.IO.IsolatedStorage.IsolatedStorageFileStream PhotoStream = isf.OpenFile("back_temp.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.Windows.Media.Imaging.BitmapImage bmp1 = new System.Windows.Media.Imaging.BitmapImage();
                bmp1.SetSource(PhotoStream);  //把文件流转换为图片
                PhotoStream.Close();    //读取完毕，关闭文件流

                System.Windows.Media.ImageBrush ib = new System.Windows.Media.ImageBrush();
                ib.ImageSource = bmp1;
                MainPano.Background = ib;  //把图片设置为控件的背景图     
                //imageTip.Visibility = System.Windows.Visibility.Collapsed;
            }
           // else
          //  {
           //     MainPano.Background = null;
          //  }
        }
        private void showImage()
        {
               if (App.ViewModel.forShare != null)
                {
                    imagePicker.Content = App.ViewModel.forShare.ItemName;
                    imagePicker.Opacity = 0.5;
                    System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                    //点击超链接后图片存在
                    if (isf.FileExists(App.ViewModel.forShare.ItemName + ".jpg"))
                    {
                        isf.CopyFile(App.ViewModel.forShare.ItemName + ".jpg", "abc.jpg", true);
                        System.IO.IsolatedStorage.IsolatedStorageFileStream PhotoStream = isf.OpenFile("abc.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        System.Windows.Media.Imaging.BitmapImage bmp1 = new System.Windows.Media.Imaging.BitmapImage();
                        bmp1.SetSource(PhotoStream);  //把文件流转换为图片
                        PhotoStream.Close();    //读取完毕，关闭文件流

                        System.Windows.Media.ImageBrush ib = new System.Windows.Media.ImageBrush();
                        ib.ImageSource = bmp1;
                        tip.Background = ib;  //把图片设置为控件的背景图     
                        //imageTip.Visibility = System.Windows.Visibility.Collapsed;
                    }
               }
            //如果存在背景，则显示关联按钮
                   if (tip.Background != null)
                   {

                   
                       appBar3();
                   }
               
        }
        private void editImage()
        {
            if (App.ViewModel.forShare != null)
            {
                imagePicker.Content = App.ViewModel.forShare.ItemName;
            }
                       
            
            if (tip.Background != null && imagePicker.Content.ToString()!=txtnote.Resources.StringLibrary.imagename)
                        {

                           // imageTip.Visibility = System.Windows.Visibility.Visible;
                            appBar3();
                        }

        }
        private void CleanCheckBox()
        {
            App.ViewModel.CleanCheckBox();
            if (Allselect.IsChecked == true)
            {
                Allselect.IsChecked = false;
            }
        }
        private void imageView(int x)
        {
            switch (x)
            {
                //x=0是通过点击超链接，附加图片选项或按钮展示图片情况
                case 0:
                    showImage();
                    break;
                //选择文件名后返回，或是选择图片后返回
                case 1:
                    if (imageTip.Visibility == System.Windows.Visibility.Visible)
                    {
                        showImage();
                    }
                    editImage();
                    break;
                default:
                    break;


            }
        }


        private void saveOrNot()
        {
            if (fileNameInput.Text == txtnote.Resources.StringLibrary.filename || fileNameInput.Text == "")
            {
                MessageBox.Show(txtnote.Resources.StringLibrary.filename);
                
               // MainPano.DefaultItem = AddPano;
                fileNameInput.Focus();
            }
            else
            {
                Save();
            }
        }
        private void Save()
        {
            
            String itemname = fileNameInput.Text;
            int count = App.ViewModel.displayItemCount(fileNameInput.Text);
           // ToDoItem toSave = App.ViewModel.displayItem(fileNameInput.Text);
            if (AddHeader.Text == txtnote.Resources.StringLibrary.addtxt)
            {
                // Picker.Visibility = System.Windows.Visibility.Collapsed;

                if (count > 0)
                {
                    itemname = fileNameInput.Text + "_" + count.ToString() ;
                }
                //不同difference
                // Create a new to-do item.
                ToDoItem newToDoItem = new ToDoItem
                {
                    ItemName = itemname+".txt",
                    Category = (ToDoCategory)IEcategoriesListPicker.SelectedItem,
                    CreatTime = DateTime.Now,
                    EditTime = DateTime.Now,
                    TxtFile = editTextBox.Text.ToString()

                };
                App.ViewModel.AddToDoItem(newToDoItem);
                initialAddPano();//重置添加页面
                MainPano.DefaultItem = ViewPano;
                
               CleanCheckBox();
                //navigationback();
            }
            else
            {
                //如果该txt还存在，则保存编辑
                if (count != 0)
                {
                    if (App.ViewModel.forShare == null)
                    {
                        App.ViewModel.forShare = App.ViewModel.displayItem(fileNameInput.Text);
                    }


                    App.ViewModel.forShare.EditTime = DateTime.Now;
                    App.ViewModel.forShare.ItemName = fileNameInput.Text;
                    App.ViewModel.forShare.Category = (ToDoCategory)IEcategoriesListPicker.SelectedItem;
                    App.ViewModel.forShare.TxtFile = editTextBox.Text;
                    App.ViewModel.SaveChangesToDB();
                    initialAddPano();
                    MainPano.DefaultItem = ViewPano;
                    CleanCheckBox();
                    // navigationback();
                }
                else
                {
                    AddHeader.Text = txtnote.Resources.StringLibrary.addtxt;
                    Save();
                }
            }

        }

        private void okBack()
        {
            if (MessageBox.Show(txtnote.Resources.StringLibrary.okBack, txtnote.Resources.StringLibrary.shanchu, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                initialAddPano();
                MainPano.DefaultItem = ViewPano;
                CleanCheckBox();
            }

        }

        #endregion


        #region imageTip

        private void View_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/txtnote;component/Pick.xaml", UriKind.Relative));

        }
        private void tip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            takePhote();
        }

        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/txtnote;component/Pick.xaml", UriKind.Relative));

        }
        private void Picture_Save(object sender, EventArgs e)
        {

            System.IO.IsolatedStorage.IsolatedStorageFile isf2 = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
          //  if (isf2.FileExists(imagePicker.Content + ".jpg")) isf2.DeleteFile(imagePicker.Content + ".jpg");
            if (isf2.FileExists("abc.jpg"))
            {
                isf2.CopyFile("abc.jpg", imagePicker.Content + ".jpg",true);
                //MessageBox.Show("关联成功");
                //takePhote();
                if (MessageBox.Show(txtnote.Resources.StringLibrary.chenggong+"!", txtnote.Resources.StringLibrary.guanlian, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MainPano.DefaultItem = ViewPano;
                    imageTip.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Fail!"); 
            }

        }
        void PhotoTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {



                System.Windows.Media.Imaging.WriteableBitmap bmp = Microsoft.Phone.PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                if (isf.FileExists("abc.jpg")) isf.DeleteFile("abc.jpg");
                System.IO.IsolatedStorage.IsolatedStorageFileStream PhotoStream = isf.CreateFile("abc.jpg");
                System.Windows.Media.Imaging.Extensions.SaveJpeg(bmp, PhotoStream, 432, 645, 0, 100); //这里设置保存后图片的大小、品质
                PhotoStream.Close();    //写入完毕，关闭文件流

                PhotoStream = isf.OpenFile("abc.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);   //读取刚刚保存的图片的文件流
                System.Windows.Media.Imaging.BitmapImage bmp1 = new System.Windows.Media.Imaging.BitmapImage();
                bmp1.SetSource(PhotoStream);  //把文件流转换为图片
                PhotoStream.Close();    //读取完毕，关闭文件流

                System.Windows.Media.ImageBrush ib = new System.Windows.Media.ImageBrush();
                ib.ImageSource = bmp1;
                tip.Background = ib;//把图片设置为控件的背景图
                imageTip.Visibility = System.Windows.Visibility.Collapsed;
                if (imagePicker.Content.ToString() == txtnote.Resources.StringLibrary.imagename)
                {
                    if (MessageBox.Show(txtnote.Resources.StringLibrary.imagename, txtnote.Resources.StringLibrary.guanlian, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {

                        NavigationService.Navigate(new Uri("/txtnote;component/Pick.xaml", UriKind.Relative));

                    }
                }
                else
                {
                    appBar3();
                }
            }
        }
        #endregion

        private void addImage_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedLink = (MenuItem)sender;       
            App.ViewModel.forShare = clickedLink.DataContext as ToDoItem;
            MainPano.DefaultItem = tip;
        }





           
      
        
        


    

    }
}
