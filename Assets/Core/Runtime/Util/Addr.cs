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
                public static class Popups
                {
                    public const string Popup_Generic = "content/ui/popup_generic";
                }
                
                public static class Pages
                {
                    public const string Main_Menu_Generic = "content/ui/pages/main_menu_generic";
                    public const string Options_Page = "content/ui/pages/options";
                    public const string Main_Menu_Page = "content/ui/pages/main_menu";
                }
                
                public static class Overlays
                {
                    public const string FadeFull    = "content/ui/overlays/overlay_fade_full";
                    public const string LoadingFull = "content/ui/overlays/overlay_loading_full";
                }

                public static class Toasts
                {
                    public const string Default = "content/ui/toasts/toast_generic";
                }

            }

            public static class Audio
            {
                public static class Bgm
                {
                    public const string MenuTheme = "content/audio/bgm/menu_theme";
                    public const string GameTheme = "content/audio/bgm/game_theme";
                }
                
            }
        }
    }
}