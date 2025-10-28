using System.Numerics;
using System.Runtime.CompilerServices;

namespace XDFLib.Numerics
{
    public static class Vector3Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalized(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }
    }
}
