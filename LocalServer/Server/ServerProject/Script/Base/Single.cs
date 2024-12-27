namespace GameServer{
    public class Single<T> where T: Single<T>,new()
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    
    
    }

}

