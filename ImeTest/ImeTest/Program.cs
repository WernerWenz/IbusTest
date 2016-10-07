using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Text;

namespace ImeTest
{
	class MainClass
	{
        private static Bitmap _bmp;
        private static Graphics _gr;
        private static Font _fnt;
        private static Brush _br;
        private static Brush _brRed;
        private static Pen _pen;

        private static string _preEdit;
        private static string _aux;
        private static string _selCand;
        private static int _selCandIdx;
        private static string[] _candidates = new string[0];
        private static int _preCur;

        public static void UpdateTexSD()
        {
            var mem = _bmp.LockBits(new Rectangle(Point.Empty, _bmp.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, mem.Stride / 4);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _bmp.Width, _bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, mem.Scan0);
            _bmp.UnlockBits(mem);
        }


        public static void UpdateTextSD()
        {
            
            _gr.Clear(Color.Transparent);
            System.Windows.Forms.TextRenderer.DrawText(_gr, "Pre: " +_preEdit, _fnt, new Point(10, 6), Color.Green);
            if (_preEdit != null)
            {
                var s1 = "Pre: " + _preEdit;
                var s2 = "Pre: " + new string(_preEdit.Take(_preCur).ToArray());
                var s3 = new string(_preEdit.Skip(_preCur + 1).ToArray());
                var totLen = System.Windows.Forms.TextRenderer.MeasureText(s1, _fnt);
                var start = System.Windows.Forms.TextRenderer.MeasureText(s2, _fnt);
                var endLen = System.Windows.Forms.TextRenderer.MeasureText(s3, _fnt);
                var selLen = System.Windows.Forms.TextRenderer.MeasureText(_selCand, _fnt);
                var end1 = totLen.Width - endLen.Width;
                var end2 = start.Width + selLen.Width;
                var end = end1 > end2 ? end1 : end2;

                if (s3.Length == 0)
                    _gr.DrawLine(_pen, 10 + start.Width, 22, 10 + start.Width + 10, 22);
                else                    
                    _gr.DrawLine(_pen, 10 + start.Width, 22, 10 + end2, 22);
            }

            System.Windows.Forms.TextRenderer.DrawText(_gr, "Aux: " +_aux, _fnt, new Point(10, 26), Color.Green);


            //var cands =　_candidates.Length <= 0 ? "<none>" : _candidates.Aggregate((x, y) => x + " " + y);
            //System.Windows.Forms.TextRenderer.DrawText(_gr, "Can: " + cands, _fnt, new Point(10, 46), Color.Green);

            var xPos = 0;

            for (int i = 0; i < _candidates.Length; i++)
            {
                var cand = _candidates[i];
                System.Windows.Forms.TextRenderer.DrawText(_gr, cand, _fnt, new Point(xPos, 46), i == _selCandIdx ? Color.Red : Color.Green);
                xPos += System.Windows.Forms.TextRenderer.MeasureText(cand, _fnt).Width;
                //_gr.DrawLine(_pen, xPos, 62, xPos, 66);
                xPos += 1;
            }

            _gr.Flush();
            UpdateTexSD();
        }
  
        
		public static void Main (string[] args)
		{
			Ibus ibus = null;
            int texId = 0; 
         
			using (var win = new GameWindow ())
            using (_bmp = new Bitmap(256, 128, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            using (_gr = Graphics.FromImage(_bmp))
            using (_fnt = new Font("unifont", 12.0f, FontStyle.Regular, GraphicsUnit.Point, 0))
            using (_br = new SolidBrush(Color.Green))
            using (_brRed = new SolidBrush(Color.Red))
            using (_pen = new Pen(Color.Red))
			{
                var impl = (OpenTK.Platform.X11.X11GLNative)typeof(NativeWindow).GetField("implementation", System.Reflection.BindingFlags.Instance　|　System.Reflection.BindingFlags.NonPublic).GetValue(win);
                Console.WriteLine("{0}", impl);

				win.Load += (s, e) => 
				{
					ibus = new Ibus();
                    texId = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texId);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                    ibus.UpdatePreedit += (_s, _p, _v) => 
                    {
                        _preEdit = _s;
                        _preCur = _p;
                        UpdateTextSD();
                    };
                    ibus.UpdateAuxiliary += (_s, _p, _v) => 
                    {
                        _aux = _s;
                        UpdateTextSD();
                    };
                    ibus.UpdateCandidates += (_c, _v, _p) =>
                    {
                        _candidates = _c;
                        if (_p < _c.Length)
                            _selCand = _candidates[_p];
                        else
                            _selCand = ""; 
                        _selCandIdx = (int)_p;
                        UpdateTextSD();
                    };


                    UpdateTextSD();
				};

				win.Unload += (s, e) => 
				{
					ibus.Dispose();
                    GL.DeleteTexture(texId);
				};

                /*
				win.KeyDown += (s, e) =>
				{
					ibus.TestKey(e, true);
				};

				win.KeyUp += (s, e) =>
				{
					ibus.TestKey(e, false);
				};*/

                impl.KeyDownRaw += (s, e) => ibus.TestKey(e, true, win.Keyboard.GetState());
                impl.KeyUpRaw += (s, e) => ibus.TestKey(e, false, win.Keyboard.GetState());

                win.FocusedChanged += (s, e) =>
                {
                    ibus.TestSetFocus(win.Focused);
                };

                win.Move += (s, e) =>
                {
                    var pt = win.PointToScreen(Point.Empty);
                    //ibus.TestCursor(pt.X, pt.Y);
                };

				win.RenderFrame += (s, e) =>
				{
                    ibus.Process();
					GL.ClearColor(0.8f, 0.8f, 1.0f, 1.0f);
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.Viewport(win.ClientSize);
                    var mOrtho = Matrix4.CreateOrthographicOffCenter(0, win.ClientSize.Width, win.ClientSize.Height, 0, -1, 1);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref mOrtho);

                    GL.Enable(EnableCap.Texture2D);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
                    GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(_bmp.Width, 0.0f);
                    GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(_bmp.Width, _bmp.Height);
                    GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0.0f, _bmp.Height);
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);

					win.SwapBuffers();
				};
				win.Run (0.1f, 0.1f);
			}
		}
	}
}
