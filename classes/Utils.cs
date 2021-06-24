using UnityEngine;
using Il2CppSystem.Collections.Generic;

class Utils
{

    public static void SetColor(SpriteRenderer renderer, float alpha)
    {
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }

    public static PlayerControl GetPlayerById(byte id)
    {
        List<PlayerControl> players = PlayerControl.AllPlayerControls;
        for (int i = 0; i < players.Count; i++)
        {
            PlayerControl player = players[i];
            if (player.Data.PlayerId == id)
            {
                return player;
            }
        }
        return null;
    }
}