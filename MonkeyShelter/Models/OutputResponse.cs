namespace MonkeyShelter.Models
{
    public class OutputResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
