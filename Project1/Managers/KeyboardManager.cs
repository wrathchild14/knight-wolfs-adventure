using Microsoft.Xna.Framework.Input;

namespace Managers
{
    internal class KeyboardManager
    {
        private static KeyboardState currentKeyState_;
        private static KeyboardState previousKeyState_;

        public static KeyboardState GetState()
        {
            previousKeyState_ = currentKeyState_;
            currentKeyState_ = Keyboard.GetState();
            return currentKeyState_;
        }

        public static bool IsPressed(Keys key)
        {
            return currentKeyState_.IsKeyDown(key);
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyState_.IsKeyDown(key) && !previousKeyState_.IsKeyDown(key);
        }
    }
}