using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeshaMapEditor
{
    public class LayerListViewBinding
    {
        public string LayerName { get; set; }
        public int layerIndex;

        public LayerListViewBinding(string layerName, int layerIndex)
        {
            this.LayerName = layerName;
            this.layerIndex = layerIndex;
        }
    }
}
