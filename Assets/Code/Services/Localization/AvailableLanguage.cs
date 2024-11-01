namespace Code.Services
{
    public class AvailableLanguage
    {
        public string TwoLetterISOLanguageName;
        public string DisplayedName;

        public AvailableLanguage(string twoLetterISOLanguageName, string displayedName)
        {
            TwoLetterISOLanguageName = twoLetterISOLanguageName;
            DisplayedName = displayedName;
        }
    }
}