namespace Infura.SDK.Models
{
    public interface IPage<T>
    {
        public int PageNumber { get; }
        
        public string Cursor { get; }
        
        public T[] Contents { get; }
    }
}