namespace EduWorld
{
    public static class Config
    {
        public static string GetApiUrl()
        {
#if API_URL
            return "https://67b30c2fbc0165def8cfb241.mockapi.io";
#else
            return "GG";
#endif
        }
    }
}
