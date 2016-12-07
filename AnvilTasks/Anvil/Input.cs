//22.06.2016
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Forms = System.Windows.Forms;
using System.Windows.Input;

namespace Anvil
{
    public class Input
    {
        public static Dictionary<Key, KeyEvent> Keys = new Dictionary<Key, KeyEvent>();
        public static KeyEvent MouseLeft = new KeyEvent();
        public static KeyEvent MouseRight = new KeyEvent();

        public static bool Enabled = true;

        public static void Start()
        {
            
        }

        public static void Update()
        {
            UpdateKeys();
        }

        public static Point GetCursorPosition() //Позиция курсора внутри картинки
        {
            return GUI.CurrentPicture.Parent.PointToClient(Forms.Cursor.Position);
        }

        public static bool Hover(Rect R) //Факт нахождения курсора в прямоугольнике
        {
            Point P = GetCursorPosition();

            return (P.X > R.X && P.Y > R.Y && P.X < R.X + R.Width && P.Y < R.Y + R.Height) && Enabled;
        }

        //

        public static bool GetKey(Key key)
        {
            PushKey(key);

            return Keys[key].Pressed && Enabled;
        }

        public static bool GetKeyDown(Key key)
        {
            PushKey(key);

            return Keys[key].Down && Enabled;
        }

        public static bool GetKeyUp(Key key)
        {
            PushKey(key);

            return Keys[key].Up && Enabled;
        }

        public static bool GetMouseLeft()
        {
            return MouseLeft.Pressed && Enabled;
        }

        public static bool GetMouseLeftDown()
        {
            return MouseLeft.Down && Enabled;
        }

        public static bool GetMouseLeftUp()
        {
            return MouseLeft.Up && Enabled;
        }

        public static bool GetMouseRight()
        {
            return MouseRight.Pressed && Enabled;
        }

        public static bool GetMouseRightDown()
        {
            return MouseRight.Down && Enabled;
        }

        public static bool GetMouseRightUp()
        {
            return MouseRight.Up && Enabled;
        }

        //

        private static void PushKey(Key key)
        {
            if (!Keys.ContainsKey(key))
            {
                Keys.Add(key, new KeyEvent());
            }
        }

        public static void UpdateKeys()
        {
            foreach (KeyValuePair<Key, KeyEvent> K in Keys)
            {
                KeyEvent KE = K.Value;

                UpdateKeyEvent(Keyboard.IsKeyDown(K.Key), KE);
            }

            UpdateKeyEvent((Forms.Control.MouseButtons & Forms.MouseButtons.Left) == Forms.MouseButtons.Left, MouseLeft);
            UpdateKeyEvent((Forms.Control.MouseButtons & Forms.MouseButtons.Right) == Forms.MouseButtons.Right, MouseRight);
        }

        public static void UpdateKeyEvent(bool PressState, KeyEvent KE)
        {
            if (PressState) // Если нажато
            {
                if (!KE.Pressed)
                {
                    KE.Down = true;
                }
                else
                {
                    KE.Down = false;
                }

                KE.Pressed = true;
                KE.Up = false;

                KE.Life++;
            }
            else
            {
                if (KE.Pressed)
                {
                    KE.Up = true;
                }
                else
                {
                    KE.Up = false;
                }

                KE.Pressed = false;
                KE.Down = false;

                KE.Life = 0;
            }
        }

        public class KeyEvent
        {
            public int Life = 0;
            public bool Down = false;
            public bool Up = false;
            public bool Pressed = false;
        }
    }
}
