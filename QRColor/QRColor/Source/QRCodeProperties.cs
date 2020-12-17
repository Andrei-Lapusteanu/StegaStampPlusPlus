using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRColor.Source
{

    struct QRSize
    {
        public QRSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        int width;
        int height;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
    }


    class QRCodeProperties
    {
        private QRSize imgSize;
        private int cellSize;
        private int cellsPerDim;

        public QRCodeProperties(int width, int height, int cellSize)
        {
            this.imgSize = new QRSize(width, height);
            this.cellSize = cellSize;
            this.cellsPerDim = width / cellSize;
        }

        internal QRSize ImgSize { get => imgSize; set => imgSize = value; }
        public int CellSize { get => cellSize; set => cellSize = value; }
        public int CellsPerDim { get => cellsPerDim; set => cellsPerDim = value; }
    }
}
