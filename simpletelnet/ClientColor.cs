using System;
using System.Linq;

namespace Telnet.UI
{
    public class Color
    {
    	public static Tools tools = new Tools();
        public void reset_color() //Reset Color
        {
          tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'m' });
        }
        public void setForegroundColor(ConsoleColor fcolor) //Set the foreground color
        {
            if (fcolor == ConsoleColor.DarkGray)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'0', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Red)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'9', (byte)'1', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Green)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'2', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Yellow)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'3', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Blue)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'4', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Magenta)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'5', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Cyan)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'9', (byte)'6', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.White)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'7', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Black)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'0', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkRed)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'1', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkGreen)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'2', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkYellow)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'3', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkBlue)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'4', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkMagenta)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'5', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkCyan)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'6', (byte)'m' });
            else if (fcolor == ConsoleColor.Gray)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'7', (byte)'m' });
        }
        //Set background color
        public void setBackgroundColor(ConsoleColor bcolor)
        {
            if (bcolor == ConsoleColor.Black)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'0', (byte)'m' });
            else if (bcolor == ConsoleColor.Red)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'1', (byte)'m' });
            else if (bcolor == ConsoleColor.Green)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'2', (byte)'m' });
            else if (bcolor == ConsoleColor.Yellow)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'3', (byte)'m' });
            else if (bcolor == ConsoleColor.Blue)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'4', (byte)'m' });
            else if (bcolor == ConsoleColor.Magenta)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'5', (byte)'m' });
            else if (bcolor == ConsoleColor.Cyan)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'6', (byte)'m' });
            else if (bcolor == ConsoleColor.White)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'7', (byte)'m' });
        }
        //Set before and after views
        public void setColor(ConsoleColor fcolor, ConsoleColor bcolor)
        {
            if (fcolor == ConsoleColor.DarkGray)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'0', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Red)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'9', (byte)'1', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Green)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'2', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Yellow)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'3', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Blue)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'4', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Magenta)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'5', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Cyan)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'6', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.White)
            {
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'1', (byte)'m' });
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'7', (byte)'m' });
            }
            else if (fcolor == ConsoleColor.Black)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'0', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkRed)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'1', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkGreen)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'2', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkYellow)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'3', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkBlue)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'4', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkMagenta)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'5', (byte)'m' });
            else if (fcolor == ConsoleColor.DarkCyan)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'6', (byte)'m' });
            else if (fcolor == ConsoleColor.Gray)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'3', (byte)'7', (byte)'m' });

            if (bcolor == ConsoleColor.Black)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'0', (byte)'m' });
            else if (bcolor == ConsoleColor.Red)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'1', (byte)'m' });
            else if (bcolor == ConsoleColor.Green)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'2', (byte)'m' });
            else if (bcolor == ConsoleColor.Yellow)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'3', (byte)'m' });
            else if (bcolor == ConsoleColor.Blue)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'4', (byte)'m' });
            else if (bcolor == ConsoleColor.Magenta)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'5', (byte)'m' });
            else if (bcolor == ConsoleColor.Cyan)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'6', (byte)'m' });
            else if (bcolor == ConsoleColor.White)
                tools.writebytes(new byte[] { 0x1b, (byte)'[', (byte)'4', (byte)'7', (byte)'m' });
        }
    }
}
