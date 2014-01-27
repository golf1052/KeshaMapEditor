using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeshaMapEditor
{
    public class TileTemplateBinding
    {
        public Uri Image { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Collide { get; set; }
        public string LayerName { get; set; }

        public int imageSize;
        public int internalX;
        public int internalY;
        public int layer;

        public TileTemplateBinding(string image, int imageSize)
        {
            this.Image = new Uri(image);
            this.X = 0;
            this.Y = 0;
            this.Collide = false;
            this.imageSize = imageSize;
            this.internalX = 0;
            this.internalY = 0;
            this.LayerName = "Layer 0";
            this.layer = 0;
        }

        public TileTemplateBinding(string image, int imageSize, int x, int y, bool collide, int layer)
        {
            this.Image = new Uri(image);
            this.X = x;
            this.Y = y;
            this.Collide = collide;
            this.imageSize = imageSize;
            this.internalX = X * imageSize;
            this.internalY = Y * imageSize;
            this.LayerName = "Layer " + layer.ToString();
            this.layer = layer;
        }

        public void Refresh()
        {
            internalX = X * imageSize;
            internalY = Y * imageSize;
        }
    }
}
