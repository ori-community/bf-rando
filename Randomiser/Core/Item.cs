using System.IO;
using Newtonsoft.Json;

namespace Randomiser
{
    public struct Item
    {
        public int id;
        public string name;
        public string displayName;
    }

    public class RandomiserItems
    {
        Item[] items;

        public Item this[int index] => items[index];

        void LoadItems(string path)
        {
            items = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText(path));

            // you get the idea
        }
    }
}
