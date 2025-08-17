namespace FrameDrive
{
    public struct Fraction<T> where T:struct
    {
        public T Numerator;
        public T Denominator;
        
        public Fraction(T numerator, T denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}