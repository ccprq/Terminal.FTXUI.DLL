using System;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Color;
using Terminal.String;
using Terminal.Mouse;

namespace Terminal.FTXUI
{
   public  class Drawing
    {
        public static void DrawBox(int x, int y, int wid, int hei)
        {
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < hei; j++)
                {
                    if (j == 0 || j == hei - 1)
                    {
                        if (i != 0)
                        {
                            extension.SR.Position(x + i, y + j).Write();
                        }
                        if (i == 0 && j == 0)
                        {
                            extension.LU.Position(x, y).Write();
                        }
                        else if (i == wid - 1 && j == 0)
                        {
                            extension.RU.Position(x + wid, y).Write();
                        }
                        else if (i == 0 && j == hei - 1)
                        {
                            extension.LD.Position(x, y + j).Write();
                        }
                        else if (i == wid - 1 && j == hei - 1)
                        {
                            extension.RD.Position(x + i + 1, y + j).Write();
                        }
                    }
                    else
                    {
                        extension.S.Position(x, y + j).PadRight(wid).AddRight(extension.S).Write();
                    }
                }
            }
        }
    }
    public class Label
    {
        private int _X = 0;
        private int _Y = 0;
        private string _Label = null;
        private string _ForeColor = null;
        private string _BackColor = null;
        public Label((int x, int y) pos, (string forecolor, string backcolor) colors, string text)
        {
            _X = pos.x;
            _Y = pos.y;
            _Label = text;
            _ForeColor = colors.forecolor;
            _BackColor = colors.backcolor;
        }
        public void Draw()
        {
            if (_Label != null)
            {
                if(_BackColor == "")
                {
                    _Label.Position(_X, _Y).WriteFore(_ForeColor);
                }
                else _Label.Position(_X, _Y).WriteForeBackGround(_ForeColor, _BackColor);
            }
        }
        public int Width()
        {
            if (_Label != null)
            {
                return _Label.Length;
            }
            else
            {
                return -1;
            }
        }
        public int Height()
        {
            if (_Label != null)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    public class Button
    {
        private int _X = 0;
        private int _Y = 0;
        private string _Label = null;
        private string _ForeColor = null;
        private Action _OnClick = null;
        public Button((int x, int y) pos, (string label, string forecolor) text, Action onclick)
        {
            _X = pos.x;
            _Y = pos.y;
            _Label = text.label;
            _ForeColor = text.forecolor;
            _OnClick = onclick;
        }

        public void Draw()
        {
            Drawing.DrawBox(_X, _Y, _Label.Length + 3, 3);
            _Label.
            Position(_X + 2, _Y + 1).
            WriteFore(_ForeColor);
        }
        public int Width()
        {
            if (_Label != null)
            {
                return _Label.Length + 3;
            }
            else
            {
                return -1;
            }
        }
        public int Height()
        {
            if (_Label != null)
            {
                return 3;
            }
            else
            {
                return -1;
            }
        }
        private void Clicked()
        {
            _Label.
            Position(_X + 2, _Y + 1).
            WriteFore(Color.Color.Azure);
            Thread.Sleep(100);
            _Label.
            Position(_X + 2, _Y + 1).
            WriteFore(_ForeColor);
        }
        public bool On_Click()
        {
            if (Mouse.Mouse.IsLeftClick() && Mouse.Mouse.MouseHover(_X, _X + _Label.Length + 3, _Y, _Y + 3))
            {
                Clicked();
                _OnClick();
                return true;
            }
            else return false;
        }
    }
}
