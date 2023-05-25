using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace img2ascii
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

            
                Console.WriteLine("Lütfen hangi resmi girmek istediğinizi yazın.");
                string[] imageNames = Directory.GetFiles("input");

                for (int i = 0; i < imageNames.Length; i++)
                {
                    Console.WriteLine(i + "- " + imageNames[i].Substring(imageNames[i].IndexOf('\\') + 1, imageNames[i].Length - 1 - imageNames[i].IndexOf('\\')));
                }
                //Console.WriteLine("s- Settings");
                Console.WriteLine("q- Quit");
                

                



            

                //9 width ve 19 height'lik parçalar halinde taramak lazım, ondan sonra ortalama parlaklığını alıp 0-7 arasındaki bir değere küçültmek lazım. 
                //resim widthini 9'a bölüp ceil değerini almak mantıklı olabilir. aynı şekilde height değerini de 19'a bölüp ceilini almak.
                Bitmap image;
                float compression = 1;
                int cellHeight = 4;
                int cellWidth = 2; // Konsoldaki 1 hücrenin boyutları.
                int brightSum = 0; //Ortalama hesaplanmadan önceki toplam parlaklık. Bu değeri pixel sayısına böleceğiz.
                int pixCnt = (cellHeight) * (cellWidth);
                float brightAvg;
                bool showProcess = true;
                bool invert = false;
                string symbols = " .,*/(#%&@";
                string selection = Console.ReadLine().ToLower();
                switch (selection)
                {
                    case "q":
                        System.Environment.Exit(0);
                        continue;
                    //case "s":
                    //    float compressionTemp = compression;
                    //    int cellHeightTemp = cellHeight;
                    //    int cellWidthTemp = cellWidth;
                    //    bool brk = false;
                    //    while (true)
                    //    {
                    //        if (brk)
                    //        {
                    //            break;
                    //        }
                    //        Console.Clear();
                    //        Console.WriteLine("1- Change Compression: " + compressionTemp);
                    //        Console.WriteLine("2- Change Height of a cell in pixels: " + cellHeightTemp);
                    //        Console.WriteLine("3- Change Width of a cell in pixels: " + cellWidthTemp);
                    //        Console.WriteLine("s- Save And Quit");
                    //        Console.WriteLine("q- Quit Without Saving");
                    //        switch (Console.ReadLine().ToLower()[0])
                    //        {
                    //            case '1':
                    //                Console.WriteLine("Lütfen yeni değeri giriniz:");
                    //                compressionTemp = float.Parse(Console.ReadLine());
                    //                continue;
                    //            case '2':
                    //                Console.WriteLine("Lütfen yeni değeri giriniz:");
                    //                cellHeightTemp = int.Parse(Console.ReadLine());

                    //                continue;
                    //            case '3':
                    //                Console.WriteLine("Lütfen yeni değeri giriniz:");
                    //                cellWidthTemp = int.Parse(Console.ReadLine());

                    //                continue;
                    //            case 's':
                    //                compression = compressionTemp;
                    //                cellHeight = cellHeightTemp;
                    //                cellWidth = cellWidthTemp;
                    //                brk = true;
                    //                continue;
                    //            case 'q':
                    //                brk = true;
                    //                continue;
                    //        }

                    //    }
                    //    continue;
                    default:
                        try
                        {
                            image = new Bitmap(Image.FromFile(imageNames[int.Parse(selection)]));
                        }
                        catch (Exception e)
                        {
                            Console.Clear();
                            Console.WriteLine(e.Message + "\n");
                            continue;
                        }
                        break;
                }
                cellHeight = (int)Math.Ceiling((float)cellHeight * compression);
                cellWidth = (int)Math.Ceiling((float)cellWidth * compression);

                //Resmin en sol üst noktası, 0, 0 ise sağ alt noktası width-1, height-1
                if (true) // hesabı açmak.
                {
                    string cOut = "";
                    for (int topY = 0; topY < image.Height; topY += cellHeight) // topleft değişkeni taranacak alanın sol üst kısmını belirtir. Satır satır taranacak.
                    {
                        for (int leftX = 0; leftX < image.Width; leftX += cellWidth)
                        {
                            //Console.Write("W: " + leftX + " to " + (leftX + 9) + "  H: " + topY + " to " + (topY + 19) + "\n");
                            for (int yPtr = topY; yPtr < topY + cellHeight; yPtr++)
                            {
                                for (int xPtr = leftX; xPtr < leftX + cellWidth; xPtr++)
                                {
                                    Color currentPixel;
                                    int currBrightness;
                                    if((xPtr < image.Width - cellWidth) && (yPtr < image.Height - cellHeight))
                                    {
                                        currentPixel = image.GetPixel(xPtr, yPtr);
                                        currBrightness = getBrightness(currentPixel);
                                    }
                                    else
                                    {
                                        if (invert)
                                        {
                                            currBrightness = 255;
                                        }
                                        else
                                        {
                                            currBrightness = 0;
                                        }
                                    }
                                    if (invert)
                                    {
                                        brightSum += 255-currBrightness;
                                    }
                                    else
                                    {
                                        brightSum += currBrightness;
                                    }
                                
                                }
                            }
                            brightAvg = (float)brightSum / pixCnt;
                            int charIndex = (int)Math.Floor((float)brightAvg/ (256 / symbols.Length + 1));
                            if (charIndex > symbols.Length-1)
                            {
                                Console.WriteLine("Error between:\nX " + leftX + "-" + (leftX + cellWidth) + "\nY " + topY + "-" + (topY + cellHeight) + "\nBrightness: " + brightAvg + "\nChar Index: " + (float)brightAvg / (256 / symbols.Length + 1));
                                charIndex = symbols.Length-1;
                            }
                            Console.Write(symbols[charIndex]);
                            if (showProcess)
                            {
                                brightSum = 0; //bir sonraki hesaplama için temizleniyor.
                            }
                            else
                            {
                                cOut += symbols[charIndex];
                            }
                        }
                        if (showProcess)
                        {
                            Console.WriteLine();
                        }
                        else
                        {
                            cOut += "\n";
                        }
                    }
                    if (!showProcess)
                    {
                        Console.WriteLine(cOut);
                    }
                }
                Console.ReadLine();
                Console.Clear();
            }
        }
        static int getBrightness(Color inC)
        {
            return (int)Math.Ceiling(Math.Sqrt(inC.R * inC.R * 0.241 + inC.G * inC.G * 0.691 + inC.G * inC.G * 0.068));
        }
       
    }
}
