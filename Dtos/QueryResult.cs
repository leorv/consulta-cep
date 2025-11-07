
namespace ConsultaCEP.Dtos
{
    public class QueryResult
    {
        public bool? Exists { get; set; }
        public bool? Success { get; set; }
        public string? Source { get; set; }
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}