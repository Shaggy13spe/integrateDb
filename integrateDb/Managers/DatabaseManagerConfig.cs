namespace integrateDb.Managers {
    public class DatabaseManagerConfig {
        public static DatabaseManagerConfig GlobalConfig { get; } = new DatabaseManagerConfig { IsGlobalConfig = true };

        public bool SerializeTests { get; set; } = true;
        public bool IsGlobalConfig { get; private set; }
        public bool ReuseConnection { get; set; }
    }
}
