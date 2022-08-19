using System.Runtime.InteropServices;

namespace Swine.Demo
{
    public class CincozeService
    {
        [DllImport("inpout32.dll", EntryPoint = "Out32")]
        public static extern void Output(int adress, int value);

        [DllImport("inpout32.dll", EntryPoint = "Inp32")]
        public static extern int Input(int adress);

        [DllImport("inpout32.dll", EntryPoint = "IsInpOutDriverOpen")]
        public static extern bool IsInpOutDriverOpen();
    }
}
