using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace App
{
    class Main : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramObject;
        private int vertexArrayHandle;
        private int indexBufferHandle;

        
        public Main(int width, int height, string title)
        :base(GameWindowSettings.Default, new NativeWindowSettings()
        {

            Title = title,
            ClientSize = new Vector2i(width, height),
            WindowBorder = WindowBorder.Resizable,
            StartVisible = false,
            StartFocused = true,
            WindowState = WindowState.Normal,
            API = ContextAPI.OpenGL,
            Profile = ContextProfile.Core,
            APIVersion = new Version(3, 3)
        })
        {
            CenterWindow();
            Console.WriteLine("Main...");
        }

        protected override void OnResize(ResizeEventArgs e)
        {

            GL.Viewport(0, 0, e.Width, e.Height);
            Console.WriteLine("resizing...");
            base.OnResize(e);  
        }


        protected override void OnLoad()
        {
            
            IsVisible = true;

            ImageResult loadedIcon;
            using (Stream stream = File.OpenRead("./../../../images/icon.png")) loadedIcon = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            OpenTK.Windowing.Common.Input.Image _icon = new OpenTK.Windowing.Common.Input.Image(loadedIcon.Width, loadedIcon.Height, loadedIcon.Data);
            Icon = new WindowIcon(_icon);

            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));

            float x = 384f;
            float y = 400f;
            float w = 512f;
            float h = 256f;



            float[] vertices = new float[]
            {
                 x, y + h, 0f, 1f, 0f, 0f, 1f,     //vertex0 left
                 x + w, y + h, 0f, 0f, 1f, 0f, 1f, //vertex1 top left
                 y + w, y, 0f, 0f, 0f, 1f, 1f,     //vertex2 top right
                -x, y, 0f, 1f, 1f, 0f, 1f,         //vertex3 right
                 


            };

            int[] indices = new int[]
            {
                0, 1, 2, 0, 2, 3,
            };

            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            string vertexShaderCode = @"
            #version 330 core

            uniform ViewportSize;

            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec4 aColor;

            out vec4 vColor;
            
            void main()
            {
                float nx = aPosition.x / ViewportSize * 2f - 1f;
                float ny = aPosition.y / ViewportSize * 2f - 1;
                gl_Position = vec4(nx, ny, 0f, 1f);

                vColor = aColor;                
            }
             ";
            string pixelShaderCode = @"
            #version 330 core

                in vec4 vColor;

                out vec4 pixelColor;
                void main()
                {
                  pixelColor = vColor;
                }
            ";

            int vertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderObject, vertexShaderCode);
            GL.CompileShader(vertexShaderObject);

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderObject);
            if (vertexShaderInfo != string.Empty)
            {
                Console.WriteLine(vertexShaderInfo);
            }


            int pixelShaderObject = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderObject, pixelShaderCode);
            GL.CompileShader(pixelShaderObject);

            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderObject);
            if (vertexShaderInfo != string.Empty)
            {
                Console.WriteLine(pixelShaderInfo);
            }


            this.shaderProgramObject = GL.CreateProgram();

            GL.AttachShader(this.shaderProgramObject, vertexShaderObject);
            GL.AttachShader(this.shaderProgramObject, pixelShaderObject);

            GL.LinkProgram(this.shaderProgramObject);

            GL.DetachShader(this.shaderProgramObject, vertexShaderObject);
            GL.DetachShader(this.shaderProgramObject, pixelShaderObject);

            GL.DeleteShader(vertexShaderObject);
            GL.DeleteShader(pixelShaderObject);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            GL.UseProgram(this.shaderProgramObject);
            int viewportSizeUniformLoacation = GL.GetUniformLocation(this.shaderProgramObject, "ViewportSize");
            GL.Uniform2(viewportSizeUniformLoacation, (float)viewport[2], (float)viewport[3]);
            GL.UseProgram(0);


            base.OnLoad();
            Console.WriteLine("OnLoading...");


        }
        protected override void OnUnload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this.indexBufferHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.vertexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgramObject);

            base.OnUnload();
        }




        private static void OnDisplay()
        {

        }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(new Color4(0.0f, 0.4f, 0.7f, 1));
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgramObject);
            GL.BindVertexArray(this.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, 13, DrawElementsType.UnsignedInt, 0);

            

            Context.SwapBuffers();

            base.OnRenderFrame(args);

            if (KeyboardState.IsKeyPressed(Keys.Space))
            {
                spacePressed();
            }
           
        }

        private void spacePressed()
        {
            Console.WriteLine("found a space");
        }
    }
}
