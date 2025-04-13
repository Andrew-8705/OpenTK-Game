using Open_TK;

namespace Open_TK
{
    class Program
    {
        static void Main(string[] args) {
            using (Game game = new Game(2000, 1700))
            {
                game.Run();
            }
        }
    }
}