//22.06.2016
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Anvil
{
    public class GUISkin
    {
        public GUIStyle Button = CreateButton();
        public GUIStyle Box = CreateBox();
        public GUIStyle Label = CreateLabel();

        //

        public static GUIStyle CreateButton()
        {
            return new GUIStyle
            {
                FontSize = 13f,
                FontName = "Arial",
                NormalColor = GUIColors.White,
                HoverColor = GUIColors.White,
                ActiveColor = GUIColors.White,
                NormalBackgroundColor = GUIColors.Orange,
                HoverBackgroundColor = GUIColors.OrangeHover,
                ActiveBackgroundColor = GUIColors.Orange,
                BorderRadius = 6,
            };
        }

        public static GUIStyle CreateBox()
        {
            return new GUIStyle
            {
                FontSize = 13f,
                FontName = "Arial",
                NormalColor = GUIColors.Gray,
                HoverColor = GUIColors.Gray,
                ActiveColor = GUIColors.Gray,
                NormalBackgroundColor = GUIColors.White,
                HoverBackgroundColor = GUIColors.White,
                ActiveBackgroundColor = GUIColors.White,
                BorderRadius = 8,
            };
        }

        public static GUIStyle CreateLabel()
        {
            return new GUIStyle
            {
                FontSize = 13f,
                FontName = "Arial",
                NormalColor = GUIColors.Gray,
                HoverColor = GUIColors.Gray,
                ActiveColor = GUIColors.Gray,
                NormalBackgroundColor = Color.Transparent,
                HoverBackgroundColor = Color.Transparent,
                ActiveBackgroundColor = Color.Transparent,
                BorderRadius = 0,
            };
        }
    }

    public class GUIStyle
    {
        public float FontSize = 13f;
        public string FontName = "Arial";
        public int BorderRadius = 0;
        public Color NormalColor = Color.Black;
        public Color HoverColor = Color.Black;
        public Color ActiveColor = Color.Black;
        public Color NormalBackgroundColor = Color.Transparent;
        public Color HoverBackgroundColor = Color.Transparent;
        public Color ActiveBackgroundColor = Color.Transparent;
    }

    public class GUIColors
    {
        public static readonly Color Orange = Color.FromArgb(255, 243, 156, 17);
        public static readonly Color OrangeHover = Color.FromArgb(255, 245, 181, 23);
        public static readonly Color Green = Color.FromArgb(255, 45, 204, 112);
        public static readonly Color GreenHover = Color.FromArgb(255, 88, 214, 141);
        public static readonly Color Blue = Color.FromArgb(255, 53, 152, 219);
        public static readonly Color BlueHover = Color.FromArgb(255, 92, 173, 226);
        public static readonly Color Red = Color.FromArgb(255, 232, 76, 61);
        public static readonly Color RedHover = Color.FromArgb(255, 238, 111, 102);
        public static readonly Color Gray = Color.FromArgb(255, 73, 74, 69);
        public static readonly Color GrayHover = Color.FromArgb(255, 103, 104, 98);
        public static readonly Color White = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color WhiteHover = Color.FromArgb(255, 238, 238, 238);
    }
}
