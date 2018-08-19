using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth
{
    class Brick:System.Windows.Forms.PictureBox
    {
        private int width;

        private int height;

        private bool is_visited = false;

        public bool IsVisited
        {
            get { return is_visited; }
            set { is_visited = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

    }
}
