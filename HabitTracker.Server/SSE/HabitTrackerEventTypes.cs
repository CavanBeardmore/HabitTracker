namespace HabitTracker.Server.SSE
{
    public static class HabitTrackerEventTypes
    {
        public const string USER_CREATED = "USER_CREATED";
        public const string USER_UPDATED = "USER_UPDATED";
        public const string USER_DELETED = "USER_DELETED";
        public const string LOGGED_IN = "LOGGED_IN";
        public const string HABIT_ADDED = "HABIT_ADDED";
        public const string HABIT_DELETED = "HABIT_DELETED";
        public const string HABIT_UPDATED = "HABIT_UPDATED";
        public const string HABIT_LOG_ADDED = "HABIT_LOG_ADDED";
        public const string HABIT_LOG_UPDATED = "HABIT_LOG_UPDATED";
        public const string HABIT_LOG_DELETED = "HABIT_LOG_DELETED";
        public const string ERROR = "ERROR";
    }
}
