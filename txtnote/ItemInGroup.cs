using LocalDatabaseSample.Model;
using System.Collections.Generic;



namespace txtnote
{

    /// <summary>

    /// 组集合

    /// </summary>

    public class ItemInGroup : List<Item>
    {

        public ItemInGroup(string category)
        {

            Key = category;

        }

        //组的键

        public string Key { get; set; }

        //组是否有选项

        public bool HasItems { get { return Count > 0; } }

    }

}