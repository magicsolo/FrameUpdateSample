namespace CenterBase
{
    public class Single<T> where T : Single<T>, new()
    {
        private static T ins;

        public static T instance
        {
            get
            {
                if (ins == null)
                    ins = new T();
                return ins;
            }
        }
    }
}