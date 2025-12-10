using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XDFLib
{
    public static partial class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreEqual<T>(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] CreateFilledArray<T>(int length, T value)
        {
            var array = new T[length];
            var span = MemoryMarshal.CreateSpan(ref array[0], length);
            span.Fill(value);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,] CreateFilledArray<T>(int rows, int cols, T value)
        {
            var array = new T[rows, cols];
            var span = MemoryMarshal.CreateSpan(ref array[0, 0], rows * cols);
            span.Fill(value);
            return array;
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
