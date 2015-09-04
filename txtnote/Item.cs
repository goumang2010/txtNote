using System.Windows.Media.Imaging;
namespace txtnote
{

    /// <summary>

    /// 选项实体类

    /// </summary>

    public class Item
    {

        public string Name { get; set; }

        public string Content { get; set; }

        

        //获取名字的首个字符用来作为分组的依据

        public static string GetFirstNameKey(Item item)
        {

            char key;

            key = char.ToLower(item.Name[0]);

            if (key < 'a' || key > 'z')
            {

                key = '#';

            }

            return key.ToString();

        }

    }

}