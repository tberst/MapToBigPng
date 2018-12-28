using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader
{
    public class PixelDpiConverter
    {
        private int dpi;

        private double inch = 2.54;
        
        public PixelDpiConverter(int dpi)
        {
            this.dpi = dpi;
        }
        public int CmToPixels(double Cm)
        {
            return (int)Math.Floor(Cm / inch * dpi) ;
        }
    }
}
