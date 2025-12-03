using System;

namespace hp55games.Mobile.Core.Save
{
    [Serializable]
    public class OptionsData
    {
        public float music = 0.8f;
        public float sfx   = 0.8f;
        public bool  hapt  = true;
        public string lang = "en";
        public bool  musicMute = false;
        public bool  sfxMute   = false;
    }
}