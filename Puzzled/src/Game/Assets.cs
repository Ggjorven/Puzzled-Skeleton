using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace Puzzled
{

    //////////////////////////////////////////////////////////////////////////////////
    // Structs
    //////////////////////////////////////////////////////////////////////////////////
    public struct Save
    {
        [JsonInclude]
        public string Name;
        [JsonInclude]
        public uint Level;

        [JsonInclude]
        public List<uint> Scores;
    }

    public class SortSaveScoreHelper : IComparer<Save>
    {

        //////////////////////////////////////////////////////////////////////////////////
        // Constructor
        //////////////////////////////////////////////////////////////////////////////////
        public SortSaveScoreHelper(uint level)
        {
            m_Level = level;
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Methods
        //////////////////////////////////////////////////////////////////////////////////
        public int Compare(Save x, Save y)
        {
            if (x.Scores[(int)m_Level] > y.Scores[(int)m_Level]) return 1;
            if (x.Scores[(int)m_Level] < y.Scores[(int)m_Level]) return -1;
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Variables
        //////////////////////////////////////////////////////////////////////////////////
        private uint m_Level;

    }

    //////////////////////////////////////////////////////////////////////////////////
    // Assets
    //////////////////////////////////////////////////////////////////////////////////
    public class Assets
    {

        ////////////////////////////////////////////////////////////////////////////////////
        // Paths
        ////////////////////////////////////////////////////////////////////////////////////
        public static string ResourcesDirectory = "../../../Puzzled/"; // Debug & Release path
        //public static string ResourcesDirectory = ""; // Distribution path

        public static string IntroMusicPath = ResourcesDirectory + "Resources/Music/MainMenu_Intro.wav";
        public static string MainMenuMusicPath = ResourcesDirectory + "Resources/Music/MainMenu_Loop.wav";
        public static string LevelMusicPath = ResourcesDirectory + "Resources/Music/Level.wav";
        public static string WinMenuMusicPath = ResourcesDirectory + "Resources/Music/WinMenu.wav";

        public static string JumpSoundPath = ResourcesDirectory + "Resources/SFX/Jump.wav";
        public static string KeyPickupSoundPath = ResourcesDirectory + "Resources/SFX/Key_Pickup.wav";
        public static string BoxPushSoundPath = ResourcesDirectory + "Resources/SFX/Box_Push.wav";

        public static string MainMenuLogoPath = ResourcesDirectory + "Resources/Textures/Main-Logo.png";

        public static string IdleSheetPath = ResourcesDirectory + "Resources/Sprites/idle.png";
        public static string JumpSheetPath = ResourcesDirectory + "Resources/Sprites/jump.png";
        public static string PainSheetPath = ResourcesDirectory + "Resources/Sprites/pain.png";
        public static string PushSheetPath = ResourcesDirectory + "Resources/Sprites/push.png";
        public static string RunSheetPath = ResourcesDirectory + "Resources/Sprites/run.png";
        public static string ObjectsSheetPath = ResourcesDirectory + "Resources/Sprites/objects.png";
        public static string TileSheetPath = ResourcesDirectory + "Resources/Sprites/tiles.png";
        public static string DoorKeySheetPath = ResourcesDirectory + "Resources/Sprites/doorkey.png";

        public static string Level1Path = ResourcesDirectory + "Resources/Levels/level-1.json";
        public static string Level2Path = ResourcesDirectory + "Resources/Levels/level-2.json";
        public static string Level3Path = ResourcesDirectory + "Resources/Levels/level-3.json";
        public static string Level4Path = ResourcesDirectory + "Resources/Levels/level-4.json";
        public static string Level5Path = ResourcesDirectory + "Resources/Levels/level-5.json";
        public static string Level6Path = ResourcesDirectory + "Resources/Levels/level-6.json";
        public const uint LevelCount = 6;

        public static string SaveQuotesPath = ResourcesDirectory + "Resources/Saves/Quotes.txt";

        public static string LevelToPath(uint level)
        {
            switch (level)
            {
            case 1:     return Level1Path;
            case 2:     return Level2Path;
            case 3:     return Level3Path;
            case 4:     return Level4Path;
            case 5:     return Level5Path;
            case 6:     return Level6Path;

                default:
                break;
            }

            return "<INVALID>";
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // MainMenu/Saves menu
        ////////////////////////////////////////////////////////////////////////////////////
        public static FireableAudio IntroMusic = new FireableAudio(IntroMusicPath, UserSettings.MusicVolume);
        public static LoopAudio MainMenuMusic = new LoopAudio(MainMenuMusicPath, UserSettings.MusicVolume);
        public static LoopAudio LevelMusic = new LoopAudio(LevelMusicPath, UserSettings.MusicVolume);
        public static LoopAudio WinMenuMusic = new LoopAudio(WinMenuMusicPath, UserSettings.MusicVolume);

        public static FireableAudio JumpSound = new FireableAudio(JumpSoundPath, UserSettings.SFXVolume);
        public static FireableAudio KeyPickupSound = new FireableAudio(KeyPickupSoundPath, UserSettings.SFXVolume);

        public static Texture MainMenuLogo = new Texture(MainMenuLogoPath);
        public static Texture WhiteTexture = new Texture();
        public static Texture BlackTexture = new Texture(new byte[]{ 0, 0, 0, 255 });

        ////////////////////////////////////////////////////////////////////////////////////
        // Tilesheets
        ////////////////////////////////////////////////////////////////////////////////////
        public static Texture IdleSheet = new Texture(IdleSheetPath);
        public static Texture JumpSheet = new Texture(JumpSheetPath);
        public static Texture PainSheet = new Texture(PainSheetPath);
        public static Texture PushSheet = new Texture(PushSheetPath);
        public static Texture RunSheet = new Texture(RunSheetPath);
        public static Texture ObjectsSheet = new Texture(ObjectsSheetPath);
        public static Texture DoorKeySheet = new Texture(DoorKeySheetPath);

        public static Texture TileSheet = new Texture(TileSheetPath);

        ////////////////////////////////////////////////////////////////////////////////////
        // Saves
        ////////////////////////////////////////////////////////////////////////////////////
        public static string GetSaveSlotPath(uint slot)
        {
            string directory = Assets.ResourcesDirectory + "Resources/Saves/";
            string saveSlotFilename = "save-" + slot + ".json";
            return System.IO.Path.Combine(directory, saveSlotFilename);
        }

        public static Save LoadSave(uint slot)
        {
            Logger.Info($"Save file loading from: {GetSaveSlotPath(slot)}.");

            string json = File.ReadAllText(GetSaveSlotPath(slot));
            return JsonSerializer.Deserialize<Save>(json);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Tiles
        ////////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<Texture, 
            Dictionary<(uint x, uint y), CroppedTexture>
        > s_TextureCache = new Dictionary<Texture, Dictionary<(uint x, uint y), CroppedTexture>>();

        public static CroppedTexture GetTexture(Texture texture, uint x, uint y)
        {
            if (!s_TextureCache.ContainsKey(texture))
                s_TextureCache.Add(texture, new Dictionary<(uint x, uint y), CroppedTexture>());

            Dictionary<(uint x, uint y), CroppedTexture> textureCache = s_TextureCache[texture];

            if (textureCache.ContainsKey((x, y)))
                return textureCache[(x, y)];

            textureCache.Add((x, y), new CroppedTexture(texture, new UV(x, y, (Settings.SpriteSize / Settings.Scale), (Settings.SpriteSize / Settings.Scale))));
            return textureCache[(x, y)];
        }

    }

}