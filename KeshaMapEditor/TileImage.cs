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
        public int layer;

        public TileImage(Image image, int x, int y, int layer)
        {
            this.image = image;
            this.x = x;
            this.y = y;
            this.layer = layer;
        }
    }
}
