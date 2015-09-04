/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the data model.
using LocalDatabaseSample.Model;
using System;
using txtnote;




namespace LocalDatabaseSample.ViewModel
{
    public class ToDoViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private ToDoDataContext toDoDB;

        // Class constructor, create the data context object.
        public ToDoViewModel(string toDoDBConnectionString)
        {
            toDoDB = new ToDoDataContext(toDoDBConnectionString);
        }

        // All to-do items.
        private ObservableCollection<ToDoItem> _allToDoItems;
        public ObservableCollection<ToDoItem> AllToDoItems
        {
            get { return _allToDoItems; }
            set
            {
                _allToDoItems = value;
                NotifyPropertyChanged("AllToDoItems");
            }
        }


        // All to-upload items.
        private ObservableCollection<ToDoItem> _allToUpdateItems;
        public ObservableCollection<ToDoItem> AllToUpdateItems
        {
            get { return _allToUpdateItems; }
            set
            {
                _allToUpdateItems = value;
                NotifyPropertyChanged("AllToUpdateItems");
            }
        }

        // To-do items associated with the home category.
        private ObservableCollection<ToDoItem> _homeToDoItems;
        public ObservableCollection<ToDoItem> HomeToDoItems
        {
            get { return _homeToDoItems; }
            set
            {
                _homeToDoItems = value;
                NotifyPropertyChanged("HomeToDoItems");
            }
        }

        // To-do items associated with the work category.
        private ObservableCollection<ToDoItem> _workToDoItems;
        public ObservableCollection<ToDoItem> WorkToDoItems
        {
            get { return _workToDoItems; }
            set
            {
                _workToDoItems = value;
                NotifyPropertyChanged("WorkToDoItems");
            }
        }

        // To-do items associated with the hobbies category.
        private ObservableCollection<ToDoItem> _hobbiesToDoItems;
        public ObservableCollection<ToDoItem> HobbiesToDoItems
        {
            get { return _hobbiesToDoItems; }
            set
            {
                _hobbiesToDoItems = value;
                NotifyPropertyChanged("HobbiesToDoItems");
            }
        }

       


        // A list of all categories, used by the add task page.
        private List<ToDoCategory> _categoriesList;
        public List<ToDoCategory> CategoriesList
        {
            get { return _categoriesList; }
            set
            {
                _categoriesList = value;
                NotifyPropertyChanged("CategoriesList");
            }
        }

        


        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            toDoDB.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all to-do items in the database.
            //  var toDoItemsInDB = from ToDoItem todo in toDoDB.Items
            //     select todo;

            // Query the database and load all to-do items.
            //  AllToDoItems = new ObservableCollection<ToDoItem>(toDoItemsInDB);

            // Specify the query for all categories in the database.
            var toDoCategoriesInDB = from ToDoCategory category in toDoDB.Categories
                                     select category;

            txtnote.Resources.StringLibrary.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            // Query the database and load all associated items to their respective collections.
            CategoriesList = toDoDB.Categories.ToList();
            CategoriesList.ElementAt(0).Name = txtnote.Resources.StringLibrary.richang;
            CategoriesList.ElementAt(1).Name = txtnote.Resources.StringLibrary.gongzuo;
            CategoriesList.ElementAt(2).Name = txtnote.Resources.StringLibrary.xingqu;

            foreach (ToDoCategory category in toDoCategoriesInDB)
            {
                if (category.Id == 1)
                {


                    HomeToDoItems = new ObservableCollection<ToDoItem>(category.ToDos);
                }
                else if (category.Id == 2)
                {
                    WorkToDoItems = new ObservableCollection<ToDoItem>(category.ToDos);
                }
                else
                {
                    HobbiesToDoItems = new ObservableCollection<ToDoItem>(category.ToDos);
                }



            }

            // Load a list of all categories.
            SaveChangesToDB();
            //  string abc=CategoriesList.ElementAt(1).Name;
        }


        public ToDoItem forShare = null;
        

        //when serchbox has changed,update the results
        public void Search(String queryString,bool  textQuery)
        {
            if (queryString == txtnote.Resources.StringLibrary.searchBox)
            {
                queryString = "";
            }


            setting abc = new setting();

         

            if (textQuery == true)
            {
                if (abc.RadioButton1Setting)
                {
                    var query =
        from ord in toDoDB.Items
        where ord.ItemName.Contains(queryString) || ord.TxtFile.Contains(queryString)
        orderby ord.EditTime descending
        select ord;
                    AllToDoItems = new ObservableCollection<ToDoItem>(query);
                }
                else
                {
                    if (abc.RadioButton2Setting)
                    {
                        var query =
         from ord in toDoDB.Items
         where ord.ItemName.Contains(queryString) || ord.TxtFile.Contains(queryString)
         orderby ord.ItemName descending
         select ord;
                        AllToDoItems = new ObservableCollection<ToDoItem>(query);
                    }
                    else
                    {
                        var query =
    from ord in toDoDB.Items
    where ord.ItemName.Contains(queryString) || ord.TxtFile.Contains(queryString)
    orderby ord.CreatTime descending
    select ord;
                        AllToDoItems = new ObservableCollection<ToDoItem>(query);

                    }
                }


               
            }
            else
            {
                if (abc.RadioButton1Setting)
                {

                    var query =
                        from ord in toDoDB.Items
                        where ord.ItemName.Contains(queryString)
                        orderby ord.EditTime descending
                        select ord;
                    AllToDoItems = new ObservableCollection<ToDoItem>(query);
                }
                else
                {
                    if (abc.RadioButton2Setting)
                    {

                        var query =
                            from ord in toDoDB.Items
                            where ord.ItemName.Contains(queryString)
                            orderby ord.ItemName 
                            select ord;

                        AllToDoItems = new ObservableCollection<ToDoItem>(query);
                    }
                    else
                    {
                        var query =
    from ord in toDoDB.Items
    where ord.ItemName.Contains(queryString)
    orderby ord.CreatTime descending
    select ord;


                        AllToDoItems = new ObservableCollection<ToDoItem>(query);

                    }
                }

            }


        }

        //clean the checkbox
        public void CleanCheckBox()
        {
            foreach (ToDoItem ord in toDoDB.Items)
            {
                ord.IsComplete = false;
            }
            SaveChangesToDB();
           
        }


        // Add a to-do item to the database and collections.
        public void AddToDoItem(ToDoItem newToDoItem)
        {
            // Add a to-do item to the data context.
            toDoDB.Items.InsertOnSubmit(newToDoItem);

            // Save changes to the database.
            

            // Add a to-do item to the "all" observable collection.
            //AllToDoItems.Add(newToDoItem);

            // Add a to-do item to the appropriate filtered collection.


            if (newToDoItem.Category.Id == 1)
            {


                HomeToDoItems.Add(newToDoItem);
            }
            else if (newToDoItem.Category.Id == 2)
            {
                WorkToDoItems.Add(newToDoItem);
            }
            else if (newToDoItem.Category.Id == 3)
            {
                HobbiesToDoItems.Add(newToDoItem);
            }

            toDoDB.SubmitChanges();

        }


        public void UpdateItem()
        {
            var query =
                    from ord in toDoDB.Items
                    where ord.IsComplete == true
                    select ord;
            AllToUpdateItems = new ObservableCollection<ToDoItem>(query);
        
        }
        public ToDoItem displayItem2()
        {
            var query =
        from ord in toDoDB.Items
        where ord.IsComplete == true
        select ord;
            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }

        }
        public void DeleteSelecItem()
        {
            var query =
                    from ord in toDoDB.Items
                    where ord.IsComplete == true
                    select ord;
            foreach (ToDoItem ord in query)
            {
                DeleteToDoItem(ord);
            }
            

        }


        // Edit a to-do item to the database and collections.



        public void changeAllItem(string content)
        {



            if (content == "0")
            {


                AllToDoItems = HomeToDoItems;
            }
            else if (content == "1")
            {
                AllToDoItems = WorkToDoItems;
            }
            else if (content == "2")
            {
                AllToDoItems = HobbiesToDoItems;
            }
            else Search("",false);



        }

        //return editting item
        public ToDoItem displayItem(string fileName)
        {

            var query =
                from ord in toDoDB.Items
                where ord.ItemName == fileName
                orderby ord.EditTime
                select ord;

            foreach (ToDoItem ord in query)
            {
                    return ord;
            }
            return null;
        }

        public int displayItemCount(string fileName)
        {

            var query =
                from ord in toDoDB.Items
                where ord.ItemName.Contains(fileName)
                select ord;


            return query.Count();
        }

        
        //rename
        public void RenameToDoItem(ToDoItem toDoForRename,String newName)
        {
            toDoForRename.ItemName = newName;
        }

        // Remove a to-do task item from the database and collections.
        public void DeleteToDoItem(ToDoItem toDoForDelete)
        {

            // Remove the to-do item from the "all" observable collection.
            AllToDoItems.Remove(toDoForDelete);

            // Remove the to-do item from the data context.
            toDoDB.Items.DeleteOnSubmit(toDoForDelete);

            // Remove the to-do item from the appropriate category.   

            if (toDoForDelete.Category.Id == 1)
            {


                HomeToDoItems.Remove(toDoForDelete);
            }
            else if (toDoForDelete.Category.Id == 2)
            {
                WorkToDoItems.Remove(toDoForDelete);
            }
            else
            {
                HobbiesToDoItems.Remove(toDoForDelete);
            }




            // Save changes to the database.
            toDoDB.SubmitChanges();
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}