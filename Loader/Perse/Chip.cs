namespace Loader
{
    public class Chip
    {
        public double Time;
        public double BPM;
        public double Scroll = 1;
        public ENote Type;
        public Chip? LongEnd = null;
        
        public int Bar;
        public int Position;
        public int Face;
        
        public bool Hit;
        public double? HitTime = null;
    }
    
    public enum ENote
    {
        None = 0,
        Don = 1,
        Ka = 2,
        DON = 3,
        KA = 4,
        Roll = 5,
        ROLL = 6,
        Balloon = 7,
        End = 8,
        Potato = 9,
        
        //SpiCatsロングノーツ
        LDon = 10,
        LKa = 11,
        LDON = 12,
        LKA = 13,
    }
}