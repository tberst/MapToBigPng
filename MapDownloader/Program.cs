using RestSharp;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader
{
    class Program
    {

        private static int DPI_RESOLUTION = 300;
        private static int TILE_SIZE = 256;
        static void Main(string[] args)
        {

            var dpiConverter = new PixelDpiConverter(DPI_RESOLUTION);
            var meterPixelConverter = new MeterPixelConverter();
            GeoCoordinate topleft = new GeoCoordinate(47.791522, -122.5307717);
            GeoCoordinate bottomright = new GeoCoordinate(47.448477, -122.1473697);

        
            double widthInCm = 180;
        
            int widhtInPixel = dpiConverter.CmToPixels(widthInCm);

            var horizontalDistance = topleft.GetDistanceTo(new GeoCoordinate(topleft.Latitude, bottomright.Longitude));
            var verticalDistance = topleft.GetDistanceTo(new GeoCoordinate(bottomright.Latitude, topleft.Longitude));

            int level = meterPixelConverter.GetLevel(topleft.Latitude, widhtInPixel, horizontalDistance);
            double heightInPixel = meterPixelConverter.PixelResolution(bottomright.Latitude, level, verticalDistance);



            int nbTilesHeight = ((int)heightInPixel / TILE_SIZE) + 2;
            int nbTilesWidth = (widhtInPixel / TILE_SIZE) + 2;
            string quad;


            var baseUrl = "http://ak.dynamic.t0.tiles.virtualearth.net";
            var urlFormatBase = "/comp/ch/{0}?mkt=en-US&it=G,L&shading=hill&og=341&n=z&";
            string urlFormat = "";
           
            urlFormat = urlFormatBase +
                "st=" + //Style
                "ar|v:0_" + //area
                "wt|fc:e1e1e1_" + //water
                "rd|fc:000000;sc:a1a1a1_" + //road
                "st|fc:a1a1a1_" + //street
                "rl|fc:000000_" + //railway
                "trl|fc:000000_" + //trail
                "bld|sc:FF00FF;fc:F7F7F7_" + //building
                "wr|v:0_" + //water route
                "trs|lv:0_" + //transportation
                "pt|lv:0_" + //point
                "re|v:0_" + //road exit
                "trn|v:0_" + //transit (Icon representing a bus stop, train stop, airport, etc.)
                "mapElement|lv:0_" +
                "g|lc:ffffff";
            int x, y;



            TileSystem.LatLongToPixelXY(topleft.Latitude, topleft.Longitude, level, out x, out y);

            Bitmap display = new Bitmap(256 * nbTilesWidth, 256 * nbTilesHeight,PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(display);

            int totalTiles = nbTilesHeight * nbTilesWidth;

            int index = 0;
            for (int i = 0; i < nbTilesWidth; i++)
            {
                for (int j = 0; j < nbTilesHeight; j++)
                {
                    index++;
                    var xx = (x / 256 + i);
                    var yy = (y / 256 + j);
                    double progress = index * 100 / (double)totalTiles;
                    Console.WriteLine($"progress = {progress}  | {index}/{totalTiles}  |   x = {xx}, y={ yy}, level = {level}");
                    try
                    {
                        // using (var writer = File.OpenWrite(tempFile))
                        using (var writer = new MemoryStream())
                        {
                            quad = TileSystem.TileXYToQuadKey(xx, yy, level);
                            var client = new RestClient(baseUrl);
                            string url = String.Format(urlFormat, quad);
                            var request = new RestRequest(url);
                            Console.WriteLine(url);
                            request.ResponseWriter = (responseStream) => responseStream.CopyTo(writer);
                            client.DownloadData(request);
                            writer.Seek(0, SeekOrigin.Begin);
                            using (Image image = new Bitmap(writer))
                            {
                                g.DrawImage(image, i * 256, j * 256);
                            }
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                   catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }

                }
            }
            display.Save($"result{level}.jpg");



        }
    }
}
