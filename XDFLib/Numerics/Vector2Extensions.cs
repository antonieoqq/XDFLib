using System.Numerics;
using System.Runtime.CompilerServices;

namespace XDFLib.Numerics
{
    public static class Vector2Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalized(this Vector2 vector)
        {
            return Vector2.Normalize(vector);
        }
    }
}
