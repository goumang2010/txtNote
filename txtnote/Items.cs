using LocalDatabaseSample.Model;
using System.Collections.Generic;

using System.Linq;

namespace txtnote
{

    /// <summary>

    /// 总数据集合

    /// </summary>

    public class Items : List<ItemInGroup>
    {

        //索引

        private static readonly string Groups = "#|a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z";
        


        public Items()
        {

            //获取要绑定的数据源

           List<Item> items = new List<Item>();
           var updateIe = App.ViewModel.AllToDoItems.AsQueryable();
          foreach (ToDoItem updateItem in updateIe)
            {
                items.Add(new Item { Name = updateItem.ItemName, Content = updateItem.TxtFile });
            }
            
            //组的字典列表

            Dictionary<string, ItemInGroup> groups = new Dictionary<string, ItemInGroup>();
            //初始化组列表，即用字母列表来分组

            foreach (string c in Groups.Split('|'))
            {

                ItemInGroup group = new ItemInGroup(c.ToString());

                //添加组数据到集合

                this.Add(group);

                groups[c.ToString()] = group;

            }

            //初始化选项列表，即按照选项所属的组来放进它属于的组里面

            foreach (Item item in items)
            {

                //添加选项数据到集合

                groups[Item.GetFirstNameKey(item)].Add(item);

            }

        }

    }

}
