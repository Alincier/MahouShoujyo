﻿using MahouShoujyo.Content.Tiles.MusicBox;
using Terraria.ID;
using Terraria.ModLoader;

namespace MahouShoujyo.Content.Items.Placeable
{
    public class MusicBox_SisPuellaMagica : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

            // The following code links the music box's item and tile with a music track:
            //   When music with the given ID is playing, equipped music boxes have a chance to change their id to the given item type.
            //   When an item with the given item type is equipped, it will play the music that has musicSlot as its ID.
            //   When a tile with the given type and Y-frame is nearby, if its X-frame is >= 36, it will play the music that has musicSlot as its ID.
            // When getting the music slot, you should not add the file extensions!
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Radio/Music/SisPuellaMagica"), ModContent.ItemType<MusicBox_SisPuellaMagica>(), ModContent.TileType<MusicBox_SisPuellaMagica_Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<MusicBox_SisPuellaMagica_Tile>(), 0);
        }
    }
}
