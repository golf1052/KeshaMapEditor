using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KeshaMapEditor
{
    public class TileImage
    {
        public Image image;
        public int x;
        public int y;

        public TileImage(Image image, int x, int y)
        {
            this.image = image;
            this.x = x;
            this.y = y;
        }
    }
}
