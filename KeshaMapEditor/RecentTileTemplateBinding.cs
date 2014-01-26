using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeshaMapEditor
{
    public class RecentTileTemplateBinding
    {
        public string Name { get; set; }
        public Uri Image { get; set; }

        public RecentTileTemplateBinding(string filename)
        {
            this.Image = new Uri(filename);
            string[] splitPath = Image.ToString().Split('/');
            string[] secondSplit = splitPath[splitPath.Length - 1].Split('.');
            this.Name = secondSplit[0];
        }
    }
}
