//22.06.2016 (класс отвечает за геометрические вычисления)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Anvil
{
    public class Geometry
    {
        public static Point RotatePoint(Point Input, float degAngle)
        {
            double Angle = degAngle * (Math.PI / 180);
            int x = Input.X;
            int y = Input.Y;
            int rx = (int)Math.Round((Convert.ToDouble(x) * Math.Cos(Angle)) - (Convert.ToDouble(y) * Math.Sin(Angle)));
            int ry = (int)Math.Round((Convert.ToDouble(x) * Math.Sin(Angle)) + (Convert.ToDouble(y) * Math.Cos(Angle)));

            return new Point(rx, ry);
        }

        public static int GetAngle(Point First, Point Second)
        {
            double X1 = First.X, Y1 = First.Y;
            double X2 = Second.X, Y2 = Second.Y;
            double Angle = Math.Atan2(Y2 - Y1, X2 - X1) * (180 / Math.PI);
            Angle = (Angle < 0) ? Angle + 360 : Angle;

            return (int)Math.Round(Angle);
        }

        public static bool isRightAngle(int Angle) //Проверка прямоты угла
        {
            return (Angle == 0) || (Angle == 90) || (Angle == 180) || (Angle == 270) || (Angle == 360);
        }
    }
}
