namespace CenterBase
{
    public class Single<T> where T : Single<T>
    {
        private static T ins;

        public static T instance
        {
            get
            {
                if (ins == null)
                    ins = new Single<T>() as T;

                return ins;
            }
        }
    }
}