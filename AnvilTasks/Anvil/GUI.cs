//22.06.2016
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace Anvil
{
    public class GUI
    {
        public static GUISkin DefaultSkin = new GUISkin();
        public static GUISkin Skin = DefaultSkin;
        public static GUIStyle CurrentStyle = new GUIStyle();

        public static int Width = 0;
        public static int Height = 0;

        public static bool isStarted = false;

        public static PictureBox CurrentPicture;
        public static Graphics G;

        //

        public static Vector2 Offset = new Vector2(0, 0);
        public static Rect Clip = new Rect();
        public static List<Vector2> OffsetBuffer = new List<Vector2>();

        //

        public static void Update(PictureBox GUIPictureBox)
        {
            if (!isStarted)
            {
                Start();

                isStarted = true;
            }

            Input.Update(); // Энтропия ввода

            //

            CurrentPicture = GUIPictureBox;

            if (CurrentPicture.Width != 0 && CurrentPicture.Height != 0)
            {
                CurrentPicture.Image = null;
                CurrentPicture.Image = new Bitmap(CurrentPicture.Width, CurrentPicture.Height);

                Width = CurrentPicture.Width;
                Height = CurrentPicture.Height;
            }
            
            G = Graphics.FromImage(CurrentPicture.Image);
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //
            ResetOffset();
            ResetClip();
        }

        public static void Start()
        {

        }

        //

        public static bool Button(Rect rect, string text, GUIStyle Style = null)
        {
            if (Style == null) CurrentStyle = Skin.Button;
            else CurrentStyle = Style;
            //
            bool hover = Input.Hover(rect);
            bool press = Input.GetMouseLeft() || Input.GetMouseRight();
            bool click = Input.GetMouseLeftUp() || Input.GetMouseRightUp();
            //
            if (!hover)
            {
                DrawBox(rect, CurrentStyle.NormalBackgroundColor, CurrentStyle.BorderRadius);
            }
            else
            {
                if (press)
                {
                    DrawBox(rect, CurrentStyle.ActiveBackgroundColor, CurrentStyle.BorderRadius);
                }
                else
                {
                    DrawBox(rect, CurrentStyle.HoverBackgroundColor, CurrentStyle.BorderRadius);
                }
            }

            DrawLabel(rect, text, false, false, false, false);
            //

            return hover && click;
        }

        public static bool RepeatButton(Rect rect, string text, GUIStyle Style = null)
        {
            Button(rect, text, Style);
            return Input.Hover(rect) && (Input.GetMouseLeft() || Input.GetMouseRight());
        }

        public static void Box(Rect rect, string text, GUIStyle Style = null)
        {
            if (Style == null) CurrentStyle = Skin.Box;
            else CurrentStyle = Style;
            //
            DrawBox(rect, CurrentStyle.NormalBackgroundColor, CurrentStyle.BorderRadius);

            DrawLabel(new Rect(rect.X, rect.Y, rect.Width, (int)CurrentStyle.FontSize * 3), text, false, false, false, false); // ложь, ложь, ложь, ложь
        }

        public static void Label(Rect rect, string text, GUIStyle Style = null)
        {
            if (Style == null) CurrentStyle = Skin.Label;
            else CurrentStyle = Style;
            //
            DrawLabel(rect, text, true, true, false, false);
        }

        //

        public static bool isVerticalScrolling = false;
        public static bool isHorisontalScrolling = false;
        private static Rect VerticalScrollingRect = new Rect();
        private static Rect HorisontalScrollingRect = new Rect();

        public static Vector2 BeginScrollView(Rect R, Rect InnerRect, Vector2 ScrollOffset)
        {
            float RatioX = (float)R.Width / (float)(InnerRect.Width);
            float RatioY = (float)R.Height / (float)(InnerRect.Height);

            int ScrollWidth = 8;

            int OX = ScrollOffset.X;
            int OY = ScrollOffset.Y;

            Point MousePos = Input.GetCursorPosition();

            float BarX = (float)OX * RatioX;
            float BarY = (float)OY * RatioY;

            //

            bool Hover = Input.Hover(R);

            if (RatioX < 1)
            {
                int ScrollLength = (int)(R.Width * RatioX);

                if (Hover || isHorisontalScrolling)
                {
                    HorisontalScrollingRect = new Rect(R.X + (int)BarX, R.Y + R.Height - ScrollWidth, ScrollLength, ScrollWidth);

                    /*if (GUI.RepeatButton(new Rect(R.X + (int)BarX, R.Y + R.Height - ScrollWidth, (int)(R.Width * RatioX), ScrollWidth), ""))
                    {
                        isHorisontalScrolling = true;
                    }*/
                }
                

                if (Input.GetMouseLeftUp())
                {
                    isHorisontalScrolling = false;
                }

                if (isHorisontalScrolling)
                {
                    BarX = MousePos.X - ScrollLength / 2;
                }
            }

            if (RatioY < 1)
            {
                int ScrollLength = (int)(R.Height * RatioY);

                if (Hover || isVerticalScrolling)
                {
                    VerticalScrollingRect = new Rect(R.X + R.Width - ScrollWidth, R.Y + (int)BarY, ScrollWidth, ScrollLength);

                    /*if (GUI.RepeatButton(new Rect(R.X + R.Width - ScrollWidth, R.Y + (int)BarY, ScrollWidth, (int)(R.Height * RatioY)), ""))
                    {
                        isVerticalScrolling = true;
                    }*/
                } 

                if (Input.GetMouseLeftUp())
                {
                    isVerticalScrolling = false;
                }

                if (isVerticalScrolling)
                {
                    //OY = (int)(MousePos.Y * RatioY);

                    BarY = MousePos.Y - ScrollLength/2;
                }
            }

            //

            OX = (int)(BarX / RatioX);
            OY = (int)(BarY / RatioY);

            if (OX < 0) OX = 0;
            if (OY < 0) OY = 0;
            if (OX > InnerRect.Width - R.Width) OX = InnerRect.Width - R.Width;
            if (OY > InnerRect.Height - R.Height) OY = InnerRect.Height - R.Height;

            //
            if (RatioX >= 1) OX = 0;
            if (RatioY >= 1) OY = 0;
            //
            SetClip(R);
            SetOffset(new Vector2(R.X - OX, R.Y - OY));
            //

            return new Vector2(OX, OY);
        }

        public static void EndScrollView()
        {
            //ResetOffset();
            UndoOffset();
            ResetClip();
            //
            if (VerticalScrollingRect.Width != 0 || VerticalScrollingRect.Height != 0)
            {
                if (GUI.RepeatButton(VerticalScrollingRect, ""))
                {
                    isVerticalScrolling = true;
                }
            }

            if (HorisontalScrollingRect.Width != 0 || HorisontalScrollingRect.Height != 0)
            {
                if (GUI.RepeatButton(HorisontalScrollingRect, ""))
                {
                    isHorisontalScrolling = true;
                }
            }

            VerticalScrollingRect = new Rect();
            HorisontalScrollingRect = new Rect();
        }

        //

        public static void BeginArea(Rect R)
        {
            SetClip(R);
            SetOffset(new Vector2(R.X, R.Y));
        }

        public static void EndArea()
        {
            //ResetOffset();
            UndoOffset();
            ResetClip();
        }

        //

        public static void DrawLabel(Rect rect, string text, bool top, bool left, bool bottom, bool right)
        {
            Font font = new Font(CurrentStyle.FontName, CurrentStyle.FontSize);
            SizeF TextSize = G.MeasureString(text, font);

            int TextWidth = (int)TextSize.Width;
            int TextHeight = (int)TextSize.Height;

            int PosX = (rect.Width / 2) - (TextWidth / 2);
            int PosY = (rect.Height / 2) - (TextHeight / 2);

            if (left) PosX = 0;
            if (right) PosX = rect.Width - TextWidth;
            if (top) PosY = 0;
            if (bottom) PosY = rect.Height - TextHeight;

            //DrawQuad(new Rect(rect.X + PosX, rect.Y + PosY, TextWidth, TextHeight), Color.Black);

            Brush brush = new SolidBrush(CurrentStyle.NormalColor);

            G.DrawString(text, font, brush, rect.X + PosX, rect.Y + PosY);
        }

        public static void DrawBox(Rect rect, Color color, int Radius = 8)
        {
            Brush brush = new SolidBrush(color);

            if (rect.Height < Radius * 2) Radius = rect.Height / 2;
            if (rect.Width < Radius * 2) Radius = rect.Width / 2;

            if (Radius > 0)
            {
                DrawQuad(new Rect(rect.X, rect.Y + Radius, rect.Width, rect.Height - (Radius * 2)), color);
                DrawQuad(new Rect(rect.X + Radius, rect.Y, rect.Width - (Radius * 2), rect.Height), color);

                Point R = new Point(Radius, Radius);
                Point[] Angles = new Point[] {
                    new Point(rect.X + Radius, rect.Y + Radius),
                    new Point(rect.X + rect.Width - Radius, rect.Y + rect.Height - Radius),
                    new Point(rect.X + rect.Width - Radius, rect.Y + Radius),
                    new Point(rect.X + Radius, rect.Y + rect.Height - Radius),
                };

                for (int i = 0; i < Angles.Length; i++) DrawCircle(Angles[i + 0], R, color);
            }
            else
            {
                DrawQuad(new Rect(rect.X, rect.Y, rect.Width, rect.Height), color);
            }

        }

        public static void DrawQuad(Rect rect, Color color)
        {
            Brush brush = new SolidBrush(color);
            G.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void DrawCircle(Point Srart, Point Dir, Color color)
        {
            Brush brush = new SolidBrush(color);
            G.FillEllipse(brush, Srart.X - Dir.X, Srart.Y - Dir.Y, Dir.X * 2, Dir.Y * 2);
        }

        public static void DrawImage(Image image, Rect rect)
        {
            G.DrawImage(image, new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        public static void DrawPoint(int x, int y, Color color, float Scale = 1)
        {
            Brush brush = new SolidBrush(color);
            G.FillEllipse(brush, x, y, Scale, Scale);
        }

        public static void DrawLine(Point FirstPos, Point SecondPos, int width = 1)
        {
            G.DrawLine(new Pen(Color.White, width), FirstPos, SecondPos);
        }

        public static void SetOffset(Vector2 off)
        {
            Offset = new Vector2(Offset.X + off.X, Offset.Y + off.Y);
            OffsetBuffer.Add(off);
            G.TranslateTransform(off.X, off.Y);
        }

        public static void UndoOffset()
        {
            Vector2 offset = OffsetBuffer[OffsetBuffer.Count - 1];
            OffsetBuffer.RemoveAt(OffsetBuffer.Count - 1);
            G.TranslateTransform(-offset.X, -offset.Y);
        }

        public static void ResetOffset()
        {
            G.ResetTransform();
            Offset = new Vector2();
            OffsetBuffer.Clear();
        }

        public static void SetClip(Rect R)
        {
            Clip = R;
            G.Clip = new Region(new Rectangle(R.X, R.Y, R.Width, R.Height));
        }

        public static void ResetClip()
        {
            G.ResetClip();
        }

        public static void Clear(Color color)
        {
            //if (color == null) color = GUIColors.WhiteHover; // JavaScript style :)
            Brush brush = new SolidBrush(color);

            G.FillRectangle(brush, 0, 0, GUI.Width, GUI.Height);
        }
    }
}
