using System.Linq;

namespace TicTacToe
{
    static class Extensions
    {
        public static bool Is<T>(this T Item, params T[] Items) => Items.Contains(Item);
    }
}
