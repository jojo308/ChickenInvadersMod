using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace ChickenInvadersMod
{
    class ChatUtils
    {
        /// <summary>
        /// Sends a message in chat
        /// </summary>
        /// <param name="message">The message to be send</param>
        public static void SendMessage(string message)
        {
            SendMessage(message, Color.White);
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
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(message, color);
            }
        }
    }
}
