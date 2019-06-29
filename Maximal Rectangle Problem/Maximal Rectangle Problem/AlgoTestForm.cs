using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximal_Rectangle_Problem
{
    public partial class AlgoTestForm : Form
    {
        public AlgoTestForm()
        {
            InitializeComponent();

        }

        private const int COL_COUNT = 10;
        private const int ROW_COUNT = 10;
        private int[,] createSourceArray()
        {

            Random rnd = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

            var result = new int[COL_COUNT, ROW_COUNT];
            for (int x = 0; x < result.GetLength(0); ++x)
            {
                for (int y = 0; y < result.GetLength(1); ++y)
                {
                    int value = rnd.Next(2);
                    result[x,y] = value;
                }
            }
            return result;
        }

        private void printGrid(int[,] data)
        {
            Console.WriteLine("  0123456789");
            for (int y = 0; y < ROW_COUNT; ++y)
            {
                Console.Write(y.ToString() + ":");
                for (int x = 0; x < COL_COUNT; ++x)
                {
                    Console.Write(data[x,y].ToString());
                }
                Console.WriteLine();
            }
        }

        private void AlgoTestForm_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 40; ++i)
            {
                int[,] source = createSourceArray();
                printGrid(source);

                Rectangle rect = maximalRectBruteForce(source);

                Console.WriteLine("{0},{1} {2},{3}   {4} x {5}", rect.X, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height - 1, rect.Width, rect.Height);
            }
        } // END OF CLASS

        private Rectangle maximalRectBruteForce(int[,] data)
        {
            Point bestLL = new Point(0, 0);
            Point bestUR = new Point(-1, -1);

            for (int llx = 0; llx < COL_COUNT; ++llx)
            {
                for (int lly = 0; lly < ROW_COUNT; ++lly)
                {
                    for (int urx = llx; urx < COL_COUNT; ++urx)
                    {
                        for (int ury = lly; ury < ROW_COUNT; ++ury)
                        {
                            //Console.WriteLine("{0},{1} {2},{3}", llx, lly, urx, ury);
                            if (area(llx, lly, urx, ury) > area(bestLL, bestUR) && allOnes(llx, lly, urx, ury, data))
                            {
                                bestLL.X = llx;
                                bestLL.Y = lly;
                                bestUR.X = urx;
                                bestUR.Y = ury;
                                //Console.WriteLine("BEST: {0},{1} {2},{3}", bestLL.X, bestLL.Y, bestUR.X, bestUR.Y);
                            }
                        }
                    }
                }
            }

            return new Rectangle(bestLL.X, bestLL.Y, bestUR.X - bestLL.X + 1, bestUR.Y - bestLL.Y + 1);
        }

        /*private int area(Rectangle rect)
        {
            Point ll = new Point(rect.Left, rect.Bottom);
            Point ur = new Point(rect.Right, rect.Top);
            return area(ll, ur);
        }*/

        private int area(int llx, int lly, int urx, int ury)
        {
            if (llx > urx || lly > ury)
                return 0;
            else
                return (urx - llx + 1) * (ury - lly + 1);
        }

        private int area(Point lowerLeft, Point upperRight)
        {
            if (lowerLeft.X > upperRight.X || lowerLeft.Y > upperRight.Y)
                return 0;
            else
                return (upperRight.X - lowerLeft.X + 1) * (upperRight.Y - lowerLeft.Y + 1);
        }

        private bool allOnes(int llx, int lly, int urx, int ury, int[,] data)       
        {            
            //Console.WriteLine("all ones ({0},{1} {2},{3})", llx, lly, urx, ury);
            for (int x = llx; x <= urx; ++x)
            {
                for (int y = lly; y <= ury; ++y)
                {
                    if (data[x,y] == 0) return false;
                }
            }
            return true;
        }
    }
} // END OF NAMESPACE
