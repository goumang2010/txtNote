using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Live;
using Microsoft.Live.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using LocalDatabaseSample.Model;
using Microsoft.Phone.Tasks;
namespace txtnote
{
    public partial class share : PhoneApplicationPage
    {

        private LiveConnectClient client;
        private LiveConnectSession session;
        //不同difference
        private string skyDriveFolderName = "txtNote";
        private string skyDriveFolderID = string.Empty;
        private string skyDriveFolderID_image = string.Empty;
        private string skyDriveFolderID_merge = string.Empty;
        List<Note> Notes = new List<Note>();
        List<Note> Notes_merge = new List<Note>();
      //  Items Notes = new Items();
        string itemname;
        string txtfile;
        //下载队列中的数量
        int s_number;
        //下载队列中被选中的最大值
      // int pickMaxNumber ;
        int k;

        string displayInfo = txtnote.Resources.StringLibrary.chenggong+":";
        public share()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.UpdateItem();

           // UploadFile();
        }

        #region commonConnect
        private void btnLogin_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            //若连接上了，则显示已登陆
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                session = e.Session;
                client = new LiveConnectClient(session);
                infoTextBlock.Text = txtnote.Resources.StringLibrary.shareInfo_2;
                infoTextBlock2.Text =txtnote.Resources.StringLibrary.shareInfo_2;
                infoTextBlock3.Text = txtnote.Resources.StringLibrary.shareInfo_2;

                //读取skydrive根目录的文件和文件夹，由btnSignin_GetCompleted处理
                client.GetCompleted +=
                    new EventHandler<LiveOperationCompletedEventArgs>(btnSignin_GetCompleted);
                client.GetAsync("me/skydrive/files?filter=folders");

            }
            else
            {
                //未登录,请点击sign in
                infoTextBlock.Text = txtnote.Resources.StringLibrary.shareInfo_1;
                infoTextBlock2.Text = txtnote.Resources.StringLibrary.shareInfo_1;
                infoTextBlock3.Text = txtnote.Resources.StringLibrary.shareInfo_1;
                client = null;
            }

        }
        private void btnSignin_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                infoTextBlock.Text = "Loading folder...";

                //列出根目录下的数据
                Dictionary<string, object> folderData = (Dictionary<string, object>)e.Result;
                List<object> folders = (List<object>)folderData["data"];

                //go through each folder and see if the isolatedstoragefolder exists
                foreach (object item in folders)
                {
                    //如果找到的txtNote目录，则直接抓取其folderID
                    Dictionary<string, object> folder = (Dictionary<string, object>)item;
                    if (folder["name"].ToString() == skyDriveFolderName)
                        skyDriveFolderID = folder["id"].ToString();
                }
                //没找到txtNote，则建立，结果交由CreateFolder_Completed处理
                if (skyDriveFolderID == string.Empty)
                {
                    Dictionary<string, object> skyDriveFolderData = new Dictionary<string, object>();
                    skyDriveFolderData.Add("name", skyDriveFolderName);
                    
                    client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                    client.PostAsync("me/skydrive", skyDriveFolderData); //creating the IsolatedStorageFolder in Skydrive
                    
                    infoTextBlock.Text = "Creating folder...txtNote";
                }

                    //找到了txtNote目录，则获取该目录下的文件和文件夹，交由getFiles_GetCompleted处理
                else
                {
                    client = new LiveConnectClient(session);
                    client.GetCompleted -=
new EventHandler<LiveOperationCompletedEventArgs>(btnSignin_GetCompleted);
                    client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(getFiles_GetCompleted);
                   client.GetAsync(skyDriveFolderID + "/files"); //check through the files in the folder


                }
                }


            else
            {
                MessageBox.Show(e.Error.Message);
            }

        }

        

        private void CreateFolder3_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                infoTextBlock.Text = txtnote.Resources.StringLibrary.shareInfo_3;
                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                skyDriveFolderID_merge = folder["id"].ToString(); //grab the folder ID
                //merge文件夹建立后，自动上传merge的空文件
                byte[] b2 = System.Text.Encoding.UTF8.GetBytes("");
                MemoryStream file = new MemoryStream(b2);
                client.UploadAsync(skyDriveFolderID_merge, "txtNote_Merge.txt", file, OverwriteOption.Overwrite);
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        //创建image目录完成
        private void CreateFolder2_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //infoTextBlock.Text = txtnote.Resources.StringLibrary.shareInfo_3;
                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                skyDriveFolderID_image = folder["id"].ToString(); //grab the folder ID

            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
            //如果image文件夹不存在，建立之后，判断merge是否存在，不存在则建立之
            if (skyDriveFolderID_merge == string.Empty)
            {
                Dictionary<string, object> skyDriveFolderData = new Dictionary<string, object>();
                skyDriveFolderData.Add("name", "Merge");
               // client.PostCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                client.PostCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder2_Completed);
                client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder3_Completed);

                client.PostAsync(skyDriveFolderID, skyDriveFolderData);
                infoTextBlock.Text = "Creating folder...Merge";
            }
        }
        private void getFiles_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {

            if (e.Error == null)
            {
         infoTextBlock.Text = "Loading folder...";

                List<object> data = (List<object>)e.Result["data"];
                foreach (IDictionary<string, object> content in data)
                {
                    string downname = (string)content["name"];

                    //如果不是这两个文件夹，则加入待下载文件的列表中
                    if (downname != "Image" && downname != "Merge")
                        Notes.Add(new Note() { Name = downname, FileID = (string)content["id"] });
                   
                    //不同difference
                    else
                    {
                        //如果是这两个文件夹，尝试获得他们的folderID

                        if (downname == "Image")
                            skyDriveFolderID_image = content["id"].ToString();
                        if (downname == "Merge")
                            skyDriveFolderID_merge = content["id"].ToString();
                    }
                }
                noteListBox2.ItemsSource = Notes;
                //如果没有获得两个文件夹的folderID成功，则分别建立之
                if (skyDriveFolderID_image == string.Empty || skyDriveFolderID_merge == string.Empty)
                {

                    if (skyDriveFolderID_image == string.Empty)
                    {
                        Dictionary<string, object> skyDriveFolderData = new Dictionary<string, object>();
                        skyDriveFolderData.Add("name", "Image");
                        client.PostCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                        client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder2_Completed);
                        client.PostAsync(skyDriveFolderID, skyDriveFolderData);
                        infoTextBlock.Text = "Creating folder...Image";
                    }
                    //如果Image文件夹存在，则Merge文件夹必不存在，则必须建立Merge文件夹
                    else
                    {
                        Dictionary<string, object> skyDriveFolderData = new Dictionary<string, object>();
                        skyDriveFolderData.Add("name", "Merge"); 
                        client.PostCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                        client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder3_Completed);
                        client.PostAsync(skyDriveFolderID, skyDriveFolderData);
                        infoTextBlock.Text = "Creating folder...Merge";

                    }



                }
                    //如果这两个文件夹都存在，则抓取txtNote_merge中的内容
                else
                {


                    client.GetCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(getFiles_GetCompleted);
                    client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(getMergeFiles_GetCompleted);
                    client.GetAsync(skyDriveFolderID_merge + "/files"); //check through the files in the folder
                    infoTextBlock.Text = txtnote.Resources.StringLibrary.shareInfo_3;
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
            
        }

        private void getMergeFiles_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                List<object> data = (List<object>)e.Result["data"];
                foreach (IDictionary<string, object> content in data)
                {
                    string downname = (string)content["name"];
                    //不同difference
                    Notes_merge.Add(new Note() { Name = downname, FileID = (string)content["id"] });

                }
                noteListBox3.ItemsSource = Notes_merge;

            }



            else
            {
                MessageBox.Show(e.Error.Message);
            }
            
        }


        private void CreateFolder_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            //没找到txtNote目录，建立之后，调用该方法，在得到txtNote目录之后，继续列出txtNote目录下的文件和文件夹
            //交由getFiles_GetCompleted处理
            if (e.Error == null)
            {
                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                skyDriveFolderID = folder["id"].ToString(); //grab the folder ID
                client.GetCompleted -= new EventHandler<LiveOperationCompletedEventArgs>(btnSignin_GetCompleted);
                client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(getFiles_GetCompleted);
                client.GetAsync(skyDriveFolderID + "/files");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }

        }
        #endregion

        #region upload


        private void UploadFile()
        {




                var updateIe = App.ViewModel.AllToUpdateItems.AsQueryable();
                client.UploadCompleted += client_UploadCompleted;
                System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                foreach (ToDoItem uploadItem in updateIe)
                {
                    infoTextBlock.Text = "Uploading..." ;

                                     
                    
                        byte[] b2 = System.Text.Encoding.UTF8.GetBytes(uploadItem.TxtFile);
                        MemoryStream file = new MemoryStream(b2);
                        //不同difference
                        client.UploadAsync(skyDriveFolderID, uploadItem.ItemName, file, OverwriteOption.Overwrite);
                        string imagename = uploadItem.ItemName + ".jpg";
                        if (isf.FileExists(imagename))
                       {
                           System.IO.IsolatedStorage.IsolatedStorageFileStream PhotoStream = isf.OpenFile(imagename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                           client.UploadAsync(skyDriveFolderID_image, imagename, PhotoStream, OverwriteOption.Overwrite);
                       }
              
                    

                }


            
        }

        void client_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            displayInfo=displayInfo + e.Result.ElementAt(1).Value.ToString()+";";
            infoTextBlock.Text = displayInfo;
        }


        #endregion

        #region download


        private void download(Note selected)
        {



                    if (selected.FileID != null)
                    {
                        
                        itemname = selected.Name;
                        client.DownloadAsync(selected.FileID + "/content");
                        
                        
                    }

                    else
                        MessageBox.Show("该文件不存在", "Error", MessageBoxButton.OK);
                

                
                
            }
            

        void client_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                infoTextBlock2.Text = "Downloading...";
                try
                {
                    using (MemoryStream stream = e.Result as MemoryStream)
                    {
                        //stream = e.Result as MemoryStream;

                        using (var reader = new StreamReader(stream))
                        {


                            txtfile = reader.ReadToEnd();
                            if (App.ViewModel.displayItem(itemname) == null)
                            {

                                ToDoItem newToDoItem = new ToDoItem
                                {
                                    //txtnote需更改
                                    ItemName = itemname,
                                    Category = App.ViewModel.CategoriesList.First(),
                                    CreatTime = DateTime.Now,
                                    EditTime = DateTime.Now,
                                    TxtFile = txtfile

                                };

                                App.ViewModel.AddToDoItem(newToDoItem);



                            }
                            else
                            {
                                ToDoItem toUpdate = App.ViewModel.displayItem(itemname);
                                toUpdate.TxtFile = txtfile;
                                toUpdate.EditTime = DateTime.Now;
                                App.ViewModel.SaveChangesToDB();

                            }
                        }
                    }
                    //如果有所选中，并且选中的就是当前处理的
                    if (k != -1 && k == s_number - 1)
                    {

                        //infoTextBlock2.Text = txtnote.Resources.StringLibrary.download_2;
                        infoTextBlock2.Text = txtnote.Resources.StringLibrary.download_2;
                        // client.DownloadCompleted -= client_DownloadCompleted;
                        client.DownloadCompleted -= client_DownloadCompleted;

                    }
                    else    pickMax();        
                }
                catch (Exception exc)
                {
                   // MessageBox.Show(exc.Message);
                    MessageBox.Show("Click so frequent！Now restart the Page");
                    //NavigationService.GoBack();
                    string uri = String.Format("/txtnote;component/share.xaml");
                    NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                    shareManager.SelectedIndex = 1;
                }
            }
            else
            {
                MessageBox.Show("Unable to download Backup file.", "Failure", MessageBoxButton.OK);
            }

        }









        void merge_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                infoTextBlock3.Text = "Downloading...";
                                try
                {
                using (MemoryStream stream = e.Result as MemoryStream)
                {
                    // stream = e.Result as MemoryStream;
                    using (StreamReader reader = new StreamReader(stream))
                    {

                        if (MessageBox.Show(txtnote.Resources.StringLibrary.download_1, "Alert！", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            downloadMerge(reader);
                        }

                    }
                }
                if (k != -1 && k == s_number - 1)
                {

                    //infoTextBlock2.Text = txtnote.Resources.StringLibrary.download_2;
                    infoTextBlock3.Text = txtnote.Resources.StringLibrary.download_2;
                    // client.DownloadCompleted -= client_DownloadCompleted;
                    client.DownloadCompleted -= merge_DownloadCompleted;

                }
                else pickMaxMerge();     

                    
                    
            
                }


                                catch (Exception exc)
                                {
                                    // MessageBox.Show(exc.Message);
                                    MessageBox.Show("Click so frequent！Now restart the Page");
                                    //NavigationService.GoBack();
                                    string uri = String.Format("/txtnote;component/share.xaml");
                                    NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                                    shareManager.SelectedIndex = 2;
                                }
            }
            else
            {
                MessageBox.Show("Unable to download Backup file.", "Failure", MessageBoxButton.OK);
            }
            
        
    }




        private void downloadMerge(StreamReader reader)
        {

            while (!reader.EndOfStream)
            {
                txtfile = reader.ReadLine();
                string[] abc = txtfile.Split(new Char[] { '\t' });
                if (abc[0] != "")
                {
                    if (App.ViewModel.displayItem(abc[0]) == null)
                    {

                        ToDoItem newToDoItem = new ToDoItem
                        {
                            //txtnote需更改
                            ItemName = abc[0],
                            Category = App.ViewModel.CategoriesList.First(),
                            CreatTime = DateTime.Now,
                            EditTime = DateTime.Now,
                            TxtFile = abc[1]

                        };

                        App.ViewModel.AddToDoItem(newToDoItem);

                    }
                    else
                    {
                        ToDoItem toUpdate = App.ViewModel.displayItem(abc[0]);
                        toUpdate.TxtFile = abc[1];
                        toUpdate.EditTime = DateTime.Now;
                        App.ViewModel.SaveChangesToDB();
                    }

                }
            }
           // NavigationService.GoBack();
        }

        private void pickMax()
        {


           
            while (s_number <=k)
            {

                if (Notes.ElementAt(s_number).IsSelected == true)
                {
                    download(Notes.ElementAt(s_number));
                   // k = s_number;
                    s_number = s_number + 1;
                    break;
                }
                else
                {
                    s_number = s_number + 1;
                }

            }
            //如果没有任何项被选中
            if (k == -1)
            {
                client.DownloadCompleted -= client_DownloadCompleted;
            }




        }

        private void pickMaxMerge()
        {

            
            while (s_number < Notes_merge.Count())
            {

                if (Notes_merge.ElementAt(s_number).IsSelected == true)
                {
                    download(Notes_merge.ElementAt(s_number));
                    k = s_number;
                    s_number = s_number + 1;
                    break;
                }
                else
                {
                    s_number = s_number + 1;
                }

            }

            //如果没有任何项被选中
            if (k == -1)
            {
                client.DownloadCompleted -= merge_DownloadCompleted;
            }


                            

        }


        #endregion

        #region appBar


        private void shareManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (shareManager.SelectedIndex)
            {

                case 0:
                    appBar1();
                    break;

                case 1:
                    appBar2();
                    
                    break;


                default:
                    appBar2();
                    
                   // appBar3();
                    break;
            }


        }



        void appBar1()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/upload.png", UriKind.Relative);
            button1.Text = txtnote.Resources.StringLibrary.skydrive;
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/appbar.upload.rest.png", UriKind.Relative);
            button2.Text = txtnote.Resources.StringLibrary.shareInfo_5;
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.email.png", UriKind.Relative);
            button3.Text = txtnote.Resources.StringLibrary.email;
            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/favs.addto.png", UriKind.Relative);
            button4.Text = txtnote.Resources.StringLibrary.shareInfo_4;
            ApplicationBar.Buttons.Add(button1);
            ApplicationBar.Buttons.Add(button2);
            ApplicationBar.Buttons.Add(button3);
            ApplicationBar.Buttons.Add(button4);
            button1.Click += new EventHandler(upload_Button_Click);
            button2.Click += new EventHandler(merge_Button_Click);
            button3.Click += new EventHandler(mail_Button_Click);
            button4.Click += new EventHandler(weibo_Button_Click);
        }
        void appBar2()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/download.png", UriKind.Relative);
            button1.Text = txtnote.Resources.StringLibrary.xiazai;
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/appbar.questionmark.rest.png", UriKind.Relative);
            button2.Text = txtnote.Resources.StringLibrary.bangzhu;
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.email.png", UriKind.Relative);
            button3.Text = txtnote.Resources.StringLibrary.email;
            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/favs.addto.png", UriKind.Relative);
            button4.Text = txtnote.Resources.StringLibrary.shareInfo_4;
            ApplicationBar.Buttons.Add(button1);
            ApplicationBar.Buttons.Add(button3);
            ApplicationBar.Buttons.Add(button4);
            ApplicationBar.Buttons.Add(button2);
            button1.Click += new EventHandler(download_Click);
            
            button3.Click += new EventHandler(mail_Button_Click);
            button4.Click += new EventHandler(weibo_Button_Click);
            button2.Click += new EventHandler(Help_Button_Click);
        }

        private void Help_Button_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/txtnote;component/Help.xaml", UriKind.Relative));
        }



        private void download_Click(object sender, EventArgs e)
        {
            //每次点击，重置参数
            s_number = 0;
         //  pickMaxNumber = -2;
           // k=-1;
            if (client == null || client.Session == null)
            {
                MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_1);
            }
            else
            {
                if (Notes.Count() == 0)
                {
                    MessageBox.Show("Please wait!");
                }
                else
                {
                    if (MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_7, txtnote.Resources.StringLibrary.xiazai, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        k = Notes.Count() - 1;
                        while (k >= 0)
                        {

                            if (Notes.ElementAt(k).IsSelected == true)
                            {
                                break;
                            }
                            else
                            {
                                k = k - 1;
                            }

                        }
                        if (shareManager.SelectedIndex == 1)
                        {

                            client.DownloadCompleted += client_DownloadCompleted;
                            pickMax();
                        }
                        else
                        {
                            client.DownloadCompleted += merge_DownloadCompleted;
                            pickMaxMerge();
                        }
                    }
                }


            }
        }
        private void weibo_Button_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = "txtNote"+txtnote.Resources.StringLibrary.fenxiang+"："+"\r\n"+Merge();

            shareStatusTask.Show();
        }

        private void mail_Button_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "txtNote" + txtnote.Resources.StringLibrary.fenxiang;
            emailComposeTask.Body = Merge();
            emailComposeTask.To = "";
            

            emailComposeTask.Show();
        }

        private void merge_Button_Click(object sender, EventArgs e)
        {
            if (client == null || client.Session == null)
            {
                MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_1);
            }
            else
            {
                if (infoTextBlock.Text == txtnote.Resources.StringLibrary.shareInfo_3 || infoTextBlock.Text.Contains(txtnote.Resources.StringLibrary.chenggong))
                {
                    if (MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_6, txtnote.Resources.StringLibrary.skydrive, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        byte[] b2 = System.Text.Encoding.UTF8.GetBytes(Merge());
                        MemoryStream file = new MemoryStream(b2);


                        try
                        {
                            client.UploadAsync(skyDriveFolderID_merge, "txtNote_Merge.txt", file, OverwriteOption.Overwrite);

                            client.UploadCompleted += client_UploadCompleted;
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please wait!");
                }
            }
        }

        private String Merge()
        {
                  var updateIe = App.ViewModel.AllToUpdateItems.AsQueryable();
                  String tempStr = "";

                  foreach (ToDoItem uploadItem in updateIe)
                  {
                      tempStr = tempStr + uploadItem.ItemName + "\t" + uploadItem.TxtFile + "\r\n";
                  }
                  return tempStr;

        }

        private void upload_Button_Click(object sender, EventArgs e)
        {
            if (client == null || client.Session == null)
            {
                MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_1);
            }
            else
            {
                if (infoTextBlock.Text == txtnote.Resources.StringLibrary.shareInfo_3 || infoTextBlock.Text.Contains(txtnote.Resources.StringLibrary.chenggong))
                {
                    if (MessageBox.Show(txtnote.Resources.StringLibrary.shareInfo_6, txtnote.Resources.StringLibrary.skydrive, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        UploadFile();
                }
                else
                    MessageBox.Show("Please wait!");
            }
        }
        #endregion

     /*   private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.CleanCheckBox();
        }
      * */





    }
}