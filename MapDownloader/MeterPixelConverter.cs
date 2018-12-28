using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader
{
    class MeterPixelConverter
    {
      
        public double PixelResolution(double latitude, int level, double distance)
        {
            return distance/((Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2.0, level)));
        }

     

        public int GetLevel(double latitude, int nbPixel,double distance)
        {
            double MeterPerPixel = distance/nbPixel  ;
            double Level = Math.Log((Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (MeterPerPixel*256), 2);
            return (int) Math.Floor(Level);
        }

       
    }
}
