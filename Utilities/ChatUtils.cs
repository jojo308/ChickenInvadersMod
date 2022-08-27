using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace ChickenInvadersMod.Utilities
{
    class ChatUtils
    {
        public static Color DefaultColor = Color.White;
        public static Color EventColor = new Color(175, 75, 255);

        /// <summary>
        /// Sends a message in chat
        /// </summary>
        /// <param name="message">The message to be send</param>
        public static void SendMessage(string message)
        {
            SendMessage(message, DefaultColor);
        }

        /// <summary>
        /// Sends a message in chat
        /// </summary>
        /// <param name="message">The message to be send</param>
        /// <param name="color">The color of the message</param>
        public static void SendMessage(string message, Color color)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(message, color);
            }
        }
    }
}
