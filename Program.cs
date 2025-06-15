using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using App;
using OpenTK.Windowing.Common;
namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using Main window = new Main(1024, 546, "Window");
            window.Run();
        }
       

    }
}
