namespace hp55games
{
    /// <summary>
    /// Chiavi Addressables centralizzate (niente stringhe sparse).
    /// Convenzione: "config/..." e "content/<categoria>/<nome>"
    /// </summary>
    public static class Addr
    {
        public static class Config
        {
            public const string Main = "config/main";
        }

        public static class Content
        {
            public static class UI
            {
                public const string Popup_Generic = "content/ui/popup_generic";
                // aggiungi qui altre UI se servono
            }

            // Esempi futuri:
            // public static class Audio { public const string Sfx_Click = "content/audio/sfx_click"; }
            // public static class Tables { public const string Weapons = "content/weapons/table"; }
        }
    }
}