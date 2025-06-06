namespace EStore.Application.Models.Files
{
    public class PushingFileMessage
    {
        public string FilePath { get; set; }
        public Guid FileId { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
    }
}